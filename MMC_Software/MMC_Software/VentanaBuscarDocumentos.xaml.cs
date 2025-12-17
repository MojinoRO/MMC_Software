using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static MMC_Software.ModuloPuntoVenta;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaBuscarDocumentos.xaml
    /// </summary>
    public partial class VentanaBuscarDocumentos : Window
    {
        public DataTable _DatosEncabezado;
        public List<ArticulosAgregados> _ArticulosAgregados;

        public int? _DocumentoID;
        public int? _TerceroID;
        public VentanaBuscarDocumentos()
        {
            InitializeComponent();
            dpDesde.SelectedDate = DateTime.Now;
            dpHasta.SelectedDate = DateTime.Now;
            LlenaCriterioBusqueda();
            TxtFiltroDocumento.Focus();
        }

        private void LlenaCriterioBusqueda()
        {
            List<KeyValuePair<int, string>> ListaCriterios = new List<KeyValuePair<int, string>>()
            {
                new KeyValuePair<int,string>(1,"Rango De Fechas"),
                new KeyValuePair<int,string>(2,"Docto & Numero"),
                new KeyValuePair<int, string>(3,"Nit/CC & Rango Fechas"),
                new KeyValuePair<int, string>(4,"Docto & Fechas & Nit/CC")
            };

            CmbFiltroCriterioBusqueda.DisplayMemberPath = "Value";
            CmbFiltroCriterioBusqueda.SelectedValuePath = "Key";
            CmbFiltroCriterioBusqueda.ItemsSource = ListaCriterios;
            CmbFiltroCriterioBusqueda.SelectedIndex = 0;
        }

        public class LlenarFacturasBuscadas
        {
            public int VentaID { get; set; }
            public string Docto { get; set; }
            public int NumeroDocto { get; set; }
            public DateTime Fecha { get; set; }
            public string NitCC { get; set; }
            public string Nombres { get; set; }
            public int Items { get; set; }
            public decimal TotalVenta { get; set; }
            public string FormaPago { get; set; }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                var BuscadorRespositorio = new RepositorioBuscarDocumentos(ConexionSql);
                int criterioBusquedaKey = Convert.ToInt32(CmbFiltroCriterioBusqueda.SelectedValue);
                if (criterioBusquedaKey == 1)
                {
                    DateTime Desde = dpDesde.SelectedDate.Value;
                    DateTime hasta = dpHasta.SelectedDate.Value;
                    dgListadoFacturas.ItemsSource = BuscadorRespositorio.ObtenerVentasPorFechas(Desde, hasta);
                }
                else if (criterioBusquedaKey == 2)
                {
                    int? NumeroDocto = Convert.ToInt32(TxtFiltroNumeroDocumento.Text);
                    if (NumeroDocto == null)
                    {
                        MessageBox.Show("Debe Digitar Numero De Factura", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                    TxtFiltroDocumento.IsReadOnly = true;
                    dgListadoFacturas.ItemsSource = BuscadorRespositorio.obtenerVentasXDoctoNumero(_DocumentoID, NumeroDocto);
                }
                else if (criterioBusquedaKey == 3)
                {
                    DateTime desde = dpDesde.SelectedDate.Value;
                    DateTime Hasta = dpHasta.SelectedDate.Value;
                    if (_TerceroID == null || _TerceroID == 0)
                    {
                        MessageBox.Show("Debe Digitar Cedula", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                    dgListadoFacturas.ItemsSource = BuscadorRespositorio.ObtenerVentasXFechasTerceros(desde, Hasta, _TerceroID);
                }
                else if (criterioBusquedaKey == 4)
                {
                    DateTime Desde = dpDesde.SelectedDate.Value;
                    DateTime Hasta = dpHasta.SelectedDate.Value;
                    if (_DocumentoID == null)
                    {
                        MessageBox.Show("Debe Seleccionar Documento", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        TxtFiltroDocumento.Focus();
                        return;
                    }
                    if (_TerceroID == null)
                    {
                        MessageBox.Show("Debe Seleccionar Cliente", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        TxtFiltroNitCC.Focus();
                        return;
                    }
                    dgListadoFacturas.ItemsSource = BuscadorRespositorio.obtenerFacturasXDoctoXRangoFechasXTerceros(Desde, Hasta, _DocumentoID, _TerceroID);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }



        private void TxtFiltroDocumento_TextChanged(object sender, TextChangedEventArgs e)
        {
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
            string Filtro = TxtFiltroDocumento.Text;
            var RespositorioDocumentos = new RepositorioBuscarDocumentos(ConexionSql);
            DataTable dt = RespositorioDocumentos.LLenarDocumento(Filtro);
            LstFiltroDocumento.DisplayMemberPath = "Documento";
            LstFiltroDocumento.SelectedValuePath = "DocumentoID";
            LstFiltroDocumento.ItemsSource = dt.DefaultView;
            PopupDocumentos.IsOpen = dt.Rows.Count > 0;
        }

        private void LstFiltroDocumento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstFiltroDocumento.SelectedItem is DataRowView Row)
            {
                TxtFiltroDocumento.Text = Row["Documento"].ToString();
                if (Row.Row.Table.Columns.Contains("DocumentoID"))
                {
                    _DocumentoID = Convert.ToInt32(Row["DocumentoID"]);
                }
                else
                {
                    MessageBox.Show("ERROR");
                }
                TxtFiltroNumeroDocumento.Focus();
                PopupDocumentos.IsOpen = false;
            }
        }

        private void TxtFiltroNumeroDocumento_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, "[0-9]+$");
        }

        private void TxtFiltroNitCC_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                    string Cedula = TxtFiltroNitCC.Text;
                    var RespositorioUsuarios = new RepositorioBuscarDocumentos(ConexionSql);
                    DataTable dt = RespositorioUsuarios.BuscarCCNit(Cedula);
                    int TipoDocumento = Convert.ToInt32(dt.Rows[0]["TercerosTipoDocumento"]);
                    if (TipoDocumento == 31)
                    {
                        string NombreORazonSocial = dt.Rows[0]["TerceroRazonSocial"].ToString();
                        TxtNombres.Text = NombreORazonSocial;
                    }
                    else
                    {
                        string NombreORazonSocial = dt.Rows[0]["TercerosNombreCompleto"].ToString();
                        TxtNombres.Text = NombreORazonSocial;
                    }
                    int TerceroID = Convert.ToInt32(dt.Rows[0]["TercerosID"]);
                    _TerceroID = TerceroID;
                    TxtNombres.IsEnabled = false;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR " + ex.Message, "Mensaje", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void dgListadoFacturas_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dgListadoFacturas.SelectedItem is LlenarFacturasBuscadas facturaseleccionada)
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                int VentaID = facturaseleccionada.VentaID;
                var Repositorio = new RepositorioBuscarDocumentos(ConexionSql);
                _DatosEncabezado = Repositorio.LlenarDatosEncabezado(VentaID);
                _ArticulosAgregados = Repositorio.LLenarListadoArticulos(VentaID);
                this.DialogResult = true;
                this.Close();
            }
        }

    }
}
