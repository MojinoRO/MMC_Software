using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using MMC_Software;
using System.Data;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Web.UI.WebControls.WebParts;
using System.Printing;
using CrystalDecisions.CrystalReports.ViewerObjectModel;
using System.Windows.Controls.Primitives;
using System.Globalization;
using MMC_Software.Helpers;


namespace MMC_Software
{
    public class VentanaRegistroVentaViewModel : INotifyPropertyChanged
    {
        public static readonly string _ConexinSql= ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
        public static readonly RepositorioOperacionesMatematicas ROpe = new RepositorioOperacionesMatematicas();
        public static readonly DocumentosVentaRespositorio RDocVenta = new DocumentosVentaRespositorio(_ConexinSql);
        public static readonly RepositorioBuscarTercero RTercero = new RepositorioBuscarTercero(_ConexinSql);
        private readonly IDialogService _dialogService;
        private string _ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
        public TotalesVenta Totales { get; set; } = new TotalesVenta();
        public FormaPagoVenta FormaPago { get; set; } = new FormaPagoVenta();

        public VentanaRegistroVentaViewModel() : this(new DialogService()) { }// <== InstanciarPorDefecto
        public VentanaRegistroVentaViewModel(IDialogService dialogService)
        {
            BtnCrearVenta = true;
            dgListadoArticulos = false;
            _dialogService = dialogService;
            BuscarDocumento = new RelayCommand(EjecutarBuscarDocumento);
            BuscarTercero = new RelayCommand(EjecutarBuscarCliente);
            CSaleDate = new RelayCommand(EjecutarActualizarFecha);
            CmdCrearEncabezadoVenta = new RelayCommand(CrearEncabezadoVenta);
            CmdDetallecomprasEnter = new RelayCommand(DetallecomprasEnter);
            DigitarCodigoManual = new RelayCommand(DigitarCodigoDoctoManual);
            CmdClienteEnter = new RelayCommand(DigitarCedulaClienteManual);
            ListadoArticulos.CollectionChanged += ListadoArticulos_CollectionChanged;
            CmdSaveSale = new RelayCommand(GuardarVenta);

        }

        //esto es para que todos los camandos que queira exponer  a la vista deben ser publicos para la vista del MVVM  
        public ICommand BuscarDocumento { get; }
        public ICommand BuscarTercero { get; }
        public ICommand CSaleDate { get; }
        public ICommand CmdCrearEncabezadoVenta { get; }
        public ICommand CmdDetallecomprasEnter { get; }
        public ICommand EjecutarBuscarDocumentoEnter { get;}
        public ICommand DigitarCodigoManual { get; }
        public ICommand CmdClienteEnter { get; }
        public ICommand CmdSaveSale { get; }


        public void GuardarVenta(object obj)
        {
            if(VentaID > 0)
            {
                decimal TotalVenta = ListadoArticulos.Sum(a => a.TotalConIva);
                var Ven = _dialogService.BuscarFormasPagos(2, TotalVenta);
                
                if(Ven.ID > 0)
                {
                    FormaPago.FormaPagoID = Ven.ID;
                    FormaPago.CodigoFormaPago = Ven.Codigo;
                    FormaPago.NombreFormaPago = Ven.Nombre;
                    FormaPago.ValorFormaPago = Ven.Valor;
                }
            }
        }

        public void DetallecomprasEnter(object obj)
        {
            SolicitudFoco("btnAgregarProductoVenta");
        }

