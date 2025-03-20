using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using Landscape.Model;
using Npgsql;

namespace Landscape.Pages
{
    public partial class EditWorkerWindow : Window
    {
        private Workers _worker;

        public EditWorkerWindow(Workers worker)
        {
            InitializeComponent();

            _worker = worker;
            FirstNameTextBox.Text = _worker.FirstName;
            SecondNameTextBox.Text = _worker.SecondName;
            LastNameTextBox.Text = _worker.LastName;
            PhoneNumberTextBox.Text = _worker.PhoneNumber;
            EmailTextBox.Text = _worker.Email;
            AddressTextBox.Text = _worker.Address;
            BirthdayDatePicker.SelectedDate = _worker.BirthdayDate;
            PassportSeriaTextBox.Text = _worker.PassportSeria;
            PassportNumberTextBox.Text = _worker.PassportNumber;
            ExperienceYearsTextBox.Text = _worker.ExperienceYears?.ToString();
            LoadRoles();
        }
        private void LoadRoles()
        {
            try
            {
                string connectionString = "Host=localhost;Database=LandscapeDesign;Username=postgres;Password=postgres;Persist Security Info=True";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT \"IDRole\", \"RoleName\" FROM \"Roles\"";
                    using (var command = new NpgsqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        List<Roles> roles = new List<Roles>();

                        while (reader.Read())
                        {
                            roles.Add(new Roles
                            {
                                IDRole = reader.GetInt32(0),
                                RoleName = reader.GetString(1)
                            });
                        }

                        RoleComboBox.ItemsSource = roles;
                        RoleComboBox.DisplayMemberPath = "RoleName";
                        RoleComboBox.SelectedValuePath = "IDRole";

                        RoleComboBox.SelectedValue = _worker.IDRole;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке ролей: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connectionString = "Host=localhost;Database=LandscapeDesign;Username=postgres;Password=postgres;Persist Security Info=True";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                UPDATE ""Workers"" SET 
                    ""FirstName"" = @FirstName,
                    ""SecondName"" = @SecondName,
                    ""LastName"" = @LastName,
                    ""PhoneNumber"" = @PhoneNumber,
                    ""Email"" = @Email,
                    ""Address"" = @Address,
                    ""BirthdayDate"" = @BirthdayDate,
                    ""PassportSeria"" = @PassportSeria,
                    ""PassportNumber"" = @PassportNumber,
                    ""IDRole"" = @IDRole,
                    ""ExperienceYears"" = @ExperienceYears
                WHERE ""IDWorkers"" = @IDWorkers";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", FirstNameTextBox.Text ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@SecondName", SecondNameTextBox.Text ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@LastName", LastNameTextBox.Text ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@PhoneNumber", PhoneNumberTextBox.Text ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Email", EmailTextBox.Text ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Address", AddressTextBox.Text ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@BirthdayDate", BirthdayDatePicker.SelectedDate ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@PassportSeria", PassportSeriaTextBox.Text ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@PassportNumber", PassportNumberTextBox.Text ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IDRole", RoleComboBox.SelectedValue ?? (object)DBNull.Value);

                        short? experienceYears = short.TryParse(ExperienceYearsTextBox.Text, out var result) ? result : (short?)null;
                        command.Parameters.AddWithValue("@ExperienceYears", experienceYears.HasValue ? (object)experienceYears.Value : DBNull.Value);

                        command.Parameters.AddWithValue("@IDWorkers", _worker.IDWorkers);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Сотрудник успешно обновлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connectionString = "Host=localhost;Database=LandscapeDesign;Username=postgres;Password=postgres;Persist Security Info=True";
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM \"Workers\" WHERE \"IDWorkers\" = @IDWorkers";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IDWorkers", _worker.IDWorkers);
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Сотрудник успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении сотрудника: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
