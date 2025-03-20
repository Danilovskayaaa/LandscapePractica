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
    public partial class EditPlantWindow : Window
    {
        public Plants CurrentPlant { get; set; }

        public EditPlantWindow(Plants plant)
        {
            InitializeComponent();
            CurrentPlant = plant;

            NameTextBox.Text = plant.Name;
            PricePerUnitTextBox.Text = plant.PricePerUnit.ToString();
            GrowthConditionsTextBox.Text = plant.GrowthConditions;

            PlantTypeComboBox.ItemsSource = GetPlantTypes(); 
            PlantTypeComboBox.SelectedItem = plant.PlantTypes.PlantType;
        }

        private List<string> GetPlantTypes()
        {
            List<string> plantTypes = new List<string>();
            string connectionString = "Host=localhost;Database=LandscapeDesign;Username=postgres;Password=postgres;Persist Security Info=True";

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT ""PlantType"" FROM ""PlantTypes""";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string plantType = reader.GetString(reader.GetOrdinal("PlantType"));
                                plantTypes.Add(plantType);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке типов растений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return plantTypes;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
           
                CurrentPlant.Name = NameTextBox.Text;
                CurrentPlant.PricePerUnit = decimal.Parse(PricePerUnitTextBox.Text);
                CurrentPlant.GrowthConditions = GrowthConditionsTextBox.Text;
                CurrentPlant.PlantTypes.PlantType = PlantTypeComboBox.SelectedItem.ToString();

                SavePlant(CurrentPlant);

                MessageBox.Show("Растение успешно сохранено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении растения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
             
                DeletePlant(CurrentPlant.IDPlant);

                MessageBox.Show("Растение успешно удалено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении растения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SavePlant(Plants plant)
        {
            string connectionString = "Host=localhost;Database=LandscapeDesign;Username=postgres;Password=postgres;Persist Security Info=True";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
UPDATE ""Plants""
SET ""Name"" = @Name, ""PricePerUnit"" = @PricePerUnit, ""GrowthConditions"" = @GrowthConditions, ""IDPlantType"" = @IDPlantType
WHERE ""IDPlant"" = @IDPlant";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IDPlant", plant.IDPlant);
                    command.Parameters.AddWithValue("@Name", plant.Name);
                    command.Parameters.AddWithValue("@PricePerUnit", plant.PricePerUnit);
                    command.Parameters.AddWithValue("@GrowthConditions", plant.GrowthConditions);
                    command.Parameters.AddWithValue("@IDPlantType", plant.IDPlantType ?? (object)DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }
        }

        private void DeletePlant(int plantId)
        {
            string connectionString = "Host=localhost;Database=LandscapeDesign;Username=postgres;Password=postgres;Persist Security Info=True";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string query = @"DELETE FROM ""Plants"" WHERE ""IDPlant"" = @IDPlant";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IDPlant", plantId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