        public void CrearEncabezadoVenta(object obj)
        {
            if (VentaID == 0)
            {
                if (DocumentoID == 0)
                {
                    MessageBox.Show("Error Debe Seleccionar Documento De Venta", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (VendedorID == 0)
                {
                    MessageBox.Show("Error Debe Seleccionar Vendedor", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (SaleDate == DateTime.MinValue)
                {
                    MessageBox.Show("Error, Revisa Fecha Valida", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(_ConexionSql))
                {
                    conn.Open();
                    SqlTransaction Trans = conn.BeginTransaction();
                    try
                    {
                        var Rsale = new RepositorySale(Trans);
                        int Consecutivo = Rsale.GenerarConsecutivoVentas(CodigoDocumento);
                        NumeroVenta = Consecutivo;
                        if (Consecutivo > 0)
                        {
                            int VentaIDNew = Rsale.InsertarEncabezadoVentas(DocumentoID, Consecutivo, SaleDate, VendedorID, TerceroID, DetalleVenta);
                            VentaID = VentaIDNew;


                        }
                        Trans.Commit();
                        BtnCrearVenta = false;
                        dgListadoArticulos = true;
                        ListadoArticulos.Add(new ArticulosVenta
                        {
                            Item = 1
                        });
                        PedirFoco(0, 0);


                    }
                    catch (Exception ex)
                    {
                        Trans.Rollback();
                        MessageBox.Show("Error Al Crear Venta" + ex.ToString(), "Mensaje", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                }
            }
        }

        public void DigitarCodigoDoctoManual(object obj)
        {
            DataTable dt = RDocVenta.BuscarDocumentoXCodigoVentas(CodigoDocumento);
            if (dt.Rows.Count > 0)
            {
                CodigoDocumento=dt.Rows[0]["CodigoDocumento"].ToString();
                NombreDocumento= dt.Rows[0]["NombreDocumento"].ToString();
                DocumentoID= Convert.ToInt32( dt.Rows[0]["DocumentoID"]);
                SolicitudFoco("dpfecha");
                AbrirFecha = true;
            }
            else
            {
                MessageBox.Show("Error Documento No Existe", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Error);
                CodigoDocumento = string.Empty;
                NombreDocumento = string.Empty;
                DocumentoID = 0;
            }
        }                               

        public void DigitarCedulaClienteManual(object obj)
        {
            DataTable dt = RTercero.BuscarCedulaUnitario(Identificacion);
            if(dt.Rows.Count > 0)
            {
                TerceroID = Convert.ToInt32(dt.Rows[0]["TercerosID"]);
                Identificacion = dt.Rows[0]["TercerosIdentificacion"].ToString() + "  " + dt.Rows[0]["Nombre"].ToString();
                SolicitudFoco("TxtObservacionesVenta");
            }
            else
            {
                MessageBox.Show("Error Cliente No Existe", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Error);
                TerceroID = 0;
                Identificacion = string.Empty;
            }
        }
        public void EjecutarActualizarFecha(object obj)
        {
            if (VentaID > 0)
            {
                var RSale = new RepositorySale(_ConexionSql);
                RSale.ActualizarFechaVenta(_VentaID, SaleDate);

            }
            SolicitudFoco("TxtClienteVenta");
        }


        public void EjecutarBuscarDocumento(object obj)
        {
            var Ven = _dialogService.BuscarDocumento();
            if (Ven != null)
            {
                CodigoDocumento = Ven.DocumentoCodigo;
                NombreDocumento = Ven.DocumentoNombre;
                DocumentoID = Ven.DocumentoID;

                SolicitudFoco("dpfecha");
                AbrirFecha = true;
            }
        }

        public void EjecutarBuscarCliente(object obj)
        {
            var BuscarTercero = _dialogService.BuscarTercero();
            if (BuscarTercero != null)
            {
                TerceroID = BuscarTercero.TerceroID;
                Identificacion = BuscarTercero.TercerosIdentificacion + "  " + BuscarTercero.TerceroNobres;
                SolicitudFoco("TxtObservacionesVenta");
            }
            if (VentaID > 0 && TerceroID > 0)
            {
                var RSale = new RepositorySale(_ConexionSql);
                RSale.ActualizarClienteVenta(VentaID, TerceroID);
            }
        }


        public void BuscarArticulos()
        {
            var BuscarArticulo = _dialogService.BuscarArticulo();
            if (BuscarArticulo != null)
            {

            }
        }
        public void ProcesarTecla(Key Key)
        {
            KeyHandled = false;

            if (Key == Key.F1)
            {
                var Articulo = _dialogService.BuscarArticulo();
            }
        }

       

        private bool _KeyHandled;
        public bool KeyHandled
        {
            get => _KeyHandled;
            set
            {
                _KeyHandled = value;
                PropertyChangedOn(nameof(KeyHandled));
            }

        }


        private int _DocumentoID;
        public int DocumentoID
        {
            get => _DocumentoID;
            set
            {
                if (_DocumentoID != value)
                {
                    _DocumentoID = value;
                    PropertyChangedOn(nameof(DocumentoID));
                }
            }
        }


        private int _TipoDocumentoID;
        public int TipoDocumentoID
        {
            get => _TipoDocumentoID;
            set
            {
                if (_TipoDocumentoID != value)
                {
                    _TipoDocumentoID = value;
                    PropertyChangedOn(nameof(TipoDocumentoID));
                }
            }
        }
        private 
            
            string _CodigoDocumento;
        public string CodigoDocumento
        {
            get => _CodigoDocumento;
            set
            {
                if (_CodigoDocumento != value)
                {
                    _CodigoDocumento = value;
                    PropertyChangedOn(nameof(CodigoDocumento));
                }
            }
        }
        private string _NombreDocumento;
        public string NombreDocumento
        {
            get => _NombreDocumento;
            set
            {
                if (_NombreDocumento != value)
                {
                    _NombreDocumento = value;
                    PropertyChangedOn(nameof(NombreDocumento));
                }
            }
        }


        private int _NumeroVenta;
        public int NumeroVenta
        {
            get => _NumeroVenta;
            set
            {
                if (_NumeroVenta != value)
                {
                    _NumeroVenta = value;
                    PropertyChangedOn(nameof(NumeroVenta));
                }
            }
        }

        private int _TerceroID;
        public int TerceroID
        {
            get => _TerceroID;
            set
            {
                if (_TerceroID != value)
                {
                    _TerceroID = value;
                    PropertyChangedOn(nameof(TerceroID));
                }
            }
        }

        private string _Identificacion;
        public string Identificacion
        {
            get => _Identificacion;
            set
            {
                if (_Identificacion != value)
                {
                    _Identificacion = value;
                    PropertyChangedOn(nameof(Identificacion));
                }
            }
        }

        private int _VentaID;
        public int VentaID
        {
            get => _VentaID;
            set
            {
                if (_VentaID != value)
                {
                    _VentaID = value;
                    PropertyChangedOn(nameof(VentaID));
                }
            }

        }

        private DateTime _SaleDate = DateTime.Now;
        public DateTime SaleDate
        {
            get => _SaleDate;
            set
            {
                if (_SaleDate != value)
                {
                    _SaleDate = value;
                    PropertyChangedOn(nameof(SaleDate));
                }
            }
        }

        private int _VendedorID;
        public int VendedorID
        {
            get => _VendedorID;
            set
            {
                if (_VendedorID != value)
                {
                    _VendedorID = value;
                    PropertyChangedOn(nameof(VendedorID));
                }
            }
        }

        private string _DetalleVenta;
        public string DetalleVenta
        {
            get => _DetalleVenta;
            set
            {
                if (_DetalleVenta != value)
                {
                    _DetalleVenta = value;
                    PropertyChangedOn(nameof(DetalleVenta));
                }
            }
        }
        private bool _Modificando;
        public bool Modificando
        {
            get => _Modificando;
            set
            {
                if (_Modificando != value)
                {
                    _Modificando = value;
                    PropertyChangedOn(nameof(Modificando));
                }
            }
        }

        private bool _BtnCrearVenta;
        public bool BtnCrearVenta
        {
            get => _BtnCrearVenta;
            set
            {
                if (_BtnCrearVenta != value)
                {
                    _BtnCrearVenta = value;
                    PropertyChangedOn(nameof(BtnCrearVenta));
                }
            }
        }

        private bool _dgListadoArticulos;
        public bool dgListadoArticulos
        {
            get => _dgListadoArticulos;
            set
            {
                if (_dgListadoArticulos != value)
                {
                    _dgListadoArticulos = value;
                    PropertyChangedOn(nameof(dgListadoArticulos));
                }
            }
        }

        private bool _AbrirFecha;
        public bool AbrirFecha
        {
            get => _AbrirFecha;
            set
            {
                if (_AbrirFecha != value)
                {
                    _AbrirFecha = value;
                    PropertyChangedOn(nameof(AbrirFecha));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void PropertyChangedOn(string PropertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

        public event Action<string> SolicitudFoco;
        protected void PideFoco(string NombreControl)
        {
            SolicitudFoco?.Invoke(NombreControl);
        }

        public event Action<int, int> SolicitarFoco;
        protected void PedirFoco(int Row, int Column)
        {
            SolicitarFoco?.Invoke(Row, Column);
        }


        private void RecaltulatorValues()
        {

            // Base 19
            Totales.Base19 =CultureHelper.FormatearMoneda(
        Math.Round(ListadoArticulos
            .Where(a => a.IvaArticulo == 19)
            .Sum(a => a.TotalSinIva), 2));

            // Base 5
            Totales.Base5 = CultureHelper.FormatearMoneda(Math.Round(
                ListadoArticulos.Where(A => A.IvaArticulo == 5).Sum(A => A.TotalSinIva), 2));
            // Exento
            Totales.Exento = CultureHelper.FormatearMoneda(Math.Round(
                ListadoArticulos.Where(A => A.IvaArticulo == 0).Sum(A => A.TotalSinIva), 2));

            // IVA 19

            Totales.Iva19 = CultureHelper.FormatearMoneda(
                Math.Round(ROpe.CalcularValorIva(ListadoArticulos.Where(A=>A.IvaArticulo==19).Sum(A=>A.TotalSinIva),19)));

            //IVA 5
            Totales.Iva5 = CultureHelper.FormatearMoneda(
                Math.Round(ROpe.CalcularValorIva(ListadoArticulos.Where(A => A.IvaArticulo == 5).Sum(A => A.TotalSinIva), 5)));

            //Base Gravada
            Totales.BaseGravada = CultureHelper.FormatearMoneda(
                Math.Round(ListadoArticulos.Sum(A => A.TotalSinIva)));

            //total IVA
            Totales.TotalIva = CultureHelper.FormatearMoneda(
                Math.Round(ROpe.CalcularValorIva(ListadoArticulos.Where(A => A.IvaArticulo == 19).Sum(A => A.TotalSinIva), 19),2)+
                ROpe.CalcularValorIva(ListadoArticulos.Where(A => A.IvaArticulo == 5).Sum(A => A.TotalSinIva), 5));

            // TOTAL GENERAL DE LA VENTA (Con IVA)
            Totales.TotalVenta = CultureHelper.FormatearMoneda(
                Math.Round(ListadoArticulos.Sum(A => A.TotalConIva)));

        }


        private void ListadoArticulos_CollectionChanged(object sender , NotifyCollectionChangedEventArgs e)
        {
            //si se agregan articulos nuevos
            if(e.NewItems != null)
            {
                foreach(ArticulosVenta items in e.NewItems)
                {
                    // Escuchar cambios dentro del artículo
                    items.PropertyChanged += Articulo_PropertyChanged;
                }
            }
            // Si se eliminan artículos
            if (e.OldItems != null)
            {
                foreach (ArticulosVenta item in e.OldItems)
                {
                    item.PropertyChanged -= Articulo_PropertyChanged;
                }
            }

            // Actualiza totales generales cuando cambie la lista
            RecaltulatorValues();
        }

        private void Articulo_PropertyChanged(object sender , PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ArticulosVenta.TotalConIva)||
                e.PropertyName == nameof(ArticulosVenta.TotalSinIva))
            {
                RecaltulatorValues();
            }
        }
        public void ProcesingCantDigit(ArticulosVenta Item)
        {
            if (Item == null) return;

            try
            {
                if (Item.ArticuloID == 0)
                    throw new Exception("Debe seleccionar un artículo");

                if (Item.AlmacenID == 0)
                    throw new Exception("Debe seleccionar un almacén");

                if (Item.CantidadVenta <= 0)
                    throw new Exception("Cantidad debe ser mayor que 0");

                // Estos disparan OnPropertyChanged desde la clase ArticulosVenta
                Item.TotalSinIva = Math.Round(Item.ValorUnitario * Item.CantidadVenta, 2);
                Item.TotalConIva = Math.Round(Item.ValorMasIva * Item.CantidadVenta, 2);

                // Ejecutar transacción
                var service = new ServicesTransactionSale(_ConexinSql);
                int VentaDetalleID = service.TransacctionInsertSaleDatails(Item, this);

                // Asigna el ID generado
                Item.VentaDetalleID = VentaDetalleID;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar cantidad: " + ex.Message);
            }
        }


        #region ArticulosVenta

        /// <summary>
        /// CLASE DE ARTICULOS EN LA VENTA 
        /// </summary>


        public class ArticulosVenta : INotifyPropertyChanged
        {
            private static readonly RepositorioOperacionesMatematicas ROpe = new RepositorioOperacionesMatematicas();

            private int _Item;
            public int Item
            {
                get => _Item;
                set => Set(ref _Item, value);
            }

            private int _ArticuloID;
            public int ArticuloID
            {
                get => _ArticuloID;
                set => Set(ref _ArticuloID, value);
            }

            private string _CodigoArticulo;
            public string CodigoArticulo
            {
                get => _CodigoArticulo;
                set => Set(ref _CodigoArticulo, value);
            }

            private string _NombreArticulo;
            public string NombreArticulo
            {
                get => _NombreArticulo;
                set => Set(ref _NombreArticulo, value);
            }

            private int _AlmacenID;
            public int AlmacenID
            {
                get => _AlmacenID;
                set => Set(ref _AlmacenID, value);
            }

            private string _CodigoAlmacen;
            public string CodigoAlmacen
            {
                get => _CodigoAlmacen;
                set => Set(ref _CodigoAlmacen, value);
            }


            private decimal _CantidadOld;
            public decimal CantidadOld { get => _CantidadOld; 
                set => Set(ref _CantidadOld, value);
            }

            private decimal _CantidadVenta;
            public decimal CantidadVenta
            {
                get => _CantidadVenta;
                set
                {
                    if(_CantidadVenta != value)
                    {
                        CantidadOld= _CantidadVenta;
                        if (Set(ref _CantidadVenta, value))
                        {
                            CalcularTotales();
                        }
                    }
                }
            }

            private decimal _IvaArticulo;
            public decimal IvaArticulo
            {
                get => _IvaArticulo;
                set => Set(ref _IvaArticulo, value);
            }

            private decimal _ValorUnitario;
            public decimal ValorUnitario
            {
                get => _ValorUnitario;
                set
                {
                    if (Set(ref _ValorUnitario, value))
                    {
                        if (!_ActualizandoCosto)
                        {
                            _ActualizandoCosto = true;

                            ValorMasIva = Math.Round(ROpe.CalcularValorMasIva(ValorUnitario, IvaArticulo), 2);

                            CalcularTotales();
                            _ActualizandoCosto = false;
                        }
                    }
                }
            }

            private decimal _ValorMasIva;
            public decimal ValorMasIva
            {
                get => _ValorMasIva;
                set
                {
                    if (Set(ref _ValorMasIva, value))
                    {
                        if (!_ActualizandoCosto)
                        {
                            _ActualizandoCosto = true;

                            ValorUnitario = Math.Round(ROpe.CalcularValorMasIva(ValorMasIva, IvaArticulo), 2);

                            CalcularTotales();
                            _ActualizandoCosto = false;
                        }
                    }
                }
            }

            private decimal _TotalSinIva;
            public decimal TotalSinIva
            {
                get => _TotalSinIva;
                set => Set(ref _TotalSinIva, value);
            }

            private decimal _TotalConIva;
            public decimal TotalConIva
            {
                get => _TotalConIva;
                set => Set(ref _TotalConIva, value);
            }


            private decimal _ValorCosto;
            public decimal ValorCosto
            {
                get => _ValorCosto;
                set => Set(ref _ValorCosto, value);
            }


            private int _VentaDetalleID;
            public int VentaDetalleID   
            {
                get => _VentaDetalleID;
                set => Set(ref _VentaDetalleID, value);
            }


            private string _DetalleArticulo = "Venta";
            public string DetalleArticulo
            {
                get => _DetalleArticulo;
                set => Set(ref _DetalleArticulo, value);
            }


            public bool _ActualizandoCosto = false;

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged([CallerMemberName] string prop = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }

            private bool Set<T>(ref T campo, T valor, [CallerMemberName] string propiedad = null)
            {
                if (EqualityComparer<T>.Default.Equals(campo, valor)) return false;

                campo = valor;
                OnPropertyChanged(propiedad);
                return true;
            }

            private void CalcularTotales()
            {
                TotalSinIva = Math.Round(ValorUnitario * CantidadVenta,2);
                TotalConIva =Math.Round(ValorMasIva * CantidadVenta,2);
            }
        }

        #endregion


        /// <summary>
        /// OBSERVABLE COLLECTION LISTADO DE ARTICULOS EN LA VENTA
        /// </summary>
        private ObservableCollection<ArticulosVenta> _ListadoArticulos = new ObservableCollection<ArticulosVenta>();
        public ObservableCollection<ArticulosVenta> ListadoArticulos
        {
            get => _ListadoArticulos;
            set
            {
                if (_ListadoArticulos != value)
                {
                    _ListadoArticulos = value;
                    PropertyChangedOn(nameof(ListadoArticulos));
                }
            }
        }
    }

    #region TOTALES

    /// <summary>
    /// TOTALES DE LA VENTA 
    /// </summary>
    public class TotalesVenta : INotifyPropertyChanged
    {
        private string _Base19;
        public string Base19
        {
            get => _Base19;
            set => Set(ref _Base19, value);

        }

        private string _base5;
        public string Base5
        {
            get => _base5;
            set => Set(ref _base5, value);
        }

        private string _Exento;
        public string Exento
        {
            get => _Exento;
            set => Set(ref _Exento, value);
        }

        private string _Iva19;
        public string Iva19
        {
            get => _Iva19;
            set => Set(ref _Iva19, value);
        }

        private string _Iva5;
        public string Iva5
        {
            get => _Iva5;
            set => Set(ref _Iva5, value);
        }

        private string _TotalVenta;
        public string TotalVenta
        {
            get => _TotalVenta;
            set => Set(ref _TotalVenta, value);
        }

        private string _BaseGravada;
        public string BaseGravada
        {
            get => _BaseGravada;
            set => Set(ref _BaseGravada, value);
        }

        private string _TotalIva;
        public string TotalIva
        {
            get => _TotalIva;
            set => Set(ref _TotalIva, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        private bool Set<T>(ref T campo, T valor, [CallerMemberName] string propiedad = null)
        {
            if (EqualityComparer<T>.Default.Equals(campo, valor)) return false;

            campo = valor;
            OnPropertyChanged(propiedad);
            return true;
        }

        #endregion

    }

    public class FormaPagoVenta : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }  
        
        public int FormaPagoID { get; set; }
        public string CodigoFormaPago { get; set; }
        public string NombreFormaPago { get; set; }
        public decimal ValorFormaPago { get; set; }

        private bool Set<T>(ref T campo, T valor, [CallerMemberName] string propiedad = null)
        {
            if (EqualityComparer<T>.Default.Equals(campo, valor)) return false;

            campo = valor;
            OnPropertyChanged(propiedad);
            return true;
        }

    }

}
