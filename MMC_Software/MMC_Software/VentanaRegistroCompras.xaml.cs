using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaRegistroCompras.xaml
    /// </summary>
    public partial class VentanaRegistroCompras : UserControl
    {

        /// <summary>
        /// El registro de Compra va manejar como tipo de movimiento para insertar en el historial 
        /// 1=Entrada , 2.Salida  
        /// Se va ir actualizando la lista a medida de cada proceso 
        /// </summary>

        private int _TipoMovimiento = 1;
        private string _DetalleMovInventario = "Ingreso Por Compra";

        string _ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

        #region VariablesCompra
        int _CantDv = 0;
        int _DocumentoID = 0;
        string _CodigoDocumento;
        int _NumeroCompra = 0;
        string _NumeroFactura;
        DateTime _FechaCompra;
        int _TerceroID = 0;
        int? _VendedorID;
        string _DetalleCompra;
        int? _AlmacenID = 0;
        int _CompraID = 0;
        #endregion

        public VentanaRegistroCompras(int? VendedorID, int? AlmacenID)
        {
            InitializeComponent();
            _AlmacenID = AlmacenID;
            _VendedorID = VendedorID;
            dpFecha.SelectedDate = DateTime.Now;
            BloquearCampos();
            LlenarVendedor();
            LlenarFecha();
            Articulos = new ObservableCollection<ArticulosAgregados>();
            Articulos.CollectionChanged += Articulos_CollectionChanged;
            this.DataContext = this;
        }


        #region IU REPOSS


        private void CerrarVentana()
        {
            if(_CompraID == 0)
            {
                var TabAbierto = this.Parent as TabItem;
                if(TabAbierto != null)
                {
                    var Items = TabAbierto.Parent as TabControl;
                    if(Items != null)
                    {
                        Items.Items.Remove(TabAbierto);
                    }
                }
            }
        }
        private void LlenarFecha()
        {
            _FechaCompra = dpFecha.SelectedDate.Value;
        }
        private void BloquearCampos()
        {
            TxtDocumento.IsEnabled = false;
            TxtNombreDocto.IsEnabled = false;
            txtNumeroDocumento.IsEnabled = false;
            txtNumeroFactura.IsEnabled = false;
            dpFecha.IsEnabled = false;
            cbxDigitador.IsEnabled = false;
            txtProveedor.IsEnabled = false;
            btnBuscarProveedor.IsEnabled = false;
            txtObservaciones.IsEnabled = false;
            txtBase19.IsEnabled = false;
            txtBase5.IsEnabled = false;
            txtExento.IsEnabled = false;
            txtIVA19.IsEnabled = false;
            txtIVA5.IsEnabled = false;
            txtSubTotal.IsEnabled = false;
            txtTotalGeneralIVA.IsEnabled = false;
            txtTotalGeneralIVA.IsEnabled = false;
            txtTotalCompra.IsEnabled = false;
        }

        private void DesbloquearCampos()
        {
            TxtDocumento.IsEnabled = true;
            TxtNombreDocto.IsEnabled = true;
            txtNumeroDocumento.IsEnabled = true;
            txtNumeroFactura.IsEnabled = true;
            dpFecha.IsEnabled = true;
            cbxDigitador.IsEnabled = true;
            txtProveedor.IsEnabled = true;
            btnBuscarProveedor.IsEnabled = true;
            txtObservaciones.IsEnabled = true;
        }

        private void LimpiarCampos()
        {
            TxtDocumento.Clear();
            TxtNombreDocto.Clear();
            txtNumeroDocumento.Clear();
            txtNumeroFactura.Clear();
            dpFecha.IsEnabled = true;
            txtProveedor.Clear();
            btnBuscarProveedor.IsEnabled = true;
            txtObservaciones.Clear();
            _CompraID = 0;
            _DocumentoID = 0;
            _NumeroCompra= 0;
            _NumeroFactura = "";
            _CodigoDocumento = "";
            Articulos.Clear();
            _AlmacenID = 0;
            _DetalleCompra = "";
            _TerceroID = 0;
            txtSubTotal.Text = "$ 0.00";
            txtBase19.Text= "$ 0.00";
            txtBase5.Text= "$ 0.00";
            txtIVA19.Text= "$ 0.00";
            txtIVA5.Text= "$ 0.00";
            txtReteFuente.Text= "$ 0.00";
            txtExento.Text= "$ 0.00";
            txtTotalGeneralIVA.Text= "$ 0.00";
            txtTotalCompra.Text= "$ 0.00";

        }

        private void ActivarBotones()
        {
            btnCerrar.IsEnabled= true;
            btnEditar.IsEnabled= true;
            btnGuardar.IsEnabled= true;
            btnNuevo.IsEnabled= true;
            btnEliminar.IsEnabled= true;
            btnCrearCompra.IsEnabled = true;
        }
        private void LlenarVendedor()
        {
            var RVendedor = new VendedoresRepositorio(_ConexionSql);
            if (_VendedorID == null)
            {
                DataTable dt = RVendedor.GenerarVendedores();
                cbxDigitador.DisplayMemberPath = "INFOVENDEDOR";
                cbxDigitador.SelectedValuePath = "VendedorID";
                cbxDigitador.ItemsSource = dt.DefaultView;
            }
            else
            {
                DataTable dt = RVendedor.GenerarInfoVendedorUnitario(_VendedorID);
                cbxDigitador.DisplayMemberPath = "INFOCOMPLETA";
                cbxDigitador.SelectedValuePath = "VendedorID";
                cbxDigitador.ItemsSource = dt.DefaultView;
                cbxDigitador.SelectedValue = _VendedorID;
            }
        }

        private void TxtDocumento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                VentanaBuscadorDocumentos ventana = new VentanaBuscadorDocumentos(2);
                bool? Documento = ventana.ShowDialog();
                if (Documento == true && ventana.DocumentoSeleccionado != null)
                {
                    _DocumentoID = ventana.DocumentoSeleccionado.DocumentoID;
                    TxtDocumento.Text = ventana.DocumentoSeleccionado.DocumentoCodigo;
                    TxtNombreDocto.Text = ventana.DocumentoSeleccionado.DocumentoNombre;
                    _DocumentoID = ventana.DocumentoSeleccionado.DocumentoID;
                    _CodigoDocumento = ventana.DocumentoSeleccionado.DocumentoCodigo;
                    TxtNombreDocto.IsEnabled = false;
                    txtNumeroFactura.Focus();
                }
            }
            if (e.Key == Key.Enter)
            {
                if (e.Key == Key.Enter)
                {
                    string Codigo = TxtDocumento.Text;
                    if (string.IsNullOrEmpty(Codigo))
                    {
                        MessageBox.Show("Debe Seleccionar Documento", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        TxtNombreDocto.Focus();
                        return;
                    }
                    var RDocumento = new DocumentosVentaRespositorio(_ConexionSql);
                    DataTable Dt = RDocumento.BuscarDocumentoXCodigo(Codigo);
                    if (Dt.Rows.Count > 0)
                    {
                        TxtDocumento.Text = Dt.Rows[0]["CodigoDocumento"].ToString();
                        TxtNombreDocto.Text = Dt.Rows[0]["NombreDocumento"].ToString();
                        _DocumentoID = Convert.ToInt32(Dt.Rows[0]["DocumentoID"]);
                        _CodigoDocumento = Dt.Rows[0]["CodigoDocumento"].ToString();
                        txtNumeroFactura.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Codigo Documento No Exite", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
                        TxtDocumento.Focus();
                        return;
                    }
                }
            }
        }
        private void btnBuscarProveedor_Click(object sender, RoutedEventArgs e)
        {
            VentanaBuscarTerceros ventana = new VentanaBuscarTerceros();
            bool? BuscarTercero = ventana.ShowDialog();
            if (BuscarTercero != null && ventana.TerceroSelecionado != null)
            {
                txtProveedor.Text = (ventana.TerceroSelecionado.TercerosIdentificacion + "  " + ventana.TerceroSelecionado.TerceroNobres);
                _TerceroID = ventana.TerceroSelecionado.TerceroID;
                TxtDocumento.IsEnabled = true;
                txtObservaciones.Focus();
            }
        }

        private void txtProveedor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string CcNit = txtProveedor.Text;
                if (string.IsNullOrEmpty(CcNit))
                {
                    MessageBox.Show("Digite Nit Del Proveedor", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtProveedor.Focus();
                    return;
                }
                var RDocumentos = new RepositorioBuscarDocumentos(_ConexionSql);
                DataTable dt = RDocumentos.BuscarCCNit(CcNit);
                if (dt.Rows.Count > 0)
                {
                    string TipoDoc = dt.Rows[0]["TercerosTipoDocumento"].ToString();
                    if (TipoDoc == "13")
                    {
                        string CC = dt.Rows[0]["TercerosIdentificacion"].ToString();
                        string Nombre = dt.Rows[0]["TercerosNombreCompleto"].ToString();
                        txtProveedor.Text = CC + "  " + Nombre;
                        _TerceroID = Convert.ToInt32(dt.Rows[0]["TercerosID"]);
                        txtObservaciones.Focus();


                    }
                    else
                    {
                        string NIT = dt.Rows[0]["TercerosIdentificacion"].ToString();
                        string Nombre = dt.Rows[0]["TerceroRazonSocial"].ToString();
                        txtProveedor.Text = NIT + "  " + Nombre;
                        _TerceroID = Convert.ToInt32(dt.Rows[0]["TercerosID"]);
                        txtObservaciones.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Proveedor No Existe", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtProveedor.Focus();
                    return;
                }
            }
        }

        private void txtNumeroDocumento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtNumeroFactura.Focus();
            }
        }

        private void txtNumeroFactura_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                dpFecha.Focus();
            }
        }

        private void txtObservaciones_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnCrearCompra.Focus();
            }
        }

        private void dgProductos_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            // Cuando se agrega una nueva fila, baja el scroll automáticamente
            dgProductos.ScrollIntoView(e.Row.Item);
        }

        private void dpFecha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtProveedor.Focus();
            }
        }
        #endregion

        #region --- Botones----

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            CerrarVentana();
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            DesbloquearCampos();
            LimpiarCampos();
            TxtDocumento.Focus();
            btnEditar.IsEnabled = false;
            btnEliminar.IsEnabled = false;
            btnNuevo.IsEnabled = false;
        }


        private void btnCrearCompra_Click(object sender, RoutedEventArgs e)
        {
            string Documento = TxtDocumento.Text;
            string Numero = txtNumeroDocumento.Text;
            string Factura = txtNumeroFactura.Text;
            _NumeroFactura = Factura;

            if (string.IsNullOrEmpty(Documento))
            {
                MessageBox.Show("Debe Seleccionar Documento", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtDocumento.Focus();
                return;
            }

            if (_VendedorID == 0)
            {
                MessageBox.Show("Debe Seleccionar Vendedor", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpFecha.Focus();
                return;
            }

            if (string.IsNullOrEmpty(_DetalleCompra))
            {
                _DetalleCompra = "Compra";
            }

            var RCompras = new RepositorioCompras(_ConexionSql);
            _NumeroCompra = RCompras.GenerarConsecutivoCompras(_CodigoDocumento);
            txtNumeroDocumento.Text = _NumeroCompra.ToString();
            btnCrearCompra.IsEnabled = false;
            txtNumeroDocumento.IsEnabled = false;
            _CompraID = RCompras.InsertCompra(_DocumentoID, _NumeroCompra, _NumeroFactura, _TerceroID, _VendedorID, _FechaCompra,
                _DetalleCompra);
            dgProductos.IsEnabled = true;
            Articulos.Add(new ArticulosAgregados());
            btnCerrar.IsEnabled = false;

        }

        private void dpFecha_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                _FechaCompra = dpFecha.SelectedDate.Value;
            }
            catch
            {
                throw;
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (Articulos.Count > 0 && _CompraID!=0)
            {
                btnGuardar.IsEnabled = true;
                VentanaSelecionarFormaPago Ven = new VentanaSelecionarFormaPago(1, Convert.ToDouble(Articulos.Sum(A => A.TotalConIVA)));
                bool? FormaPago = Ven.ShowDialog();
                if (FormaPago == true && Ven.FormaPagoIDFinal != 0)
                {
                    var Rcompras = new RepositorioGuardaPagos(_ConexionSql);
                    Rcompras.InsertarGuardaPagos(Ven.FormaPagoIDFinal, _DocumentoID, _NumeroCompra, _FechaCompra, _VendedorID,
                        Articulos.Sum(A => A.TotalConIVA));
                    Ven.Close();
                    Imprimir();
                    _CompraID = 0;
                    LimpiarCampos();
                    dgProductos.Items.Refresh();
                    ActivarBotones();
                }
            }
            else
            {
                MessageBox.Show("No Hay Articulos Agregados", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Imprimir()
        {
            var Pregunta = MessageBox.Show("¿ Imprimir Comprobante ?", "Mensaje", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (Pregunta == MessageBoxResult.Yes)
            {
                ImpresionDocumentos Ventana = new ImpresionDocumentos(1,_CompraID);
                Ventana.ShowDialog();
            }
        }
        #endregion

        #region Logica Compras

        public ObservableCollection<ArticulosAgregados> Articulos { get; set; }

        public class ArticulosAgregados : INotifyPropertyChanged
        {
            string _ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public event EventHandler<CantidadChangedEventArgs> CantidadCambiada;

            public event EventHandler ActualizarTotales;
            public class CantidadChangedEventArgs : EventArgs
            {
                public decimal CantidadAnterior { get; }
                public decimal CantidadNueva { get; }

                public CantidadChangedEventArgs(decimal anterior, decimal Nueva)
                {
                    CantidadAnterior = anterior;
                    CantidadNueva = Nueva;
                }
            }



            private bool _Actualizando = false;

            private int _ArticuloID;
            public int ArticulosID
            {
                get { return _ArticuloID; }
                set
                {
                    if (_ArticuloID != value)
                    {
                        _ArticuloID = value;
                        OnPropertyChanged();
                    }
                }
            }

            private int _Items;
            public int Items
            {
                get => _Items;
                set
                {
                    if (_Items != value)
                    {
                        _Items = value;
                        OnPropertyChanged();
                    }
                }
            }

            private string _CodigoArticulo;
            public string CodigoArticulo
            {
                get { return _CodigoArticulo; }
                set
                {
                    if (_CodigoArticulo != value)
                    {
                        _CodigoArticulo = value;
                        OnPropertyChanged();
                    }
                }
            }

            private string _NombreArticulo;
            public string NombreArticulo
            {
                get => _NombreArticulo;
                set
                {
                    if (_NombreArticulo != value)
                    {
                        _NombreArticulo = value;
                        OnPropertyChanged();
                    }
                }
            }

            private int _AlmacenID;

            public int AlmacenID
            {
                get => _AlmacenID;
                set
                {
                    if (_AlmacenID != value)
                    {
                        _AlmacenID = value;
                        OnPropertyChanged();
                    }
                }
            }

            private string _CodigoAlmacen;
            public string CodigoAlmacen
            {
                get => _CodigoAlmacen;
                set
                {
                    if (_CodigoAlmacen != value)
                    {
                        _CodigoAlmacen = value;
                        OnPropertyChanged();
                    }
                }
            }



            private decimal _Cantidad;
            public decimal Cantidad
            {
                get => _Cantidad;
                set
                {
                    if (_Cantidad != value)
                    {
                        decimal CantidadOld = _Cantidad;
                        _Cantidad = value;
                        OnPropertyChanged();
                        RecalcularTotales();
                        CantidadCambiada?.Invoke(this, new CantidadChangedEventArgs(CantidadOld, _Cantidad));
                    }
                }
            }

            private decimal _CostoSinIva;
            public decimal CostoSinIva
            {
                get => _CostoSinIva;
                set
                {
                    if (_CostoSinIva != value)
                    {
                        _CostoSinIva = value;
                        OnPropertyChanged();
                        if (!_Actualizando)
                        {
                            _Actualizando = true;
                            var Ope = new RepositorioOperacionesMatematicas();
                            CostoConIVA = Math.Round(Ope.CalcularValorMasIva(_CostoSinIva, _IvaCompra));
                            RecalcularTotales();
                            _Actualizando = false;
                        }
                    }
                }
            }

            private decimal _CostoConIva;
            public decimal CostoConIVA
            {
                get => _CostoConIva;
                set
                {
                    if (_CostoConIva != value)
                    {
                        _CostoConIva = value;
                        OnPropertyChanged();
                        if (!_Actualizando)
                        {
                            _Actualizando = true;
                            var Ope = new RepositorioOperacionesMatematicas();
                            CostoSinIva = Math.Round(Ope.CalcularValirSinIva(_CostoConIva, _IvaCompra));
                            RecalcularTotales();
                            _Actualizando = false;
                        }
                    }
                }
            }
            private decimal _IvaCompra;
            public decimal IvaCompra
            {
                get => _IvaCompra;
                set
                {
                    if (_IvaCompra != value)
                    {
                        _IvaCompra = value;
                        OnPropertyChanged();
                    }
                }
            }

            private decimal _TotalSinIVA;
            public decimal TotalSinIVA
            {
                get => _TotalSinIVA;
                set
                {
                    if (_TotalSinIVA != value)
                    {
                        _TotalSinIVA = value;
                        OnPropertyChanged();
                    }
                }
            }

            private decimal _TotalConIVA;
            public decimal TotalConIVA
            {
                get => _TotalConIVA;
                set
                {
                    if (_TotalConIVA != value)
                    {
                        _TotalConIVA = value;
                        OnPropertyChanged();
                    }
                }
            }

            private int _CompraDetalleID;
            public int CompraDetalleID
            {
                get { return _CompraDetalleID; }
                set
                {
                    if (_CompraDetalleID != value)
                    {
                        _CompraDetalleID = value;
                        OnPropertyChanged();
                    }
                }
            }

            private decimal _CostoAnterior;
            public decimal CostoAnterior
            {
                get => _CostoAnterior;
                set
                {
                    if (_CostoAnterior != value)
                    {
                        _CostoAnterior = value;
                        OnPropertyChanged();
                    }
                }
            }
            public int Estado { get; set; }
            public string Detalle { get; set; }
            public decimal PorDescuento { get; set; }
            public decimal VrDescuento { get; set; }
            public decimal PrecioVenta { get; set; }
            public decimal Margen { get; set; }
            public decimal Incremento { get; set; }

            //Constructor vacio
            public ArticulosAgregados() { }

            public void RecalcularTotales()
            {
                TotalConIVA = (_CostoConIva * _Cantidad);
                TotalSinIVA = (_CostoSinIva * _Cantidad);
                ActualizarTotales?.Invoke(this, EventArgs.Empty);
            }
        }

        private void dgProductos_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (dgProductos.CurrentCell == null || dgProductos.CurrentCell.Column == null) return;

            var columna = dgProductos.CurrentCell.Column as DataGridBoundColumn;
            if (columna == null) return;

            var binding = columna.Binding as Binding;
            string NombreDelBinding = binding?.Path?.Path;
            if (string.IsNullOrEmpty(NombreDelBinding)) return;

            var RCompras = new RepositorioCompras(_ConexionSql);
            var Item = dgProductos.CurrentItem as ArticulosAgregados;

            switch (e.Key)
            {
                case Key.F1:
                    if (NombreDelBinding == "CodigoArticulo")
                    {
                        if (Item != null)
                        {
                            VentanaBuscarArticulos Ven = new VentanaBuscarArticulos();
                            bool? Articulo = Ven.ShowDialog();
                            if (Articulo == true && Ven.Articulosseleccionado != null)
                            {
                                Item.Items = Articulos.Count;
                                Item.ArticulosID = Ven.Articulosseleccionado.ArticuloID;
                                Item.CodigoArticulo = Ven.Articulosseleccionado.Codigo;
                                Item.NombreArticulo = Ven.Articulosseleccionado.Nombre;
                                Item.IvaCompra = Ven.Articulosseleccionado.TarifaIVA;
                                Item.CostoSinIva = Math.Round(Ven.Articulosseleccionado.CostoSinIva);
                                Item.CostoAnterior = Ven.Articulosseleccionado.CostoSinIva;
                                Item.PrecioVenta = Ven.Articulosseleccionado.ValorVenta;
                                Item.Margen = Ven.Articulosseleccionado.Margen;
                                Item.Incremento = Ven.Articulosseleccionado.Incremento;
                            }

                            Item.CantidadCambiada += Items_CantidadCambiada;
                        }
                    }
                    break;

                case Key.Enter:
                    e.Handled = true;

                    if (NombreDelBinding == "CodigoAlmacen")
                    {
                        VentanaSeleccionarAlmacen Ven = new VentanaSeleccionarAlmacen(Item.ArticulosID);
                        bool? Bodega = Ven.ShowDialog();
                        if (Bodega == true && Ven.AlmacenSeleccionado != null)
                        {
                            Item.AlmacenID = Ven.AlmacenSeleccionado.AlmacenID;
                            Item.CodigoAlmacen = Ven.AlmacenSeleccionado.CodigoAlmacen;
                        }
                    }

                    if (NombreDelBinding == "Cantidad")
                    {
                        var Cell = dgProductos.CurrentCell;
                        var EditingElement = dgProductos.CurrentColumn.GetCellContent(Cell);

                        // Forzamos a que se actualice primero el binding 
                        if (EditingElement is TextBox tb && tb.GetBindingExpression(TextBox.TextProperty) != null)
                        {
                            tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                        }

                        if (Item.Cantidad == 0)
                        {
                            MessageBox.Show("Digite Cantidad", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        //El estado  es para cuando se inserte en la tabla se pueda de el codigo lo sepa
                        if (Item.Estado == 0)
                        {
                            int CompraDetalle = RCompras.InsertaCompraDetalle(
                            _CompraID,
                            Item.ArticulosID,
                            Item.AlmacenID,
                            Item.Cantidad,
                            Item.CostoAnterior,
                            Item.CostoSinIva,
                            Item.CostoConIVA,
                            Item.IvaCompra,
                            Item.Detalle,
                            Item.PorDescuento,
                            Item.VrDescuento,
                            _CantDv);
                            Item.CompraDetalleID = CompraDetalle;
                            RCompras.InsertaSaldoArticulo(Item.ArticulosID, Item.AlmacenID, Item.Cantidad, Item.CostoSinIva);
                            Item.Estado = 1;
                            RCompras.InsertarMoviminetoArticulos(_FechaCompra, _TipoMovimiento, _DocumentoID, _NumeroCompra, Item.ArticulosID, Item.AlmacenID,
                                Item.Cantidad, Item.CostoSinIva, Item.CompraDetalleID, _VendedorID, _DetalleMovInventario);
                        }
                    }

                    if (NombreDelBinding == "CostoSinIva")
                    {
                        var cell = dgProductos.CurrentCell;
                        var EditingRowColumn = dgProductos.CurrentColumn.GetCellContent(cell.Item);

                        //Formazamos a digitar actualizar el binding
                        if (EditingRowColumn is TextBox tb && tb.GetBindingExpression(TextBox.TextProperty) != null)
                        {
                            tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                        }
                        RCompras.ActualizarCostoCompraDetalle(Item.CostoSinIva, Item.CostoConIVA, Item.CostoAnterior, Item.CompraDetalleID);
                    }

                    if (NombreDelBinding == "CostoConIVA")
                    {
                        var cell = dgProductos.CurrentCell;
                        var EditingCell = dgProductos.CurrentColumn.GetCellContent(cell.Item);

                        if (EditingCell is TextBox tb && tb.GetBindingExpression(TextBox.TextProperty) != null)
                        {
                            tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                        }
                        RCompras.ActualizarCostoCompraDetalle(Item.CostoSinIva, Item.CostoConIVA, Item.CostoAnterior, Item.CompraDetalleID);

                        VentanaActualizarCostosCompra Ven = new VentanaActualizarCostosCompra(
                            Item.ArticulosID,
                            Item.CostoAnterior,
                            Item.CostoSinIva,
                            Item.CostoConIVA,
                            Item.IvaCompra,
                            Item.Margen,
                            Item.Incremento,
                            Item.PrecioVenta
                        );

                        Ven.ShowDialog();
                    }

                    if (dgProductos.CurrentCell != null)
                    {
                        var grid = (DataGrid)sender; // CASTEO Y Obtengo el Grid
                        var Columna = grid.CurrentCell;
                        int CurrenColumnIndex = grid.Columns.IndexOf(Columna.Column); // obtengo la columna
                        int CurrentRowIndex = grid.Items.IndexOf(grid.CurrentItem);    // obtengo el row

                        grid.CommitEdit(DataGridEditingUnit.Cell, true);
                        grid.CommitEdit(DataGridEditingUnit.Row, true);

                        if (CurrenColumnIndex < grid.Columns.Count - 1)
                        {
                            // Mover a la siguiente columna en la misma fila
                            grid.CurrentCell = new DataGridCellInfo(grid.Items[CurrentRowIndex],
                                grid.Columns[CurrenColumnIndex + 1]);
                        }
                        else
                        {
                            // Si es la última columna
                            if (CurrentRowIndex == grid.Items.Count - 1)
                            {
                                //  Agregar nueva fila al final
                                var nuevo = new ArticulosAgregados();
                                nuevo.Items = Articulos.Count + 1;
                                Articulos.Add(nuevo);
                                grid.Items.Refresh();
                            }

                            // Mover el foco a la primera columna de la siguiente fila (nueva o existente)
                            grid.CurrentCell = new DataGridCellInfo(grid.Items[Math.Min(CurrentRowIndex + 1, grid.Items.Count - 1)],
                                grid.Columns[0]);
                        }

                        // Activar edición
                        grid.BeginEdit();

                    }
                    break;

                case Key.F5:

                    if (NombreDelBinding == "CodigoArticulo")
                    {
                        VentanaEliminaArticulos ventana = new VentanaEliminaArticulos();
                        bool? Articulo = ventana.ShowDialog();
                        if (Articulo == true)
                        {
                            int Desde = ventana.ArtDesde;
                            int Hasta = ventana.ArtHasta;

                            var ItemsEliminar = Articulos.Where(A => A.Items >= Desde && A.Items <= Hasta).ToList();

                            foreach (var art in ItemsEliminar)
                            {
                                RCompras.ElimianrRegistrosCompraDetalle(_CompraID, art.ArticulosID, art.Cantidad);
                                RCompras.EliminarMovInventarios(art.CompraDetalleID);
                                RCompras.ActualizarSaldoArticulosEliminados(art.ArticulosID, art.Cantidad);
                                Articulos.Remove(art);

                            }
                            int Contador = 1;
                            foreach (var items in Articulos)
                            {
                                items.Items = Contador++;
                            }
                            dgProductos.Items.Refresh();
                            CalcularTotales();
                        }
                    }
                    break;
            }
        }


        private void Items_CantidadCambiada(object sender, ArticulosAgregados.CantidadChangedEventArgs e)
        {
            var RCompras = new RepositorioCompras(_ConexionSql);
            var item = sender as ArticulosAgregados;
            if (item == null) return;

            // Si el artículo no tiene aún un registro de detalle, no hacemos nada.
            if (item.CompraDetalleID == 0)
                return;

            decimal diferencia = e.CantidadNueva - e.CantidadAnterior;

            // Si la diferencia es positiva, se aumentó la cantidad
            if (diferencia > 0)
            {
                RCompras.ActualizarCantidadSaldo(1, item.ArticulosID, diferencia, item.AlmacenID);
            }
            //  Si la diferencia es negativa, se disminuyó la cantidad
            else if (diferencia < 0)
            {
                RCompras.ActualizarCantidadSaldo(2, item.ArticulosID, diferencia, item.AlmacenID);
            }

            // Actualiza totales y detalle
            RCompras.ActualizarCantidadCompraDetalleID(item.CompraDetalleID, item.Cantidad);
            RCompras.ActualizarCantidadMovInventario(item.CompraDetalleID, item.Cantidad);
        }

        #endregion

        #region TotalesCompras

        private void Articulos_CollectionChanged(object Sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ArticulosAgregados art in e.NewItems)
                {
                    art.ActualizarTotales += Articulos_TotalesCambiaron;
                }
            }
        }

        private void Articulos_TotalesCambiaron(object sender, EventArgs e)
        {
            CalcularTotales();
        }

        //Resumne:
        //      el calculos de los taoles como corresponde a una clase diferente a la clase del data grid y el evento calcular totales debe ir 
        //      dentro del inotifypropertychanged de la clase enlazada al data grid , lo que hago es crear el evento y suscribirlo primero
        //      a cada elemeto de lista y luego lo enlazo a  Articulos_TotalesCambiaron que tiene el metodo calculartotales
        public void CalcularTotales()
        {
            var ROperaciones = new RepositorioOperacionesMatematicas();

            string Signo = "$";
            decimal Base19 = Articulos.Where(A => A.IvaCompra == 19).Sum(A => A.TotalSinIVA);
            decimal Base5 = Articulos.Where(A => A.IvaCompra == 5).Sum(A => A.TotalSinIVA);
            decimal BaseExenta = Articulos.Where(A => A.IvaCompra == 0).Sum(A => A.TotalSinIVA);
            decimal TotalConiva = Articulos.Sum(A => A.TotalConIVA);
            decimal Iva19 = ROperaciones.CalcularValorIva(Base19, 19);
            decimal Iva5 = ROperaciones.CalcularValorIva(Base5, 5);
            decimal SubTotal = Base19 + Base5 + BaseExenta;
            decimal TotalIva = Iva19 + Iva5;
            decimal Descuento = 0;
            decimal RteFuente = 0;

            txtBase19.Text = Signo + (Base19).ToString("N2");
            txtBase5.Text = Signo + Base5.ToString("N2");
            txtExento.Text = Signo + BaseExenta.ToString("N2");
            txtIVA19.Text = Signo + Iva19.ToString("N2");
            txtIVA5.Text = Signo + Iva5.ToString("N2");
            txtSubTotal.Text = Signo + SubTotal.ToString("N2");
            txtTotalGeneralIVA.Text = Signo + TotalIva.ToString("N2");
            txtTotalCompra.Text = Signo + TotalConiva.ToString("N2");
            var Rcompras = new RepositorioCompras(_ConexionSql);
            Rcompras.InsertCompraTotales(_CompraID, SubTotal, BaseExenta,Base19, Base5, Iva19, Iva5, Descuento, RteFuente,TotalConiva);
        }


        #endregion

        
    }
}
