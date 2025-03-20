using System;
using System.Collections.Generic;
using System.IO;
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
 
using Npgsql;
using Landscape.Model;

namespace Landscape.Pages
{
    /// <summary>
    /// Логика взаимодействия для Auto.xaml
    /// </summary>
    public partial class Auto : Page
    {
        private readonly string connectionString;
        public Auto()
        {
            InitializeComponent();
            connectionString = "Host=localhost;Database=LandscapeDesign;Username=postgres;Password=postgres;Persist Security Info=True";

        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            string login = txtbLogin.Text.Trim();
            string password = pswbPassword.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT e.""IDWorkers"", e.""IDRole"", re.""RoleName"" 
                        FROM ""Workers"" e 
                        INNER JOIN ""Roles"" re ON e.""IDRole"" = re.""IDRole"" 
                        WHERE e.""Login"" = @Login AND e.""Password"" = @Password";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Login", login);
                        command.Parameters.AddWithValue("@Password", password);

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string roleEmployee = reader["RoleName"].ToString();
                                int idWorker = reader.GetInt32(reader.GetOrdinal("IDWorkers"));
                                int? idRole = reader.IsDBNull(reader.GetOrdinal("IDRole")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IDRole"));

                                Workers polzov = new Workers
                                {
                                    IDWorkers = idWorker,
                                    IDRole = idRole,
                                    Roles = new Roles { RoleName = roleEmployee }
                                };

                                MessageBox.Show("Вы вошли под: " + roleEmployee);
                                LoadForm(roleEmployee, polzov);
                            }
                            else
                            {
                                MessageBox.Show("Пользователь с указанным логином и паролем не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadForm(string roleName, Workers polzov)
        {
            switch (roleName)
            {
                case "Администратор":
                    NavigationService.Navigate(new Admin());
                    break;
                case "Дизайнер":
                    NavigationService.Navigate(new PlantsPage());
                    break;
                case "Рабочий":
                    NavigationService.Navigate(new EquipmentPage());
                    break;
                default:
                    MessageBox.Show("Неверная роль пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
            }
        }
    }

}
