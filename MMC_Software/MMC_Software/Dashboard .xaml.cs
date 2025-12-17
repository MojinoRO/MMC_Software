using System.ComponentModel; // Para CancelEventArgs
using System.Windows;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        private ModuloInventario ventanaInventario; // Guarda la ventana abierta
        private ModuloTaller VentanaTaller;

        public Dashboard()
        {
            InitializeComponent();
            this.Closing += Dashboard_Closing; // Manejamos el cierre
        }

        private void ButtonClick_Inventarios(object sender, RoutedEventArgs e)
        {
            if (ventanaInventario == null || !ventanaInventario.IsLoaded)
            {
                ventanaInventario = new ModuloInventario();
                ventanaInventario.Closed += (s, args) => ventanaInventario = null; // Limpia referencia al cerrar
                ventanaInventario.Show();
            }
            else
            {
                ventanaInventario.Activate();
            }
        }

        private void Dashboard_Closing(object sender, CancelEventArgs e)
        {
            // Si hay ventanaInventario abierta, no dejamos cerrar el Dashboard
            if (ventanaInventario != null && ventanaInventario.IsLoaded || VentanaTaller != null && VentanaTaller.IsLoaded)
            {
                e.Cancel = true;
                MessageBox.Show("Debe cerrar todas las ventanas abiertas antes de salir.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                ventanaInventario.Activate();
            }
        }

        private void ButtonAdministra_Click(object sender, RoutedEventArgs e)
        {
            ModuloAdministracion ventanaAdmin = new ModuloAdministracion();
            ventanaAdmin.Show();
        }

        private void ButtonSalir_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult pregunta = MessageBox.Show("¿Estas Seguro De Salir De La Aplicacion?", "MENSAJE", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (pregunta == MessageBoxResult.Yes)
            {
                this.Close(); // Ejecuta el evento Closing y valida si hay ventanas abiertas
            }
        }

        private void ButtonPuntoVenta_Click(object sender, RoutedEventArgs e)
        {
            var UsuarioID = RepositorioIngresoApp.UsuariosID.UsuarioID;
            VentanaAbrePos ventana = new VentanaAbrePos(UsuarioID);
            ventana.ShowDialog();

        }

        private void ButtonTaller_Click(object sender, RoutedEventArgs e)
        {
            if (VentanaTaller == null || !VentanaTaller.IsLoaded)
            {
                VentanaTaller = new ModuloTaller();
                VentanaTaller.Closed += (s, args) => VentanaTaller = null; // Limpia referencia al cerrar
                VentanaTaller.Show();
            }
            else
            {
                VentanaTaller.Activate();
            }
        }
    }
}
