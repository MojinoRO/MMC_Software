using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Permissions;
using System.Security.RightsManagement;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Compilation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para BuscadorDocumentos.xaml
    /// </summary>
    public partial class BuscadorDocumentos : Window
    {
        /// <summary>
        /// el parametro tipo busqueda es para saber si es compra , ventas ,ajustes etc
        /// 1=compras
        /// 2= ventas
        /// .
        /// .
        /// .
        /// 
        /// </summary>
        private int _BuscadorDocumentos;


        string _ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);


        /// <summary>
        /// FIltro Documento
        /// </summary>
        /// 

        public class DocumentoSelecionado
        {
            public int DocumentoID
            {
                get; set;
            }
            public string NombreDocto { get; set; }
        }

        private int _NumDocumento;

        List<DocumentoSelecionado> _Documento = new List<DocumentoSelecionado>();



        public class TerceroSeleccionado
        {
            public int TerceroID {  get; set; }
            public string Identificacion {  get; set; }
            public string NombreTercero {  get; set; }
        }
        List<TerceroSeleccionado> _Tercero = new List<TerceroSeleccionado>();

        public BuscadorDocumentos(int TipoBusqueda)
        {
            InitializeComponent();
            _BuscadorDocumentos = TipoBusqueda;
            LLenarFiltros();
            dtpDesde.SelectedDate = DateTime.Now;
            dtpHasta.SelectedDate = DateTime.Now;
        }

        #region UI GRAFIC

        private void txtCodigoDocumento_TextChanged(object sender, TextChangedEventArgs e)
        {
            string Codigo = txtCodigoDocumento.Text;
            var RDocumentos = new DocumentosVentaRespositorio(_ConexionSql);
            DataTable dt = RDocumentos.BuscarDoctoComprasXcodigo(Codigo, _BuscadorDocumentos);
            if (dt.Rows.Count > 0)
            {
                lstDocumentos.Visibility = Visibility.Visible;
                lstDocumentos.DisplayMemberPath = "InfoDocto";
                lstDocumentos.SelectedValuePath = "DocumentoID";
                lstDocumentos.ItemsSource = dt.DefaultView;

            }
        }


        private void ConfirmarSeleccion()
        {
            var Seleccionado = (DataRowView)lstDocumentos.SelectedItem;
            if (Seleccionado != null)
            {
                var Documento = new DocumentoSelecionado
                {
                    DocumentoID = Convert.ToInt32(Seleccionado["DocumentoID"]),
                    NombreDocto = Seleccionado["InfoDocto"].ToString()
                };
                _Documento.Clear();
                _Documento.Add(Documento);
                lstDocumentos.Visibility = Visibility.Collapsed;
                txtCodigoDocumento.Text = Documento.NombreDocto.ToString();
                txtNumeroDocumento.Focus();
            }

        }


        private void lstDocumentos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ConfirmarSeleccion();
        }

        private void txtCodigoDocumento_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (lstDocumentos.Items.Count > 0 && e.Key == Key.Enter)
            {
                lstDocumentos.Focus();
                lstDocumentos.SelectedIndex = 0;
            }
        }

        private void lstDocumentos_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (lstDocumentos.Items.Count > 0 && e.Key == Key.Enter)
            {
                ConfirmarSeleccion();
            }
        }

        private void txtTercero_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                VentanaBuscarTerceros Ventana = new VentanaBuscarTerceros();
                bool? selecciono = Ventana.ShowDialog();
                try
                {
                    if (selecciono == true && Ventana.TerceroSelecionado.TerceroID != 0)
                    {
                        var Tercero = new TerceroSeleccionado
                        {
                            TerceroID = Ventana.TerceroSelecionado.TerceroID,
                            Identificacion = Ventana.TerceroSelecionado.TercerosIdentificacion,
                            NombreTercero = Ventana.TerceroSelecionado.TerceroNobres
                        };

                        _Tercero.Clear();
                        _Tercero.Add(Tercero);
                        txtTercero.Text = Tercero.Identificacion.ToString() + " " + Tercero.NombreTercero.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Seleccione filtros Correctos" + ex.Message);
                }
            }
        }

        private void txtNumeroDocumento_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex Expre = new Regex("[^0-9]+");
            e.Handled = Expre.IsMatch(e.Text);
        }

        private void txtNumeroDocumento_TextInput(object sender, TextCompositionEventArgs e)
        {
            string Texto = txtNumeroDocumento.Text;
            if (string.IsNullOrEmpty(Texto))
            {
                return;
            }
            _NumDocumento = Convert.ToInt32(Texto);
        }
        private void LLenarFiltros()
        {
            List<KeyValuePair<int, string>> TiposDocumentos = new List<KeyValuePair<int, string>>();
            var ListaTiposDocumentos = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(1,"Entradas"),
                new KeyValuePair<int, string>(2,"Salidas"),
                new KeyValuePair<int, string>(3,"CarteraCliente"),
                new KeyValuePair<int, string>(4,"CarteraProveedor"),
                new KeyValuePair<int, string>(6,"Devolucion Entradas"),
                new KeyValuePair<int, string>(7,"Devolucion Salidas")
            };

            List<KeyValuePair<int, string>> CriterioBusqueda = new List<KeyValuePair<int, string>>();
            var listaCriterioBusqueda = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(1,"RangoFechas"),
                new KeyValuePair<int,string>(2,"Docto/Fechas"),
                new KeyValuePair<int,string>(3,"Docto&Numero"),
                new KeyValuePair<int, string>(4,"Cliente/Proveedor"),
                new KeyValuePair<int, string>(5,"ClienteProveedorFechas")
            };

            cmbCriterioBusqueda.DisplayMemberPath = "Value";
            cmbCriterioBusqueda.SelectedValuePath = "Key";
            cmbCriterioBusqueda.ItemsSource = listaCriterioBusqueda;

            cmbTipoDocumento.DisplayMemberPath = "Value";
            cmbTipoDocumento.SelectedValuePath = "Key";
            cmbTipoDocumento.ItemsSource = ListaTiposDocumentos;

            if (_BuscadorDocumentos == 1)
            {
                cmbTipoDocumento.SelectedValue = 1;
                cmbCriterioBusqueda.SelectedValue = 1;
            }
        }
        #endregion


        #region LogicaBuscador

        public class ListadoFacturas
        {
            public int CompraID {  get; set; }
            public int DocumentoID {  get; set; }
            public string Docto {  get; set; }
            public string NombreDocto {  get; set; }
            public int VendedorID {  get; set; }
            public int Numero {  get; set; }
            public string Factura {  get; set; }
            public DateTime Fecha {  get; set; }
            public string Tercero {  get; set; }
            public decimal Total { get; set; }
        }

        public ObservableCollection<ListadoFacturas> Facturas = new ObservableCollection<ListadoFacturas>();



        #endregion

        #region Botones

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Documento = txtCodigoDocumento.Text;
                int TipoFiltro = Convert.ToInt32(cmbCriterioBusqueda.SelectedValue);
                if(TipoFiltro ==2 && string.IsNullOrEmpty(Documento))
                {
                    MessageBox.Show("Debe seleccionar Documento a buscar", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                int DocumentoID = _Documento.Count > 0 ? _Documento[0].DocumentoID : 0;
                
                string NumeroDocto =txtNumeroDocumento.Text;
                if(TipoFiltro==3 && string.IsNullOrEmpty(NumeroDocto))
                {
                    MessageBox.Show("Digite Numero de Documento a buscar", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                string Tercero = txtTercero.Text;
                if(TipoFiltro ==4 && string.IsNullOrEmpty(Tercero))
                {
                    MessageBox.Show("Digite Cedula del Documento a buscar", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if(TipoFiltro == 5 && String.IsNullOrEmpty(Tercero))
                {
                    MessageBox.Show("Digite Cedula del Documento a buscar", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                int NumeroDocumentoCompra = 0;
                int.TryParse(NumeroDocto, out NumeroDocumentoCompra);
                int TerceroID = _Tercero.Count > 0 ? _Tercero[0].TerceroID : 0;
                DateTime FechaDesde = dtpDesde.SelectedDate ?? DateTime.Now;
                DateTime FechasHasta = dtpHasta.SelectedDate ?? DateTime.Now;
               
                var RCompras = new RepositorioBuscarDocumentos(_ConexionSql);

                var Resultado = RCompras.LlenarFacturas(
                    TipoFiltro,
                    DocumentoID,
                    NumeroDocumentoCompra,
                    TerceroID,
                    FechaDesde,
                    FechasHasta);
                dgResultados.ItemsSource = null;
                dgResultados.ItemsSource = Resultado;

                if (Resultado.Count == 0)
                {
                    MessageBox.Show("No se encontraron documentos con los filtros seleccionado", "Mensaje",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.ToString());
            }

        }

        #endregion


        #region Seleccionar


        public class DocumentoSeleccionado
        {
            public int TipoDocumntoID { get; set; }
            public int MovimientoID { get; set; }
            public int DocumentoID {  get; set; }
            public string CodigoDocto { get; set; }
            public string NombrreDocto { get; set; }
            public int NumeroDocto { get; set; }
            public string Factura { get; set; }
            public DateTime FechaCompra {  get; set; }
            public int VendedorID {  get; set; }
            public string DatoProveedor {  get; set; }

        }
        public DocumentoSeleccionado docto { get; set; }

        private void ConfirmarRegistro()
        {
            if(dgResultados.Items.Count>0 && dgResultados.SelectedItem != null)
            {
                if(dgResultados.SelectedItem is ListadoFacturas Factura)
                {
                    docto = new DocumentoSeleccionado
                    {
                        TipoDocumntoID = Convert.ToInt32(cmbTipoDocumento.SelectedValue),
                        MovimientoID = Factura.CompraID,
                        CodigoDocto = Factura.Docto,
                        NombrreDocto = Factura.NombreDocto,
                        FechaCompra = Factura.Fecha,
                        Factura = Factura.Factura,
                        DatoProveedor = Factura.Tercero,
                        VendedorID = Factura.VendedorID,
                        NumeroDocto = Factura.Numero,
                        DocumentoID=Factura.DocumentoID

                    };
                    this.DialogResult = true;
                    this.Close();

                };
                
            }
        }
        #endregion

        private void dgResultados_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ConfirmarRegistro();
        }
    }
}
