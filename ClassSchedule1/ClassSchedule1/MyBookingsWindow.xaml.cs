using System;
using System.Collections.Generic;
using System.Windows;
using Npgsql;
using System.Windows.Threading;
using System.Windows.Controls;

namespace ClassSchedule1
{
    public partial class MyBookingsWindow : Window
    {
        private string connectionString = "Server=localhost;Database=ClassSchedule;User id=postgres;Password=postgres;";
        private int currentUserId;
        private List<Booking> bookings;
        private DispatcherTimer refreshTimer;

        public class Booking
        {
            public int BookingId { get; set; }
            public string RoomNumber { get; set; }
            public DateTime BookingDate { get; set; }
            public string StartTime { get; set; }
            public int Duration { get; set; }
        }

        public MyBookingsWindow(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
            LoadBookings();

            // Настройка таймера для обновления списка каждые 30 секунд
            refreshTimer = new DispatcherTimer();
            refreshTimer.Interval = TimeSpan.FromSeconds(30);
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            LoadBookings(); // Обновляем список бронирований
        }

        private void LoadBookings()
        {
            bookings = new List<Booking>();
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT b.BookingId, r.RoomNumber, b.BookingDate, b.StartTime, b.Duration
                        FROM Bookings b
                        JOIN Rooms r ON b.RoomId = r.RoomId
                        WHERE b.UserId = @UserId";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("UserId", currentUserId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bookings.Add(new Booking
                                {
                                    BookingId = reader.GetInt32(0),
                                    RoomNumber = reader.GetString(1),
                                    BookingDate = reader.GetDateTime(2),
                                    StartTime = reader.GetTimeSpan(3).ToString(@"hh\:mm"),
                                    Duration = reader.GetInt32(4)
                                });
                            }
                        }
                    }
                }
                BookingsListView.ItemsSource = bookings;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteBooking_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int bookingId = (int)button.Tag;

            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить это бронирование?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var conn = new NpgsqlConnection(connectionString))
                    {
                        conn.Open();
                        using (var cmd = new NpgsqlCommand("DELETE FROM Bookings WHERE BookingId = @BookingId", conn))
                        {
                            cmd.Parameters.AddWithValue("BookingId", bookingId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Бронирование успешно удалено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadBookings(); // Обновляем список
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditBooking_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int bookingId = (int)button.Tag;
            var booking = bookings.Find(b => b.BookingId == bookingId);

            if (booking != null)
            {
                var editWindow = new EditBookingWindow(bookingId, booking.BookingDate, booking.StartTime, booking.Duration, currentUserId);
                editWindow.ShowDialog();
                LoadBookings(); // Обновляем список после изменения
            }
        }
    }
}