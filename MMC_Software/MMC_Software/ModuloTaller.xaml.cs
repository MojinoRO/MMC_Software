using System.Windows;
using System.Windows.Controls;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para ModuloTaller.xaml
    /// </summary>
    public partial class ModuloTaller : Window
    {
        public ModuloTaller()
        {
            InitializeComponent();
        }

        private void MenuItemTipoVehiculo_Click(object sender, RoutedEventArgs e)
        {
            var UserControl = new VentanaCrearTipoVehiculos();
            UserControl.HorizontalAlignment = HorizontalAlignment.Stretch;
            UserControl.VerticalAlignment = VerticalAlignment.Stretch;

            var tabcontrol = new TabItem()
            {
                Header = "TIPOS DE VEHICULOS",
                Content = UserControl
            };

            tabTaller.Items.Add(tabcontrol);
            tabTaller.SelectedItem = tabcontrol;
        }

        private void MenuItemTerceros_Click(object sender, RoutedEventArgs e)
        {
            var uc = new VentanaTerceros();
            uc.HorizontalAlignment = HorizontalAlignment.Stretch;
            uc.VerticalAlignment = VerticalAlignment.Stretch;

            var tab = new TabItem
            {
                Header = "Creacion de Terceros",
                Content = uc
            };

            tabTaller.Items.Add(tab);
            tabTaller.SelectedItem = tab;
        }

        private void MenuItemCrearTecnicos_Click(object sender, RoutedEventArgs e)
        {
            var usercontrol = new VentanaCrearTecnicos();
            usercontrol.HorizontalAlignment = HorizontalAlignment.Stretch;
            usercontrol.VerticalAlignment = VerticalAlignment.Stretch;

            var tab = new TabItem
            {
                Header = "Creacion de Tecnicos",
                Content = usercontrol
            };

            tabTaller.Items.Add(tab);
            tabTaller.SelectedItem = tab;
        }

        private void MenuItemCrearVehiculos_Click(object sender, RoutedEventArgs e)
        {
            var Usercontrol = new VentanaCrearVehiculos();
            Usercontrol.HorizontalAlignment = HorizontalAlignment.Stretch;
            Usercontrol.VerticalAlignment = VerticalAlignment.Stretch;

            var tab = new TabItem
            {
                Header = "Creacion de Vehiculos",
                Content = Usercontrol
            };

            tabTaller.Items.Add(tab);
            tabTaller.SelectedItem = tab;
        }
    }
}
