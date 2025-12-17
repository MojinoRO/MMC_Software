using System;
using System.Data;
using System.Windows;
using System.Windows.Input;


namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaRetirosCaja.xaml
    /// </summary>
    public partial class VentanaRetirosCaja : Window
    {
        private string _ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
        private int _VendedorID;
        private DateTime _Fecha;
        public VentanaRetirosCaja(int VendedorID)
        {
            InitializeComponent();
            _VendedorID = VendedorID;
            llenarVendedor();
            DpFecha.SelectedDate = DateTime.Now;
            _Fecha = DateTime.Now;
        }

        public void llenarVendedor()
        {
            var Vendedor = new VendedoresRepositorio(_ConexionSql);
            DataTable dt = Vendedor.GenerarInfoVendedorUnitario(_VendedorID);
            int VendedorID = Convert.ToInt32(dt.Rows[0]["VendedorID"]);
            CmbVendedores.DisplayMemberPath = "INFOCOMPLETA";
            CmbVendedores.SelectedValuePath = "VendedorID";
            CmbVendedores.ItemsSource = dt.DefaultView;
            CmbVendedores.SelectedIndex = 0;
        }

        private void TxtValorRetiro_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, "[0-9]+$");
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string ValidaValor = TxtValorRetiro.Text;
            if (string.IsNullOrEmpty(ValidaValor))
            {
                MessageBox.Show("Valor No Puede Ser 0", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Stop);
                TxtValorRetiro.Focus();
                return;
            }
            string Detalle = TxtDetalle.Text;
            if (string.IsNullOrEmpty(Detalle))
            {
                MessageBox.Show("Debe Agregar Detalle Del Retiro", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Stop);
                TxtDetalle.Focus();
                return;
            }
            decimal Valor = Convert.ToDecimal(TxtValorRetiro.Text);
            TimeSpan Hora = DateTime.Now.TimeOfDay;
            DateTime Fecha = _Fecha;
            MessageBoxResult Pregunta = MessageBox.Show("¿Estas Seguro De Generar Retiro?", "MENSAJE", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (Pregunta == MessageBoxResult.Yes)
            {
                var Cierre = new RepositorioCierreCaja(_ConexionSql);
                bool Crear = Cierre.InsertarRetirosCaja(_VendedorID, Valor, Detalle, Fecha, Hora);
                if (Crear == true)
                {
                    MessageBox.Show("Retiro Creado Correctamente", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
            }
        }
    }
}
