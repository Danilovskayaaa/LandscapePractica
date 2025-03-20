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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;
using Landscape.Model;
using System.Collections.ObjectModel;

namespace Landscape.Pages
{
    /// <summary>
    /// Логика взаимодействия для Plants.xaml
    /// </summary>
    public partial class PlantsPage : Page
    {
        public ObservableCollection<Plants> Plantss { get; set; }
        public PlantsPage()
        {
            InitializeComponent();
            DataContext = this;
            Plantss = new ObservableCollection<Plants>();
            LoadPlants();

        }
        private void UpdateList_Click(object sender, RoutedEventArgs e)
        {
            PlantsPage newAdminPage = new PlantsPage();
            NavigationService.Navigate(newAdminPage);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddPlantWindow addSotr = new AddPlantWindow();
            bool? result = addSotr.ShowDialog();

            if (result == true)
            {
                LoadPlants();
            }
        }
        private void PlantsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LViewPlants.SelectedItem is Plants selectedPlant)
            {
                EditPlantWindow editWindow = new EditPlantWindow(selectedPlant);
                if (editWindow.ShowDialog() == true)
                {
                    
                    LoadPlants(); 
                }
            }
        }

        private void LoadPlants()
        {
            try
            {
                string connectionString = "Host=localhost;Database=LandscapeDesign;Username=postgres;Password=postgres;Persist Security Info=True";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    var query = @"
SELECT p.*, pt.""PlantType""
FROM ""Plants"" p
LEFT JOIN ""PlantTypes"" pt ON p.""IDPlantType"" = pt.""IDPlantType""";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var plant = new Plants
                                {
                                    IDPlant = reader.GetInt32(reader.GetOrdinal("IDPlant")),
                                    IDPlantType = reader.IsDBNull(reader.GetOrdinal("IDPlantType")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IDPlantType")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    PricePerUnit = reader.GetDecimal(reader.GetOrdinal("PricePerUnit")),
                                    GrowthConditions = reader.IsDBNull(reader.GetOrdinal("GrowthConditions")) ? string.Empty : reader.GetString(reader.GetOrdinal("GrowthConditions")),
                                    PlantTypes = new PlantTypes
                                    {
                                        PlantType = reader.IsDBNull(reader.GetOrdinal("PlantType")) ? "Неизвестный тип" : reader.GetString(reader.GetOrdinal("PlantType"))
                                    }
                                };

                                Plantss.Add(plant); 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке растений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
