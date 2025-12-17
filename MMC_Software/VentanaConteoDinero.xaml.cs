using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaConteoDinero.xaml
    /// </summary>
    public partial class VentanaConteoDinero : Window
    {
        //==================================================================
        //               Variables de cant y valor 
        //==================================================================
        private int _CantBillete100;
        private int _CantBillete50;
        private int _CantBillete20;
        private int _CantBillete10;
        private int _CantBillete5;
        private int _CantBillete2;
        private int _CantMoneda1K;
        private int _CantMoneda500K;
        private int _CantMoneda200K;
        private int _CantMoneda100K;
        private int _CantMoneda50K;

        private int _ValorTotal100;
        private int _ValorTotal50;
        private int _ValorTotal20;
        private int _ValorTotal10;
        private int _ValorTotal5;
        private int _ValorTotal2;
        private int _ValorTotal1K;
        private int _ValorTotal500K;
        private int _ValorTotal200K;
        private int _ValorTotal100K;
        private int _ValorTotal50K;
        public int _TotalEntregado;
        //===================================================================
        private bool _EstaModificando = false;
        private int _VendedorID;
        private DateTime _Fecha;
        private int _ConteoID;
        public VentanaConteoDinero(int VendedorID, DateTime Fecha)
        {
            InitializeComponent();
            _VendedorID = VendedorID;
            _Fecha = Fecha;
            LLenarFecha();
            LlenarVendedor();
            ValidaExiste();

        }
        private void LLenarFecha()
        {
            dpFechaConteo.SelectedDate = _Fecha;
        }

        private void LlenarVendedor()
        {
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
            var Vendedores = new VendedoresRepositorio(ConexionSql);
            DataTable dt = Vendedores.GenerarVendedores();
            cmbVendedorSeleccionado.DisplayMemberPath = "INFOVENDEDOR";
            cmbVendedorSeleccionado.SelectedValuePath = "VendedorID";
            cmbVendedorSeleccionado.SelectedValue = _VendedorID;
            cmbVendedorSeleccionado.ItemsSource = dt.DefaultView;
            cmbVendedorSeleccionado.IsEnabled = false;
        }
        private void BloquearCampos()
        {
            txtCant100000.IsEnabled = false;
            txtCant50000.IsEnabled = false;
            txtCant20000.IsEnabled = false;
            txtCant10000.IsEnabled = false;
            txtCant5000.IsEnabled = false;
            txtCant2000.IsEnabled = false;
            txtCant1000.IsEnabled = false;
            txtCant500.IsEnabled = false;
            txtCant200.IsEnabled = false;
            txtCant100.IsEnabled = false;
            txtCant50.IsEnabled = false;
            txtTotal100000.IsEnabled = false;
            txtTotal50000.IsEnabled = false;
            txtTotal20000.IsEnabled = false;
            txtTotal10000.IsEnabled = false;
            txtTotal5000.IsEnabled = false;
            txtTotal2000.IsEnabled = false;
            txtTotal1000.IsEnabled = false;
            txtTotal500.IsEnabled = false;
            txtTotal200.IsEnabled = false;
            txtTotal100.IsEnabled = false;
            txtTotal50.IsEnabled = false;

        }
        private void DesbloquearCampos()
        {
            txtCant100000.IsEnabled = true;
            txtCant50000.IsEnabled = true;
            txtCant20000.IsEnabled = true;
            txtCant10000.IsEnabled = true;
            txtCant5000.IsEnabled = true;
            txtCant2000.IsEnabled = true;
            txtCant1000.IsEnabled = true;
            txtCant500.IsEnabled = true;
            txtCant200.IsEnabled = true;
            txtCant100.IsEnabled = true;
            txtCant50.IsEnabled = true;
            txtTotal100000.IsEnabled = true;
            txtTotal50000.IsEnabled = true;
            txtTotal20000.IsEnabled = true;
            txtTotal10000.IsEnabled = true;
            txtTotal5000.IsEnabled = true;
            txtTotal2000.IsEnabled = true;
            txtTotal1000.IsEnabled = true;
            txtTotal500.IsEnabled = true;
            txtTotal200.IsEnabled = true;
            txtTotal100.IsEnabled = true;
            txtTotal50.IsEnabled = true;
        }

        private void TxtCantMoneda_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex Expresion = new Regex("[^0-9]+");
            e.Handled = Expresion.IsMatch(e.Text);
        }

        private int CalcularValor(int Cantidad, int valor)
        {
            int ValorTotal = Cantidad * valor;
            return ValorTotal;
        }

        private void txtCant100000_TextChanged(object sender, TextChangedEventArgs e)
        {
            string CantidadDig = txtCant100000.Text;
            if (string.IsNullOrEmpty(CantidadDig))
            {
                return;
            }
            int Cantidad = Convert.ToInt32(txtCant100000.Text);
            _CantBillete100 = Cantidad;
            int Valor = 100000;
            int Total = CalcularValor(Cantidad, Valor);
            txtTotal100000.Text = Total.ToString("N0");
            _ValorTotal100 = Total;
            llenarTotal();

        }

        private void txtCant50000_TextChanged(object sender, TextChangedEventArgs e)
        {
            string CantidadDig = txtCant50000.Text;
            if (string.IsNullOrEmpty(CantidadDig))
            {
                return;
            }
            int Cantidad = Convert.ToInt32(txtCant50000.Text);
            _CantBillete50 = Cantidad;
            int Valor = 50000;
            int Total = CalcularValor(Cantidad, Valor);
            txtTotal50000.Text = Total.ToString("N0");
            _ValorTotal50 = Total;
            llenarTotal();

        }

        private void txtCant20000_TextChanged(object sender, TextChangedEventArgs e)
        {
            string CantidadDig = txtCant20000.Text;
            if (string.IsNullOrEmpty(CantidadDig))
            {
                return;
            }
            int Cantidad = Convert.ToInt32(txtCant20000.Text);
            _CantBillete20 = Cantidad;
            int Valor = 20000;
            int Total = CalcularValor(Cantidad, Valor);
            txtTotal20000.Text = Total.ToString("N0");
            _ValorTotal20 = Total;
            llenarTotal();

        }
        private void txtCant10000_TextChanged(object sender, TextChangedEventArgs e)
        {
            string CantidadDig = txtCant10000.Text;
            if (string.IsNullOrEmpty(CantidadDig))
            {
                return;
            }
            int Cantidad = Convert.ToInt32(txtCant10000.Text);
            _CantBillete10 = Cantidad;
            int Valor = 10000;
            int Total = CalcularValor(Cantidad, Valor);
            txtTotal10000.Text = Total.ToString("N0");
            _ValorTotal10 = Total;
            llenarTotal();

        }

        private void txtCant5000_TextChanged(object sender, TextChangedEventArgs e)
        {
            string CantidadDig = txtCant5000.Text;
            if (string.IsNullOrEmpty(CantidadDig))
            {
                return;
            }
            int Cantidad = Convert.ToInt32(txtCant5000.Text);
            _CantBillete5 = Cantidad;
            int Valor = 5000;
            int Total = CalcularValor(Cantidad, Valor);
            txtTotal5000.Text = Total.ToString("N0");
            _ValorTotal5 = Total;
            llenarTotal();
        }

        private void txtCant2000_TextChanged(object sender, TextChangedEventArgs e)
        {
            string CantidadDig = txtCant2000.Text;
            if (string.IsNullOrEmpty(CantidadDig))
            {
                return;
            }
            int Cantidad = Convert.ToInt32(txtCant2000.Text);
            _CantBillete2 = Cantidad;
            int Valor = 2000;
            int Total = CalcularValor(Cantidad, Valor);
            txtTotal2000.Text = Total.ToString("N0");
            _ValorTotal2 = Total;
            llenarTotal();

        }

        private void txtCant1000_TextChanged(object sender, TextChangedEventArgs e)
        {
            string CantidadDig = txtCant1000.Text;
            if (string.IsNullOrEmpty(CantidadDig))
            {
                return;
            }
            int Cantidad = Convert.ToInt32(txtCant1000.Text);
            _CantMoneda1K = Cantidad;
            int Valor = 1000;
            int Total = CalcularValor(Cantidad, Valor);
            txtTotal1000.Text = Total.ToString("N0");
            _ValorTotal1K = Total;
            llenarTotal();
        }

        private void txtCant500_TextChanged(object sender, TextChangedEventArgs e)
        {
            string CantidadDig = txtCant500.Text;
            if (string.IsNullOrEmpty(CantidadDig))
            {
                return;
            }
            int Cantidad = Convert.ToInt32(txtCant500.Text);
            _CantMoneda500K = Cantidad;
            int Valor = 500;
            int Total = CalcularValor(Cantidad, Valor);
            txtTotal500.Text = Total.ToString("N0");
            _ValorTotal500K = Total;
            llenarTotal();
        }

        private void txtCant200_TextChanged(object sender, TextChangedEventArgs e)
        {
            string CantidadDig = txtCant200.Text;
            if (string.IsNullOrEmpty(CantidadDig))
            {
                return;
            }
            int Cantidad = Convert.ToInt32(txtCant200.Text);
            _CantMoneda200K = Cantidad;
            int Valor = 200;
            int Total = CalcularValor(Cantidad, Valor);
            txtTotal200.Text = Total.ToString("N0");
            _ValorTotal200K = Total;
            llenarTotal();
        }

        private void txtCant100_TextChanged(object sender, TextChangedEventArgs e)
        {
            string CantidadDig = txtCant100.Text;
            if (string.IsNullOrEmpty(CantidadDig))
            {
                return;
            }
            int Cantidad = Convert.ToInt32(txtCant100.Text);
            _CantMoneda100K = Cantidad;
            int Valor = 100;
            int Total = CalcularValor(Cantidad, Valor);
            txtTotal100.Text = Total.ToString("N0");
            _ValorTotal100K = Total;
            llenarTotal();
        }

        private void txtCant50_TextChanged(object sender, TextChangedEventArgs e)
        {
            string CantidadDig = txtCant50.Text;
            if (string.IsNullOrEmpty(CantidadDig))
            {
                return;
            }
            int Cantidad = Convert.ToInt32(txtCant50.Text);
            _CantMoneda50K = Cantidad;
            int Valor = 50;
            int Total = CalcularValor(Cantidad, Valor);
            txtTotal50.Text = Total.ToString("N0");
            _ValorTotal50K = Total;
            llenarTotal();
        }

        private void llenarTotal()
        {
            string Signo = "$";
            int TotalEntregado = (_ValorTotal100 + _ValorTotal50 + _ValorTotal20 + _ValorTotal10 + _ValorTotal5 + _ValorTotal2 +
                                  _ValorTotal1K + _ValorTotal500K + _ValorTotal200K + _ValorTotal100K + _ValorTotal50K);
            _TotalEntregado = TotalEntregado;
            TxtTotalGeneral.Text = Signo + TotalEntregado.ToString("N0");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _CantBillete100 = LeerValor(txtCant100000);
                _CantBillete50 = LeerValor(txtCant50000);
                _CantBillete20 = LeerValor(txtCant20000);
                _CantBillete10 = LeerValor(txtCant10000);
                _CantBillete5 = LeerValor(txtCant5000);
                _CantBillete2 = LeerValor(txtCant2000);
                _CantMoneda1K = LeerValor(txtCant1000);
                _CantMoneda500K = LeerValor(txtCant500);
                _CantMoneda200K = LeerValor(txtCant200);
                _CantMoneda100K = LeerValor(txtCant100);
                _CantMoneda50K = LeerValor(txtCant50);
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                var Cierre = new RepositorioCierreCaja(ConexionSql);
                if (_EstaModificando == false)
                {
                    Cierre.InsertarConteoCierre(_VendedorID, _Fecha, _CantBillete100, _CantBillete50, _CantBillete20, _CantBillete10, _CantBillete5, _CantBillete2,
                                                _CantMoneda1K, _CantMoneda500K, _CantMoneda200K, _CantMoneda100K, _CantMoneda50K);
                }
                else
                {
                    Cierre.ActualizarConteoCiere(_ConteoID, _CantBillete100, _CantBillete50, _CantBillete20, _CantBillete10, _CantBillete5, _CantBillete2,
                                                _CantMoneda1K, _CantMoneda500K, _CantMoneda200K, _CantMoneda100K, _CantMoneda50K);
                    MessageBox.Show("Conteo Actualizado Con Exito", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                BloquearCampos();
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private int LeerValor(TextBox Txt)
        {
            if (string.IsNullOrEmpty(Txt.Text))
            {
                Txt.Text = "0";
            }
            return int.TryParse(Txt.Text, out int Valor) ? Valor : 0;
        }

        private void ValidaExiste()
        {
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
            var Cierre = new RepositorioCierreCaja(ConexionSql);
            DataTable dt = Cierre.ValidaExisteConteo(_VendedorID, _Fecha);
            if (dt.Rows.Count > 0)
            {
                _EstaModificando = true;
                txtCant100000.Text = dt.Rows[0]["Bill100"].ToString();
                txtCant50000.Text = dt.Rows[0]["Bill50"].ToString();
                txtCant20000.Text = dt.Rows[0]["Bill20"].ToString();
                txtCant10000.Text = dt.Rows[0]["Bill10"].ToString();
                txtCant5000.Text = dt.Rows[0]["Bill5"].ToString();
                txtCant2000.Text = dt.Rows[0]["Bill2"].ToString();
                txtCant1000.Text = dt.Rows[0]["Mon1000"].ToString();
                txtCant500.Text = dt.Rows[0]["Mon500"].ToString();
                txtCant200.Text = dt.Rows[0]["Mon200"].ToString();
                txtCant100.Text = dt.Rows[0]["Mon100"].ToString();
                txtCant50.Text = dt.Rows[0]["Mon50"].ToString();
                _ConteoID = Convert.ToInt32(dt.Rows[0]["ConteoID"]);
            }
        }
    }
}
