using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaInfoCierre.xaml
    /// </summary>
    public partial class VentanaInfoCierre : Window
    {
        private readonly int _VendedorID;
        private double _ValorConteo;
        private readonly DateTime _Fecha;
        private decimal _ValorBase;
        private decimal _ValorRetiros;
        private decimal _ValorTotal;
        private decimal _ValorEfectivo;
        private decimal _Diferencia;

        public VentanaInfoCierre(int VendedorID, double ValorConteo, DateTime Fecha)
        {
            InitializeComponent();
            _VendedorID = VendedorID;
            _ValorConteo = ValorConteo;
            _Fecha = Fecha;
            LlenarDatosCierre();
            LlenarCierre();
        }

        private void LlenarDatosCierre()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                DataTable Dt = CacheAplicacion.CacheGlobal.DatosEmpresa;
                TxtNombreEmpresa.Text = Dt.Rows[0]["EmpresaEstablecimiento"].ToString();
                Txtnit.Text = Dt.Rows[0]["Nit"].ToString();
                TxtDireccion.Text = Dt.Rows[0]["EmpresaDireccion"].ToString();
                TxtTelefono.Text = Dt.Rows[0]["EmpresaTelefono"].ToString();
                var Vendedor = new VendedoresRepositorio(ConexionSql);
                DataTable dt = Vendedor.GenerarInfoVendedorUnitario(_VendedorID);
                TxtVendeodor.Text = dt.Rows[0]["INFOCOMPLETA"].ToString();
                TxtFecha.Text = _Fecha.ToString("dd/MM/yyyy");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Seleccione Vendedor Para Cierre" + ex.Message, "Mensaje", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        public class RelacionFacturas
        {
            public string Docto { get; set; }
            public int NumDesde { get; set; }
            public int NumHasta { get; set; }
            public int Cantidad { get; set; }
        }

        public class RelacionImpuesto
        {
            public string Tarifa { get; set; }
            public decimal Base { get; set; }
            public decimal Impuesto { get; set; }
            public decimal BaseMasImpuesto { get; set; }
        }

        public class RelacionCategorias
        {
            public string Categoria { get; set; }
            public int Cantidad { get; set; }
        }

        public class RelacionFormasPago
        {
            public string FormaPago { get; set; }
            public int Cantidad { get; set; }
            public int ValorTotal { get; set; }
        }

        private void LlenarCierre()
        {
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
            var Cierre = new RepositorioCierreCaja(ConexionSql);
            dgRelacionFacturas.ItemsSource = Cierre.Facturas(_VendedorID, _Fecha);
            dgRelacionImpuestos.ItemsSource = Cierre.Impuestos(_VendedorID, _Fecha);
            dgRelacionProductos.ItemsSource = Cierre.Categorias(_VendedorID, _Fecha);
            DataTable dt = Cierre.llenarRetirosCaja(_VendedorID, _Fecha);
            int ValorRetiros = 0;
            if (dt.Rows.Count > 0)
            {
                int CantRetiros = Convert.ToInt32(dt.Rows[0]["Cantidad"]);
                ValorRetiros = Convert.ToInt32(dt.Rows[0]["ValorCierre"]);
                TxtRetirosCaja.Text = "Cant: " + CantRetiros.ToString("N0") + "        " + ValorRetiros.ToString("N0");
            }
            var FormasPago = Cierre.FormasPago(_VendedorID, _Fecha);
            dgFormasPago.ItemsSource = FormasPago;
            int ValorBase = Cierre.LlenarBaseCajaConteo(_VendedorID);
            int ValorTotal = FormasPago.Sum(f => f.ValorTotal);
            int ValorEfectivo = FormasPago.Where(F => F.FormaPago == "001 EFECTIVO").Sum(f => f.ValorTotal);
            int SumaEfectivo = ValorBase + ValorEfectivo - ValorRetiros;
            int Diferencia = SumaEfectivo - Convert.ToInt32(_ValorConteo);
            TxtBaseCaja.Text = ValorBase.ToString("N0");
            TxtConteoCaja.Text = _ValorConteo.ToString("N0");
            TxtTotalMovimiento.Text = ValorTotal.ToString("N0");
            TxtEfecitvoPorEntregar.Text = SumaEfectivo.ToString("N0");
            if (Diferencia <= 0)
            {
                TxtEstadoCaja.Text = "Sobran: " + Diferencia.ToString("N0");
                _Diferencia = -Diferencia;
            }
            else
            {
                TxtEstadoCaja.Text = "Faltan: " + Diferencia.ToString("N0");
                _Diferencia = Diferencia;
            }
            _ValorBase = ValorBase;
            _ValorTotal = ValorTotal;
            _ValorEfectivo = SumaEfectivo;
            _ValorRetiros = ValorRetiros;
            _Diferencia = Diferencia;
        }

        private void ButtonImprimir_Click(object sender, RoutedEventArgs e)
        {
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
            var Cierre = new RepositorioCierreCaja(ConexionSql);
            string Vendedor = TxtVendeodor.Text;

            var ListadoFacturas = dgRelacionFacturas.ItemsSource as List<RelacionFacturas>;
            if (ListadoFacturas == null)
                ListadoFacturas = (dgRelacionFacturas.ItemsSource as IEnumerable<RelacionFacturas>)?.ToList();

            var ListadoFormasPago = dgFormasPago.ItemsSource as List<RelacionFormasPago>;
            if (ListadoFormasPago == null)
                ListadoFormasPago = (dgFormasPago.ItemsSource as IEnumerable<RelacionFormasPago>)?.ToList();

            var ListadoImpuestos = dgRelacionImpuestos.ItemsSource as List<RelacionImpuesto>;
            if (ListadoImpuestos == null)
                ListadoImpuestos = (dgRelacionImpuestos.ItemsSource as IEnumerable<RelacionImpuesto>)?.ToList();

            var ListadoCategorias = dgRelacionProductos.ItemsSource as List<RelacionCategorias>;
            if (ListadoCategorias == null)
                ListadoCategorias = (dgRelacionProductos.ItemsSource as IEnumerable<RelacionCategorias>)?.ToList();

            int CierreID = Cierre.InsertarCierreCaja(Vendedor, _Fecha, _ValorBase,
                Convert.ToDecimal(_ValorConteo), _ValorTotal, _ValorEfectivo, _ValorRetiros,
                _Diferencia, ListadoFacturas, ListadoFormasPago, ListadoImpuestos, ListadoCategorias);

            if (CierreID != 0)
            {
                MessageBox.Show("Cierre de caja guardado correctamente ✅", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("error", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            //IMPRESION CIERRE
            VentanaImpresionDocumentos ventana = new VentanaImpresionDocumentos(CierreID);
            ventana.Owner = this;
            ventana.ShowDialog();

        }
    }
}
