using System;
using System.Data;
using System.Windows;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaTestigoDeVentas.xaml
    /// </summary>
    public partial class VentanaTestigoDeVentas : Window
    {
        public int _VendedorID;
        public int _DocumentoID;
        private DateTime _Fecha;
        public double _ValorRecaudo;
        public string _NombreVendedor;


        public VentanaTestigoDeVentas(int VendedorID, int DocumentoID)
        {
            InitializeComponent();
            _DocumentoID = DocumentoID;
            _VendedorID = VendedorID;
            LlenarCierre();
            dpFechaCierre.SelectedDate = DateTime.Now;
            llenarFecha();
        }

        private void llenarFecha()
        {
            _Fecha = dpFechaCierre.SelectedDate.Value;
        }
        public void LlenarCierre()
        {
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
            DataTable dtVendedor = CacheAplicacion.CacheGlobal.vendedores;
            cmbVendedor.DisplayMemberPath = "INFOVENDEDOR";
            cmbVendedor.SelectedValuePath = "VendedorID";
            cmbVendedor.ItemsSource = dtVendedor.DefaultView;
            DataTable dtDocumento = CacheAplicacion.CacheGlobal.Documento;
            cmbDocumento.DisplayMemberPath = "INFODOCUMENTO";
            cmbDocumento.SelectedValuePath = "DocumentoID";
            cmbDocumento.ItemsSource = dtDocumento.DefaultView;
        }

        private void ButtonAbrirConteo_Click(object sender, RoutedEventArgs e)
        {
            if (cmbVendedor.SelectedValue == null)
            {
                MessageBox.Show("Debe Seleccionar Cajero Para Generar Conteo", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbVendedor.Focus();
                return;
            }
            DateTime Fecha = Convert.ToDateTime(dpFechaCierre.SelectedDate);
            _Fecha = Fecha;
            int VendedorID = Convert.ToInt32(cmbVendedor.SelectedValue);
            _VendedorID = VendedorID;
            VentanaConteoDinero Ventana = new VentanaConteoDinero(_VendedorID, _Fecha);
            Ventana.Owner = this;
            bool? respuesta = Ventana.ShowDialog();
            if (respuesta == true)
            {
                string Signo = "$";
                TxtTotalConteo.Text = Signo + Ventana._TotalEntregado.ToString(System.Globalization.CultureInfo.CurrentCulture);
                _ValorRecaudo = Ventana._TotalEntregado;
            }
        }

        private void ButtonGenerar_Click(object sender, RoutedEventArgs e)
        {
            DateTime Fecha = Convert.ToDateTime(dpFechaCierre.SelectedDate);
            _Fecha = Fecha;
            int VendedorID = Convert.ToInt32(cmbVendedor.SelectedValue);
            _VendedorID = VendedorID;
            VentanaInfoCierre ventana = new VentanaInfoCierre(_VendedorID, _ValorRecaudo, _Fecha);
            ventana.Owner = this;
            ventana.ShowDialog();
        }
    }
}
