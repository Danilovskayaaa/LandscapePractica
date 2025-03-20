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
using Landscape.Model;
using Npgsql;

namespace Landscape.Pages
{
    /// <summary>
    /// Логика взаимодействия для EquipmentPage.xaml
    /// </summary>
    public partial class EquipmentPage : Page
    {
            public ObservableCollection<Equipment> EquipmentList { get; set; }

            public EquipmentPage()
            {
                InitializeComponent();
                DataContext = this;
                EquipmentList = new ObservableCollection<Equipment>();
                LoadEquipment();
            }

            private void LoadEquipment()
            {
                try
                {
                    string connectionString = "Host=localhost;Database=LandscapeDesign;Username=postgres;Password=postgres;Persist Security Info=True";
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                    var query = @"
                SELECT 
                    e.""IDEquipment"", 
                    e.""Name"", 
                    se.""StatusEquipment"", 
                    te.""TypesEquipment""
                FROM 
                    ""Equipment"" e
                LEFT JOIN 
                    ""StatusesEquipment"" se ON e.""IDStatusEquipment"" = se.""IDStatusEquipment""
                LEFT JOIN 
                    ""TypeEquipment"" te ON e.""IDTypeEquipment"" = te.""IDTypeEquipment""";

                    using (var command = new NpgsqlCommand(query, connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                EquipmentList.Clear();
                                while (reader.Read())
                                {
                                    var equipment = new Equipment
                                    {
                                        IDEquipment = reader.GetInt32(reader.GetOrdinal("IDEquipment")),
                                        Name = reader.GetString(reader.GetOrdinal("Name")),
                                        StatusesEquipment = new StatusesEquipment
                                        {
                                            StatusEquipment = reader.IsDBNull(reader.GetOrdinal("StatusEquipment")) ? "Неизвестный статус" : reader.GetString(reader.GetOrdinal("StatusEquipment"))
                                        },
                                        
                                        TypeEquipment = new TypeEquipment 
                                        { 
                                            TypesEquipment = reader.IsDBNull(reader.GetOrdinal("TypesEquipment")) ? "Неизвестный статус" : reader.GetString(reader.GetOrdinal("TypesEquipment"))
                                        },

                                    };
                                    EquipmentList.Add(equipment);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке оборудования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            private void AddEquipment_Click(object sender, RoutedEventArgs e)
            {
                var addWindow = new AddEquipmentWindow();
                if (addWindow.ShowDialog() == true)
                {
                    LoadEquipment();
                }
            }

            private void UpdateList_Click(object sender, RoutedEventArgs e)
            {
                LoadEquipment();
            }

            private void LViewEquipment_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
            {
                if (LViewEquipment.SelectedItem is Equipment CurrentEquipment)
                {
                    var editWindow = new EditEquipmentWindow(CurrentEquipment);
                    if (editWindow.ShowDialog() == true)
                    {
                        LoadEquipment();
                    }
                }
            }
        }
    }
