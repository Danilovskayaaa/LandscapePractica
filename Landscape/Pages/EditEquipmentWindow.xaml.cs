using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    /// Логика взаимодействия для EditEquipmentWindow.xaml
    /// </summary>
    public partial class EditEquipmentWindow : Window
    {
        public Equipment CurrentEquipment { get; set; }

        public EditEquipmentWindow(Equipment equipment)
        {
            InitializeComponent();
            CurrentEquipment = equipment;

            NameTextBox.Text = equipment.Name;
            TypeEquipmentComboBox.ItemsSource = GetEquipmentTypes();
            TypeEquipmentComboBox.SelectedItem = equipment.TypeEquipment.TypesEquipment;
            StatusEquipmentComboBox.ItemsSource = GetEquipmentStatuses();
            StatusEquipmentComboBox.SelectedItem = equipment.StatusesEquipment.StatusEquipment;
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentEquipment.Name = NameTextBox.Text;
            SaveEquipment(CurrentEquipment);
            MessageBox.Show("Оборудование успешно сохранено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteEquipment(CurrentEquipment.IDEquipment);
            MessageBox.Show("Оборудование удалено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveEquipment(Equipment equipment)
        {
            string connectionString = "Host=localhost;Database=LandscapeDesign;Username=postgres;Password=postgres;Persist Security Info=True";

            var selectedType = TypeEquipmentComboBox.SelectedItem as string;
            var selectedStatus = StatusEquipmentComboBox.SelectedItem as string;

            int selectedTypeId = GetEquipmentTypeId(selectedType);
            int selectedStatusId = GetEquipmentStatusId(selectedStatus);

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE \"Equipment\" SET \"Name\" = @Name, \"IDTypeEquipment\" = @IDTypeEquipment, \"IDStatusEquipment\" = @IDStatusEquipment WHERE \"IDEquipment\" = @IDEquipment";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IDEquipment", equipment.IDEquipment);
                    command.Parameters.AddWithValue("@Name", equipment.Name);
                    command.Parameters.AddWithValue("@IDTypeEquipment", selectedTypeId);
                    command.Parameters.AddWithValue("@IDStatusEquipment", selectedStatusId);
                    command.ExecuteNonQuery();
                }
            }
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


        private void DeleteEquipment(int equipmentId)
        {
            string connectionString = "Host=localhost;Database=LandscapeDesign;Username=postgres;Password=postgres;Persist Security Info=True";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM \"Equipment\" WHERE \"IDEquipment\" = @IDEquipment";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IDEquipment", equipmentId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
