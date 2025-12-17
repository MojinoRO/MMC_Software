using System;
using System.Windows;
using System.Windows.Controls;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaActualizarCostosCompra.xaml
    /// </summary>
    public partial class VentanaActualizarCostosCompra : Window
    {

        string _ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
        private int _ArticuloID;
        private decimal _CostoAnterior;
        private decimal _CostoNuevo;
        private decimal _CostoConIvaNew;
        private decimal _IvaArticulo;
        private decimal _Margen;
        private decimal _Incremento;
        private decimal _PrecioVenta;
        private decimal _PrecioNuevo;
        public VentanaActualizarCostosCompra(int ArticuloID, decimal CostoAnterior, decimal CostoNuevo, decimal CostoConIva, decimal IvaArticulo,
            decimal Margen, decimal Incremento, decimal PrecioVenta)
        {
            InitializeComponent();
            _ArticuloID = ArticuloID;
            _CostoAnterior = CostoAnterior;
            _CostoNuevo = CostoNuevo;
            _CostoConIvaNew = CostoConIva;
            _IvaArticulo = IvaArticulo;
            _Margen = Margen;
            _Incremento = Incremento;
            _PrecioVenta = PrecioVenta;
            chkRedondearVenta.IsChecked = true;
            CargarDatos();
            btnAceptar.Focus();
        }

        private void CargarDatos()
        {
            txtCostoAnterior.Text = _CostoAnterior.ToString("N2");
            txtNuevoCosto.Text = _CostoNuevo.ToString("N2");
            txtPorcentaje.Text = _Incremento.ToString("N2");
            txtMargen.Text = _Margen.ToString("N2");
            txtIva.Text = _IvaArticulo.ToString("N2");
            txtValorAnterior.Text = _PrecioVenta.ToString("N2");
            CalcularValorVenta();
        }

        private void CalcularValorVenta()
        {
            var ROperaciones = new RepositorioOperacionesMatematicas();
            if (chkRedondearVenta.IsChecked == true)
            {
                decimal VentaNew = ROperaciones.CalcularValorVentaIncremento(2, _CostoNuevo, _Incremento, _IvaArticulo);
                _PrecioNuevo = VentaNew;
                txtNuevoValor.Text = VentaNew.ToString("N2");
            }
            else
            {
                decimal VentaNew = ROperaciones.CalcularValorVentaIncremento(1, _CostoNuevo, _Incremento, _IvaArticulo);
                _PrecioNuevo = VentaNew;
                txtNuevoValor.Text = VentaNew.ToString("N2");
            }
        }

        private void chkRedondearVenta_Checked(object sender, RoutedEventArgs e)
        {
            var ROperaciones = new RepositorioOperacionesMatematicas();
            txtNuevoValor.Text = ROperaciones.CalcularValorVentaIncremento(2, _CostoNuevo, _Incremento, _IvaArticulo).ToString("N2");
        }

        private void chkRedondearVenta_Unchecked(object sender, RoutedEventArgs e)
        {
            var ROperaciones = new RepositorioOperacionesMatematicas();
            txtNuevoValor.Text = ROperaciones.CalcularValorVentaIncremento(1, _CostoNuevo, _Incremento, _IvaArticulo).ToString("N2");
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtPorcentaje_TextChanged(object sender, TextChangedEventArgs e)
        {
            string Incremento = txtPorcentaje.Text;
            if (string.IsNullOrEmpty(Incremento))
            {
                txtPorcentaje.Text = _Incremento.ToString("N2");
                CalcularValorVenta();
                return;
            }
            _Incremento = Convert.ToDecimal(Incremento);
            CalcularValorVenta();
            return;
        }

        private void txtMargen_TextChanged(object sender, TextChangedEventArgs e)
        {
            string Margen = txtMargen.Text;
            if (string.IsNullOrEmpty(Margen))
            {
                txtMargen.Text = _Margen.ToString("N2");
                return;
            }
            _Margen = Convert.ToDecimal(Margen);
            CalcularValorVenta();
            return;
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            string ValorVenta = txtNuevoValor.Text;
            if (string.IsNullOrEmpty(ValorVenta))
            {
                txtNuevoValor.Text = _PrecioVenta.ToString("N2");
                return;
            }
            var RCompras = new RepositorioCompras(_ConexionSql);
            RCompras.ActualizarCostoArticulos(_CostoAnterior, _CostoNuevo, _CostoConIvaNew, _Margen, _Incremento, _PrecioNuevo, _ArticuloID);
            this.Close();
        }
    }
}
