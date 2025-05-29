using System;
using System.Windows;
using System.Windows.Controls;
using Npgsql;

namespace ClassSchedule1
{
    public partial class EditBookingWindow : Window
    {
        private string connectionString = "Server=localhost;Database=ClassSchedule;User id=postgres;Password=postgres;";
        private int bookingId;
        private int userId;

        public EditBookingWindow(int bookingId, DateTime bookingDate, string startTime, int duration, int userId)
        {
            InitializeComponent();
            this.bookingId = bookingId;
            this.userId = userId;

            BookingDatePicker.SelectedDate = bookingDate;
            StartTimeComboBox.SelectedItem = StartTimeComboBox.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == startTime);
            DurationComboBox.SelectedItem = DurationComboBox.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == $"{duration} час");
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            DateTime? selectedDate = BookingDatePicker.SelectedDate;
            string startTime = (StartTimeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string duration = (DurationComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (!selectedDate.HasValue || string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(duration))
            {
                MessageBox.Show("Заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int durationHours = int.Parse(duration.Split(' ')[0]);
            try
            {
                // Проверка, прошло ли выбранное время
                DateTime now = DateTime.UtcNow; // 03:03 PM UTC
                DateTime selectedDateTime;
                try
                {
                    TimeSpan startTimeSpan = TimeSpan.Parse(startTime);
                    selectedDateTime = selectedDate.Value.Date.Add(startTimeSpan).ToUniversalTime(); // Переводим в UTC
                }
                catch (FormatException)
                {
                    MessageBox.Show("Неверный формат времени.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (selectedDateTime < now)
                {
                    MessageBox.Show("Выбранное время уже прошло. Пожалуйста, выберите будущее или текущее время.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string conflictQuery = @"
                        SELECT COUNT(*)
                        FROM Bookings b
                        JOIN Rooms r ON b.RoomId = r.RoomId
                        WHERE b.BookingId != @BookingId
                        AND b.RoomId = (SELECT RoomId FROM Bookings WHERE BookingId = @BookingId)
                        AND b.BookingDate = @BookingDate
                        AND (
                            (b.StartTime <= @StartTime AND @StartTime < b.StartTime + INTERVAL '1 hour' * b.Duration)
                            OR (@StartTime <= b.StartTime AND b.StartTime < @StartTime + INTERVAL '1 hour' * @Duration)
                        )";
                    using (var cmd = new NpgsqlCommand(conflictQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("BookingId", bookingId);
                        cmd.Parameters.AddWithValue("BookingDate", selectedDate.Value.Date);
                        cmd.Parameters.AddWithValue("StartTime", TimeSpan.Parse(startTime));
                        cmd.Parameters.AddWithValue("Duration", durationHours);

                        int conflictCount = Convert.ToInt32(cmd.ExecuteScalar());
                        if (conflictCount > 0)
                        {
                            MessageBox.Show("Выбранное время пересекается с другим бронированием.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    string updateQuery = @"
                        UPDATE Bookings
                        SET BookingDate = @BookingDate, StartTime = @StartTime, Duration = @Duration
                        WHERE BookingId = @BookingId";
                    using (var cmd = new NpgsqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("BookingId", bookingId);
                        cmd.Parameters.AddWithValue("BookingDate", selectedDate.Value.Date);
                        cmd.Parameters.AddWithValue("StartTime", TimeSpan.Parse(startTime));
                        cmd.Parameters.AddWithValue("Duration", durationHours);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Бронирование успешно обновлено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}