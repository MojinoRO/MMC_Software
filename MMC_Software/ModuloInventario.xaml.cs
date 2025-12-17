using CrystalDecisions.CrystalReports.Engine;
using System.ComponentModel;
using System.Net.Cache;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using static MMC_Software.RepositorioIngresoApp;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para ModuloInventario.xaml
    /// </summary>
    public partial class ModuloInventario : Window
    {
        string _ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
        public ModuloInventario()
        {
            InitializeComponent();

            this.Closing += ModuloInventario_Closing;
        }

        private void ModuloInventario_Closing(object sender, CancelEventArgs e)
        {
            // Si hay pestañas abiertas en el TabControl
            if (tabInventarios.Items.Count > 0)
            {
                // Cancelamos el cierre
                e.Cancel = true;

                // Opcional: mostrar mensaje al usuario
                MessageBox.Show("No puede cerrar el módulo mientras haya pestañas abiertas.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void MenuItemCrearArticulos_Click(object sender, RoutedEventArgs e)
        {
            var uc = new VentanaArticulos();
            uc.HorizontalAlignment = HorizontalAlignment.Stretch;
            uc.VerticalAlignment = VerticalAlignment.Stretch;

            var tab = new TabItem
            {
                Header = "Creación de Artículos",
                Content = uc
            };

            tabInventarios.Items.Add(tab);
            tabInventarios.SelectedItem = tab;
        }

        private void MenuItemCrearTerceros_Click(object sender, RoutedEventArgs e)
        {
            var uc = new VentanaTerceros();
            uc.HorizontalAlignment = HorizontalAlignment.Stretch;
            uc.VerticalAlignment = VerticalAlignment.Stretch;

            var tab = new TabItem
            {
                Header = "Creacion de Terceros",
                Content = uc
            };

            tabInventarios.Items.Add(tab);
            tabInventarios.SelectedItem = tab;
        }

        private void MenuItemAjusteConsumidor_Click(object sender, RoutedEventArgs e)
        {
            var usercontrol = new VentanaConfigurarConsumidorFinal();
            usercontrol.HorizontalAlignment = HorizontalAlignment.Stretch;
            usercontrol.VerticalAlignment = VerticalAlignment.Stretch;
            var tab = new TabItem
            {
                Header = "Configuracion Consumidor Final",
                Content = usercontrol
            };
            tabInventarios.Items.Add(tab);
            tabInventarios.SelectedItem = tab;
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            foreach (var items in ExpanderContainer.Children)
            {
                if (items is Expander exp && exp != sender) exp.IsExpanded = false;
            }
        }

        private void ButtonRegistroCompras_Click(object sender, RoutedEventArgs e)
        {
            var uc = new VentanaRegistroCompras(DatosPredeterminadosUsuarios.VendedorID, DatosPredeterminadosUsuarios.AlmacenID,false);
            uc.HorizontalAlignment = HorizontalAlignment.Stretch;
            uc.VerticalAlignment = VerticalAlignment.Stretch;
            var tab = new TabItem
            {
                Header = "Registro De Compras",
                Content = uc
            };

            tabInventarios.Items.Add(tab);
            tabInventarios.SelectedItem = tab;
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            BuscadorDocumentos Ventana = new BuscadorDocumentos(1);
            bool ?Buscar= Ventana.ShowDialog();
            if(Buscar == true && Ventana.docto.MovimientoID!= 0)
            {
                int TipoFormulario = Ventana.docto.TipoDocumntoID;
                switch (TipoFormulario)
                {
                    case 1:
                        VentanaRegistroCompras ven = new VentanaRegistroCompras(DatosPredeterminadosUsuarios.VendedorID, DatosPredeterminadosUsuarios.AlmacenID,true,Ventana.docto.MovimientoID);
                        ven.HorizontalAlignment = HorizontalAlignment.Stretch;
                        ven.VerticalAlignment = VerticalAlignment.Stretch;

                        var tab = new TabItem
                        {
                            Header = "Registro De Compras",
                            Content = ven
                        };

                        tabInventarios.Items.Add(tab);
                        tabInventarios.SelectedItem = tab;
                        break;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var uc = new VentanaRegistroVentas(DatosPredeterminadosUsuarios.VendedorID, DatosPredeterminadosUsuarios.AlmacenID);
            uc.HorizontalAlignment = HorizontalAlignment.Stretch;
            uc.VerticalAlignment = VerticalAlignment.Stretch;
            
            var tab = new TabItem
            {
                Header = "Registro De Ventas",
                Content = uc
            };
            tabInventarios.Items.Add(tab);
            tabInventarios.SelectedItem = tab;
        }

        private void btnVentas_Click(object sender, RoutedEventArgs e)
        {
            var uc = new VentanaRegistroVentas(DatosPredeterminadosUsuarios.VendedorID, DatosPredeterminadosUsuarios.AlmacenID);
            uc.HorizontalAlignment = HorizontalAlignment.Stretch;
            uc.VerticalAlignment = VerticalAlignment.Stretch;

            var tab = new TabItem
            {
                Header = "Registro De Ventas",
                Content = uc
            };
            tabInventarios.Items.Add(tab);
            tabInventarios.SelectedItem = tab;
        }

        private void btnCompras_Click(object sender, RoutedEventArgs e)
        {
            var uc = new VentanaRegistroCompras(DatosPredeterminadosUsuarios.VendedorID, DatosPredeterminadosUsuarios.AlmacenID, false);
            uc.HorizontalAlignment = HorizontalAlignment.Stretch;
            uc.VerticalAlignment = VerticalAlignment.Stretch;

            var Tab = new TabItem
            {
                Header = "Registro De Compras",
                Content = uc
            };

            tabInventarios.Items.Add(Tab);
            tabInventarios.SelectedItem = Tab;
        }
    }
}
