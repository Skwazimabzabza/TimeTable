using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Npgsql;

namespace ClassSchedule1
{
    public partial class ReservationPage : Window
    {
        private string connectionString = "Server=localhost;Database=ClassSchedule;User id=postgres;Password=postgres;";
        private List<Room> availableRooms;
        private int currentUserId = 1;
        private DispatcherTimer cleanupTimer;

        public class Room
        {
            public int RoomId { get; set; }
            public string RoomNumber { get; set; }
            public string RoomType { get; set; }
            public int Capacity { get; set; }
        }

        public ReservationPage()
        {
            InitializeComponent();
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                }
                CheckAndRemoveExpiredBookings();
                LoadRooms();

                cleanupTimer = new DispatcherTimer();
                cleanupTimer.Interval = TimeSpan.FromSeconds(30);
                cleanupTimer.Tick += CleanupTimer_Tick;
                cleanupTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CleanupTimer_Tick(object sender, EventArgs e)
        {
            CheckAndRemoveExpiredBookings();
        }

        private void CheckAndRemoveExpiredBookings()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        DELETE FROM Bookings
                        WHERE BookingId IN (
                            SELECT BookingId
                            FROM Bookings b
                            WHERE (b.BookingDate + b.StartTime) + (INTERVAL '1 hour' * b.Duration) <= @CurrentTimestamp
                        )";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        DateTime now = DateTime.UtcNow; // 08:49 AM UTC
                        cmd.Parameters.AddWithValue("CurrentTimestamp", NpgsqlTypes.NpgsqlDbType.TimestampTz, now.ToUniversalTime());
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении истёкших бронирований: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRooms()
        {
            availableRooms = new List<Room>();
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT RoomId, RoomNumber, RoomType, Capacity FROM Rooms", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                availableRooms.Add(new Room
                                {
                                    RoomId = reader.GetInt32(0),
                                    RoomNumber = reader.GetString(1),
                                    RoomType = reader.GetString(2),
                                    Capacity = reader.GetInt32(3)
                                });
                            }
                        }
                    }
                }
                RoomsListView.ItemsSource = availableRooms;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки аудиторий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            DateTime? selectedDate = BookingDatePicker.SelectedDate;
            string startTime = (StartTimeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string duration = (DurationComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string roomType = (RoomTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (!selectedDate.HasValue || string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(duration) || string.IsNullOrEmpty(roomType))
            {
                MessageBox.Show("Заполните все фильтры.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int durationHours = int.Parse(duration.Split(' ')[0]);
            try
            {
                var filteredRooms = new List<Room>();
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT r.RoomId, r.RoomNumber, r.RoomType, r.Capacity
                        FROM Rooms r
                        WHERE r.RoomType = @RoomType
                        AND r.RoomId NOT IN (
                            SELECT b.RoomId
                            FROM Bookings b
                            WHERE b.BookingDate = @BookingDate
                            AND (
                                (b.StartTime <= @StartTime AND @StartTime < b.StartTime + INTERVAL '1 hour' * b.Duration)
                                OR (@StartTime <= b.StartTime AND b.StartTime < @StartTime + INTERVAL '1 hour' * @Duration)
                            )
                        )";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("RoomType", roomType);
                        cmd.Parameters.AddWithValue("BookingDate", selectedDate.Value.Date);
                        cmd.Parameters.AddWithValue("StartTime", TimeSpan.Parse(startTime));
                        cmd.Parameters.AddWithValue("Duration", durationHours);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                filteredRooms.Add(new Room
                                {
                                    RoomId = reader.GetInt32(0),
                                    RoomNumber = reader.GetString(1),
                                    RoomType = reader.GetString(2),
                                    Capacity = reader.GetInt32(3)
                                });
                            }
                        }
                    }
                }
                RoomsListView.ItemsSource = filteredRooms;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BookRoom_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int roomId = (int)button.Tag;
            DateTime? selectedDate = BookingDatePicker.SelectedDate;
            string startTime = (StartTimeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string duration = (DurationComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (!selectedDate.HasValue || string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(duration))
            {
                MessageBox.Show("Выберите дату, время и продолжительность.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime now = DateTime.UtcNow; // 08:49 AM UTC
            DateTime selectedDateTime;
            try
            {
                TimeSpan startTimeSpan = TimeSpan.Parse(startTime);
                selectedDateTime = selectedDate.Value.Date.Add(startTimeSpan).ToUniversalTime();
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверный формат времени.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (selectedDateTime < now)
            {
                MessageBox.Show("Выбранное время уже прошло. Пожалуйста, выберите будущее время для бронирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int durationHours = int.Parse(duration.Split(' ')[0]);
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(
                        "INSERT INTO Bookings (UserId, RoomId, BookingDate, StartTime, Duration) VALUES (@UserId, @RoomId, @BookingDate, @StartTime, @Duration)", conn))
                    {
                        cmd.Parameters.AddWithValue("UserId", currentUserId);
                        cmd.Parameters.AddWithValue("RoomId", roomId);
                        cmd.Parameters.AddWithValue("BookingDate", selectedDate.Value.Date);
                        cmd.Parameters.AddWithValue("StartTime", TimeSpan.Parse(startTime));
                        cmd.Parameters.AddWithValue("Duration", durationHours);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Аудитория успешно забронирована!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                ApplyFilters_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка бронирования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MyBookings_Click(object sender, RoutedEventArgs e)
        {
            var myBookingsWindow = new MyBookingsWindow(currentUserId);
            myBookingsWindow.ShowDialog();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}