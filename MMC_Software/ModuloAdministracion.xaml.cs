using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace MMC_Software
{
    public partial class ModuloAdministracion : Window
    {
        SqlConnection miConexionSql;

        public ModuloAdministracion()
        {
            InitializeComponent();

            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
            miConexionSql = new SqlConnection(ConexionSql);
        }


        private void MenuDatosEmpresa_Click(object sender, RoutedEventArgs e)
        {
            var uc = new DatosEmpresaControl();
            uc.HorizontalAlignment = HorizontalAlignment.Stretch;
            uc.VerticalAlignment = VerticalAlignment.Stretch;

            var tab = new TabItem
            {
                Header = "Datos Empresa",
                Content = uc
            };

            tabAdministracion.Items.Add(tab);
            tabAdministracion.SelectedItem = tab;
        }

        private void ButtonAbreSCC_Click(object sender, RoutedEventArgs e)
        {
            var uc = new VentanaCentros_SubCentro();
            uc.HorizontalAlignment = HorizontalAlignment.Stretch;
            uc.VerticalAlignment = VerticalAlignment.Stretch;

            var tab = new TabItem
            {
                Header = "Centro Costo / Sub Centro Costo",
                Content = uc
            };

            tabAdministracion.Items.Add(tab);
            tabAdministracion.SelectedItem = tab;
        }

        private void ButtonDepCiudad_Click(object sender, RoutedEventArgs e)
        {
            var uc = new VentanaPaisDepCiu();
            uc.HorizontalAlignment = HorizontalAlignment.Stretch;
            uc.VerticalAlignment = VerticalAlignment.Stretch;

            var tab = new TabItem
            {
                Header = "Departamentos & Ciudades",
                Content = uc
            };

            tabAdministracion.Items.Add(tab);
            tabAdministracion.SelectedItem = tab;
        }

        private void ButtonCrearFormaPago_Click(object sender, RoutedEventArgs e)
        {
            var uc = new VentanaCrearFormasPago();
            uc.HorizontalAlignment = HorizontalAlignment.Stretch;
            uc.VerticalAlignment = VerticalAlignment.Stretch;

            var tab = new TabItem
            {
                Header = "Formas Pago",
                Content = uc
            };

            tabAdministracion.Items.Add(tab);
            tabAdministracion.SelectedItem = tab;
        }

        private void ButtonCrearDocto_Click(object sender, RoutedEventArgs e)
        {
            var uc = new VentanaCrearDocumentos();
            uc.HorizontalAlignment = HorizontalAlignment.Stretch;
            uc.VerticalAlignment = VerticalAlignment.Stretch;

            var Tab = new TabItem
            {
                Header = "Crear Documentos",
                Content = uc
            };

            tabAdministracion.Items.Add(Tab);
            tabAdministracion.SelectedItem = Tab;
        }

        private void ButtonCrearVendedores_Click(object sender, RoutedEventArgs e)
        {
            var uc = new VentanaCrearVendedores();
            uc.HorizontalAlignment = HorizontalAlignment.Stretch;
            uc.VerticalAlignment = VerticalAlignment.Stretch;

            var tab = new TabItem
            {
                Header = "Crear Vendedores",
                Content = uc
            };

            tabAdministracion.Items.Add(tab);
            tabAdministracion.SelectedItem = tab;
        }

        private void ButtonCrearUsuarios_Click(object sender, RoutedEventArgs e)
        {
            VentanaClave Ventana = new VentanaClave(1, 0, 2);
            bool? Clave = Ventana.ShowDialog();
            if (Clave == true)
            {
                var uc = new VentanaCrearUsuarios();
                uc.HorizontalAlignment = HorizontalAlignment.Stretch;
                uc.VerticalAlignment = VerticalAlignment.Stretch;
                var tab = new TabItem
                {
                    Header = "Crear Usuarios",
                    Content = uc
                };

                tabAdministracion.Items.Add(tab);
                tabAdministracion.SelectedItem = tab;
            }
        }

        private void ButtonCrearClaves_Click(object sender, RoutedEventArgs e)
        {
            VentanaClave Ventana = new VentanaClave(1, 0, 3);
            bool? Clave = Ventana.ShowDialog();
            if (Clave == true)
            {
                var uc = new VentanaClavesAcceso();
                uc.HorizontalAlignment = HorizontalAlignment.Stretch;
                uc.VerticalAlignment = VerticalAlignment.Stretch;

                var Tab = new TabItem
                {
                    Header = "Sistema De Claves",
                    Content = uc
                };

                tabAdministracion.Items.Add(Tab);
                tabAdministracion.SelectedItem = Tab;
            }
        }

        private void ButtonCrearAlmacen_Click(object sender, RoutedEventArgs e)
        {
            var uc = new VentanaCreacionAlmacen();
            uc.HorizontalAlignment = HorizontalAlignment.Stretch;
            uc.VerticalAlignment = VerticalAlignment.Stretch;

            var Tab = new TabItem
            {
                Header = "Ventana Crear Almacen",
                Content = uc
            };

            tabAdministracion.Items.Add(Tab);
            tabAdministracion.SelectedItem = Tab;
        }

        private void ButtonCrearCategorias_Click(object sender, RoutedEventArgs e)
        {
            var uc = new VentanaCategoriasProductos();
            uc.HorizontalAlignment = HorizontalAlignment.Stretch;
            uc.VerticalAlignment = VerticalAlignment.Stretch;

            var Tab = new TabItem
            {
                Header = "Crear Categorias",
                Content = uc
            };

            tabAdministracion.Items.Add(Tab);
            tabAdministracion.SelectedItem = Tab;
        }

        private void ButtonCrearSubCat_Click(object sender, RoutedEventArgs e)
        {
            var uc = new VentanaSubCategorias();
            uc.HorizontalAlignment = HorizontalAlignment.Stretch;
            uc.VerticalAlignment = VerticalAlignment.Stretch;

            var tab = new TabItem
            {
                Header = "Crear Sub Categorias",
                Content = uc
            };

            tabAdministracion.Items.Add(tab);
            tabAdministracion.SelectedItem = tab;
        }

        private void ButtonCrearMarcas_Click(object sender, RoutedEventArgs e)
        {
            var uc = new VentanaMarcas();
            uc.HorizontalAlignment = HorizontalAlignment.Stretch;
            uc.VerticalAlignment = VerticalAlignment.Stretch;

            var Tab = new TabItem
            {
                Header = "Crear Marcas",
                Content = uc
            };

            tabAdministracion.Items.Add(Tab);
            tabAdministracion.SelectedItem = Tab;
        }

        private void ButtonCrearListadoPrecios_Click(object sender, RoutedEventArgs e)
        {
            var uc = new VentanaListaPrecios();
            uc.HorizontalAlignment = HorizontalAlignment.Stretch;
            uc.VerticalAlignment = VerticalAlignment.Stretch;

            var Tab = new TabItem
            {
                Header = "Crear Marcas",
                Content = uc
            };

            tabAdministracion.Items.Add(Tab);
            tabAdministracion.SelectedItem = Tab;
        }

        private void ButtonCrearCuentasPuc_Click(object sender, RoutedEventArgs e)
        {
            var uc = new VentanaCuentasPuc();
            uc.HorizontalAlignment = HorizontalAlignment.Stretch;
            uc.VerticalAlignment = VerticalAlignment.Stretch;

            var Tab = new TabItem
            {
                Header = "Crear Cuentas Puc",
                Content = uc
            };

            tabAdministracion.Items.Add(Tab);
            tabAdministracion.SelectedItem = Tab;
        }
    }
}