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

namespace Landscape.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddWorkerWindow.xaml
    /// </summary>
    public partial class AddWorkerWindow : Window
    {
        private readonly string connectionString = "Host=localhost;Database=LandscapeDesign;Username=postgres;Password=postgres;Persist Security Info=True";

        public AddWorkerWindow()
        {
            InitializeComponent();
            LoadRoles();
        }

        private void LoadRoles()
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT \"IDRole\", \"RoleName\" FROM \"Roles\"";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var role = new { IDRole = reader.GetInt32(reader.GetOrdinal("IDRole")), RoleName = reader.GetString(reader.GetOrdinal("RoleName")) };
                                cbRole.Items.Add(new ComboBoxItem { Content = role.RoleName, Tag = role.IDRole });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке ролей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddWorker_Click(object sender, RoutedEventArgs e)
        {
            string firstName = txtFirstName.Text.Trim();
            string secondName = txtSecondName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string phoneNumber = txtPhoneNumber.Text.Trim();
            string email = txtEmail.Text.Trim();
            string address = txtAddress.Text.Trim();
            DateTime? birthdayDate = dpBirthdayDate.SelectedDate;
            string passportSeria = txtPassportSeria.Text.Trim();
            string passportNumber = txtPassportNumber.Text.Trim();
            short? experienceYears = short.TryParse(txtExperienceYears.Text, out short exp) ? (short?)exp : null;

            if (cbRole.SelectedItem is ComboBoxItem selectedRole)
            {
                int roleId = (int)selectedRole.Tag;

                try
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = @"
                        INSERT INTO ""Workers"" 
                        (""FirstName"", ""SecondName"", ""LastName"", ""PhoneNumber"", ""Email"", ""Address"", ""BirthdayDate"", ""PassportSeria"", ""PassportNumber"", ""ExperienceYears"", ""IDRole"") 
                        VALUES 
                        (@FirstName, @SecondName, @LastName, @PhoneNumber, @Email, @Address, @BirthdayDate, @PassportSeria, @PassportNumber, @ExperienceYears, @IDRole)";

                        using (var command = new NpgsqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@FirstName", firstName);
                            command.Parameters.AddWithValue("@SecondName", secondName);
                            command.Parameters.AddWithValue("@LastName", lastName);
                            command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                            command.Parameters.AddWithValue("@Email", email);
                            command.Parameters.AddWithValue("@Address", address);
                            command.Parameters.AddWithValue("@BirthdayDate", birthdayDate ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PassportSeria", passportSeria);
                            command.Parameters.AddWithValue("@PassportNumber", passportNumber);
                            command.Parameters.AddWithValue("@ExperienceYears", experienceYears ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@IDRole", roleId);

                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Сотрудник добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true; 
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении сотрудника: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите роль для сотрудника.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
