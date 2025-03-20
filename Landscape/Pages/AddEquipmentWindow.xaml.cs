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
using Landscape.Model;
using Npgsql;

namespace Landscape.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEquipmentWindow.xaml
    /// </summary>
    public partial class AddEquipmentWindow : Window
    {
        public AddEquipmentWindow()
        {
            InitializeComponent();
            TypeEquipmentComboBox.ItemsSource = GetEquipmentTypes();
            StatusEquipmentComboBox.ItemsSource = GetEquipmentStatuses();
        }

        private List<string> GetEquipmentTypes()
        {
            List<string> types = new List<string>();
            string connectionString = "Host=localhost;Database=LandscapeDesign;Username=postgres;Password=postgres;Persist Security Info=True";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT \"TypesEquipment\" FROM \"TypeEquipment\"";
                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        types.Add(reader.GetString(0));
                }
            }
            return types;
        }

        private List<string> GetEquipmentStatuses()
        {
            List<string> statuses = new List<string>();
            string connectionString = "Host=localhost;Database=LandscapeDesign;Username=postgres;Password=postgres;Persist Security Info=True";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT \"StatusEquipment\" FROM \"StatusesEquipment\"";
                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        statuses.Add(reader.GetString(0));
                }
            }
            return statuses;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите название оборудования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Equipment newEquipment = new Equipment
            {
                Name = name
            };

            AddEquipment(newEquipment);
            MessageBox.Show("Оборудование успешно добавлено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }

        private void AddEquipment(Equipment equipment)
        {
            string connectionString = "Host=localhost;Database=LandscapeDesign;Username=postgres;Password=postgres;Persist Security Info=True";
            var selectedType = TypeEquipmentComboBox.SelectedItem as string;
            var selectedStatus = StatusEquipmentComboBox.SelectedItem as string;
            int selectedTypeId = GetEquipmentTypeId(selectedType);
            int selectedStatusId = GetEquipmentStatusId(selectedStatus);

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO \"Equipment\" (\"Name\", \"IDTypeEquipment\", \"IDStatusEquipment\") VALUES (@Name, @IDTypeEquipment, @IDStatusEquipment)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", equipment.Name);
                    command.Parameters.AddWithValue("@IDTypeEquipment", selectedTypeId);
                    command.Parameters.AddWithValue("@IDStatusEquipment", selectedStatusId);
                    command.ExecuteNonQuery();
                }
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        

        private int GetEquipmentTypeId(string typeName)
        {
            string connectionString = "Host=localhost;Database=LandscapeDesign;Username=postgres;Password=postgres;Persist Security Info=True";
            int typeId = 0;

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT \"IDTypeEquipment\" FROM \"TypeEquipment\" WHERE \"TypesEquipment\" = @TypesEquipment";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TypesEquipment", typeName);
                    typeId = (int)command.ExecuteScalar();
                }
            }
            return typeId;
        }

        private int GetEquipmentStatusId(string statusName)
        {
            string connectionString = "Host=localhost;Database=LandscapeDesign;Username=postgres;Password=postgres;Persist Security Info=True";
            int statusId = 0;

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT \"IDStatusEquipment\" FROM \"StatusesEquipment\" WHERE \"StatusEquipment\" = @StatusEquipment";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StatusEquipment", statusName);
                    statusId = (int)command.ExecuteScalar();
                }
            }
            return statusId;
        }

    }
}

