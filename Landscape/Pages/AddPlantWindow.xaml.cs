using System;
using System.Collections.Generic;
using System.Windows;
using Npgsql;
using Landscape.Model;

namespace Landscape.Pages
{
    public partial class AddPlantWindow : Window
    {
        public AddPlantWindow()
        {
            InitializeComponent();
            PlantTypeComboBox.ItemsSource = GetPlantTypes();
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
                                plantTypes.Add(reader.GetString(0));
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
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(PricePerUnitTextBox.Text) || PlantTypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(PricePerUnitTextBox.Text, out decimal price))
            {
                MessageBox.Show("Введите корректную цену!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string name = NameTextBox.Text;
            string growthConditions = GrowthConditionsTextBox.Text;
            string plantType = PlantTypeComboBox.SelectedItem.ToString();

            AddPlantToDatabase(name, price, growthConditions, plantType);
        }
        private void AddPlantToDatabase(string name, decimal price, string growthConditions, string plantType)
        {
            string connectionString = "Host=localhost;Database=LandscapeDesign;Username=postgres;Password=postgres;Persist Security Info=True";

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string getTypeIdQuery = @"SELECT ""IDPlantType"" FROM ""PlantTypes"" WHERE ""PlantType"" = @PlantType";
                    int? plantTypeId = null;

                    using (var getTypeCommand = new NpgsqlCommand(getTypeIdQuery, connection))
                    {
                        getTypeCommand.Parameters.AddWithValue("@PlantType", plantType);
                        var result = getTypeCommand.ExecuteScalar();
                        if (result != null)
                            plantTypeId = Convert.ToInt32(result);
                    }

                    if (plantTypeId == null)
                    {
                        MessageBox.Show("Ошибка: выбранный тип растения не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    string insertQuery = @"
INSERT INTO ""Plants"" (""Name"", ""PricePerUnit"", ""GrowthConditions"", ""IDPlantType"") 
VALUES (@Name, @PricePerUnit, @GrowthConditions, @IDPlantType)";

                    using (var command = new NpgsqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@PricePerUnit", price);
                        command.Parameters.AddWithValue("@GrowthConditions", string.IsNullOrWhiteSpace(growthConditions) ? (object)DBNull.Value : growthConditions);
                        command.Parameters.AddWithValue("@IDPlantType", plantTypeId);

                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Растение успешно добавлено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении растения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
