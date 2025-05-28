using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Npgsql;

namespace ClassSchedule1
{
    /// <summary>
    /// Логика взаимодействия для SignWindow.xaml
    /// </summary>
    public partial class SignWindow : Window
    {
        private string connectionString = "Server=localhost;Database=ClassSchedule;User id=postgres;Password=postgres;";
        public SignWindow()
        {
            InitializeComponent();
        }
        private void Button_Sign_Click(object sender, RoutedEventArgs e)
        {
            string email = SignTextBox.Text;
            string password = PasswordTextBox.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите почту и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("INSERT INTO Users (Email, PasswordHash) VALUES (@Email, @PasswordHash)", conn))
                    {
                        cmd.Parameters.AddWithValue("Email", email);
                        cmd.Parameters.AddWithValue("PasswordHash", HashPassword(password));
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Регистрация успешна!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            catch (NpgsqlException ex) when (ex.SqlState == "23505") // Unique violation
            {
                MessageBox.Show("Этот email уже зарегистрирован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private string HashPassword(string password)
        {
            using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}