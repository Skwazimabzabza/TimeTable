using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Npgsql;

namespace ClassSchedule1
{
    public partial class MainWindow : Window
    {
        private string connectionString = "Server=localhost;Database=ClassSchedule;User id=postgres;Password=postgres;";
        private DispatcherTimer passwordHideTimer;
        private bool isPasswordVisible = false;
        private Path eyeIcon;

        public MainWindow()
        {
            InitializeComponent();

            // Инициализация таймера
            passwordHideTimer = new DispatcherTimer();
            passwordHideTimer.Interval = TimeSpan.FromSeconds(5); // Скрытие через 5 секунд
            passwordHideTimer.Tick += PasswordHideTimer_Tick;

            // Инициализация иконки глаза
            eyeIcon = TogglePasswordButton.Content as Path;
        }

        private void TogglePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPasswordVisible)
            {
                // Скрываем пароль и показываем открытый глаз
                PasswordBox.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Hidden;
                PasswordBox.Password = PasswordTextBox.Text;
                eyeIcon.Data = Geometry.Parse("M12,4.5 C7.5,4.5 3.75,7.5 2.25,12 C3.75,16.5 7.5,19.5 12,19.5 C16.5,19.5 20.25,16.5 21.75,12 C20.25,7.5 16.5,4.5 12,4.5 Z M12,16.5 C9.75,16.5 7.5,14.25 7.5,12 C7.5,9.75 9.75,7.5 12,7.5 C14.25,7.5 16.5,9.75 16.5,12 C16.5,14.25 14.25,16.5 12,16.5 Z M12,9 C10.875,9 9.75,10.125 9.75,12 C9.75,13.875 10.875,15 12,15 C13.125,15 14.25,13.875 14.25,12 C14.25,10.125 13.125,9 12,9 Z");
                eyeIcon.Fill = Brushes.White;
                isPasswordVisible = false;
                passwordHideTimer.Stop();
            }
            else
            {
                // Показываем пароль и показываем закрытый глаз
                PasswordBox.Visibility = Visibility.Hidden;
                PasswordTextBox.Visibility = Visibility.Visible;
                PasswordTextBox.Text = PasswordBox.Password;
                eyeIcon.Data = Geometry.Parse("M12,4.5 C7.5,4.5 3.75,7.5 2.25,12 C3.75,16.5 7.5,19.5 12,19.5 C16.5,19.5 20.25,16.5 21.75,12 C20.25,7.5 16.5,4.5 12,4.5 Z M12,16.5 C9.75,16.5 7.5,14.25 7.5,12 C7.5,9.75 9.75,7.5 12,7.5 C14.25,7.5 16.5,9.75 16.5,12 C16.5,14.25 14.25,16.5 12,16.5 Z M10,12 L14,12 M12,9 V15");
                eyeIcon.Fill = Brushes.White;
                isPasswordVisible = true;
                passwordHideTimer.Start();
            }
        }

        private void PasswordHideTimer_Tick(object sender, EventArgs e)
        {
            if (isPasswordVisible)
            {
                // Автоматически скрываем пароль и возвращаем открытый глаз
                PasswordBox.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Hidden;
                PasswordBox.Password = PasswordTextBox.Text;
                eyeIcon.Data = Geometry.Parse("M12,4.5 C7.5,4.5 3.75,7.5 2.25,12 C3.75,16.5 7.5,19.5 12,19.5 C16.5,19.5 20.25,16.5 21.75,12 C20.25,7.5 16.5,4.5 12,4.5 Z M12,16.5 C9.75,16.5 7.5,14.25 7.5,12 C7.5,9.75 9.75,7.5 12,7.5 C14.25,7.5 16.5,9.75 16.5,12 C16.5,14.25 14.25,16.5 12,16.5 Z M12,9 C10.875,9 9.75,10.125 9.75,12 C9.75,13.875 10.875,15 12,15 C13.125,15 14.25,13.875 14.25,12 C14.25,10.125 13.125,9 12,9 Z");
                eyeIcon.Fill = Brushes.White;
                isPasswordVisible = false;
                passwordHideTimer.Stop();
            }
        }

        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = isPasswordVisible ? PasswordTextBox.Text : PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT PasswordHash FROM Users WHERE Email = @Email", conn))
                    {
                        cmd.Parameters.AddWithValue("Email", login);
                        var result = cmd.ExecuteScalar();

                        if (result != null && VerifyPassword(password, result.ToString()))
                        {
                            var reservationWindow = new ReservationPage(); // Или ReservationPage как окно
                            reservationWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Sign_Click(object sender, RoutedEventArgs e)
        {
            var signWindow = new SignWindow();
            signWindow.Show();
            this.Close();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                var builder = new System.Text.StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            string hash = HashPassword(password);
            return hash == storedHash;
        }
    }
}