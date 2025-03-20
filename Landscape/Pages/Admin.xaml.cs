using Npgsql;
using Landscape.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Landscape.Pages
{
    /// <summary>
    /// Логика взаимодействия для Admin.xaml
    /// </summary>
    public partial class Admin : Page
    {
        public ObservableCollection<Workers> Workerss { get; set; }
        public Admin()
        {
            InitializeComponent();
            DataContext = this;
            Workerss = new ObservableCollection<Workers>();
            LoadEmployee();
        }

        private void LoadEmployee()
        {
            try
            {
                string connectionString = "Host=localhost;Database=LandscapeDesign;Username=postgres;Password=postgres;Persist Security Info=True";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    var query = @"
                SELECT w.*, r.""RoleName"" 
                FROM ""Workers"" w
                LEFT JOIN ""Roles"" r ON w.""IDRole"" = r.""IDRole""";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var worker = new Workers
                                {
                                    IDWorkers = reader.GetInt32(reader.GetOrdinal("IDWorkers")),
                                    IDRole = reader.IsDBNull(reader.GetOrdinal("IDRole")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IDRole")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    SecondName = reader.IsDBNull(reader.GetOrdinal("SecondName")) ? string.Empty : reader.GetString(reader.GetOrdinal("SecondName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString(reader.GetOrdinal("Email")),
                                    Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? string.Empty : reader.GetString(reader.GetOrdinal("Address")),
                                    BirthdayDate = reader.IsDBNull(reader.GetOrdinal("BirthdayDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("BirthdayDate")),
                                    PassportSeria = reader.IsDBNull(reader.GetOrdinal("PassportSeria")) ? string.Empty : reader.GetString(reader.GetOrdinal("PassportSeria")),
                                    PassportNumber = reader.IsDBNull(reader.GetOrdinal("PassportNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("PassportNumber")),
                                    ExperienceYears = reader.IsDBNull(reader.GetOrdinal("ExperienceYears")) ? (short?)null : reader.GetInt16(reader.GetOrdinal("ExperienceYears")),
                                    Roles = new Roles
                                    {
                                        RoleName = reader.IsDBNull(reader.GetOrdinal("RoleName")) ? "Неизвестная роль" : reader.GetString(reader.GetOrdinal("RoleName"))
                                    }
                                };

                                Console.WriteLine($"Загружен работник: {worker.FirstName} {worker.LastName}, Роль: {worker.Roles.RoleName}");

                                Workerss.Add(worker); 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке сотрудников: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void UpdateList_Click(object sender, RoutedEventArgs e)
        {
            Admin newAdminPage = new Admin();
            NavigationService.Navigate(newAdminPage);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddWorkerWindow addSotr = new AddWorkerWindow();
            bool? result = addSotr.ShowDialog();

            if (result == true)
            {
                LoadEmployee();
            }
        }

        private void LViewProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LViewProduct.SelectedItem is Workers selectedWorker)
            {
              
                EditWorkerWindow editWindow = new EditWorkerWindow(selectedWorker);

                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    LoadEmployee();
                }
            }
        }
    }
    }

