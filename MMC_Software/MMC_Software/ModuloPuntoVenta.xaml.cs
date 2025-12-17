using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using static MMC_Software.VentanaConfigurarConsumidorFinal;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para ModuloPuntoVenta.xaml
    /// </summary>
    public partial class ModuloPuntoVenta : Window
    {
        private int? idTerceroSeleccionado = null;

        private double ValorTotalFactura;

        //lista para el buscador de articulos
        private List<string> BuscarArticulos = new List<string>();

        //guardar forma de pago
        private int FormaPagoIDseleccionado;

        private int NumeroConsecutivo;

        private string DetalleVenta = "";

        private int VentaID = 0; // 0 indica que aún no existe encabezado en BD

        //=============================
        //VARABLES DE CAMBIO DE FACTURA
        private double _Entregado;
        private double _Cambio;
        //=============================
        //variable para traer los datos de la ventanaabrepos

        private readonly int AlmacenID;
        private readonly string CodigoAlmacen;
        private readonly string nombreAlmacen;
        private readonly int VendedorID;
        private readonly string CodigoVendedor;
        private readonly string NombreVendedor;
        private readonly string fechaTexto;
        private readonly string CodigoDocto;
        private readonly string NombreDocto;
        private readonly int DocumentoID;

        // editar flujo
        private bool isEditing = false;
        private ArticulosAgregados editingArticulo = null;


        // Reloj 
        private DispatcherTimer _Hora;

        public ModuloPuntoVenta(int almacenID, string codigoAlmacen, string nombreAlmacen,
                                int vendedorID, string codigoVendedor, string nombreVendedor,
                                string fechaTexto, string CodigoDocto, string NombreDocto, int DocumentoID)
        {
            InitializeComponent();
            ListadoArticulos.Clear();
            AlmacenID = almacenID;
            CodigoAlmacen = codigoAlmacen;
            this.nombreAlmacen = nombreAlmacen;
            VendedorID = vendedorID;
            CodigoVendedor = codigoVendedor;
            NombreVendedor = nombreVendedor;
            this.fechaTexto = fechaTexto;
            this.CodigoDocto = CodigoDocto;
            this.NombreDocto = NombreDocto;
            this.DocumentoID = DocumentoID;
            LlenaCampos();
            LeerConsumudor();
            BloquearCampos();
            cmbArticuloBusqueda.Focus();
            montarTercero();
            IniciarReloj();
        }

        private void IniciarReloj()
        {
            _Hora = new DispatcherTimer();
            _Hora.Interval = TimeSpan.FromSeconds(1);
            _Hora.Tick += _HoraSalto;
            _Hora.Start();
        }

        private void _HoraSalto(object serder, EventArgs e)
        {
            TxtHora.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void BloquearCampos()
        {
            txtAlmacen.IsEnabled = false;
            txtVendedor.IsEnabled = false;
            dpFecha.IsEnabled = false;
            TxtNombreCompleto.IsEnabled = false;
            TxtDV.IsEnabled = false;
            TxtNombreDocumento.IsEnabled = false;
            txtNombreArticulo.IsEnabled = false;
            TxtPrecio.IsEnabled = false;
        }

        private void LlenaCampos()
        {
            txtAlmacen.Text = (CodigoAlmacen + " " + nombreAlmacen);
            txtVendedor.Text = (CodigoVendedor + " " + NombreVendedor);
            dpFecha.Text = fechaTexto;
            txtDocumento.Text = CodigoDocto;
            TxtNombreDocumento.Text = NombreDocto;
        }

        //------------------llenado de la forma de pago---------------------------------//
        private void montarTercero()
        {
            try
            {
                string CedulaTercero = TxtNitCC.Text.Trim();

                if (string.IsNullOrEmpty(CedulaTercero))
                {
                    MessageBox.Show("Debe Digitar Cedula O Nit Del Cliente", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                    using (SqlConnection conexion = new SqlConnection(ConexionSql))
                    {
                        conexion.Open();
                        string consulta = @"select TercerosID, TercerosTipoDocumento,TercerosIdentificacion,TercerosDV,TercerosNombreCompleto,TerceroRazonSocial,TercerosEmail 
                                        from InveTerceros 
                                        where TercerosIdentificacion =@filtro";
                        using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                        {
                            cmd.Parameters.AddWithValue("@filtro", CedulaTercero);
                            using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                            {
                                DataTable dataTable = new DataTable();
                                adp.Fill(dataTable);
                                if (dataTable.Rows.Count > 0)
                                {
                                    idTerceroSeleccionado = Convert.ToInt32(dataTable.Rows[0]["TercerosID"]);
                                    int tipodocumento = Convert.ToInt32(dataTable.Rows[0]["TercerosTipoDocumento"]);
                                    if (tipodocumento == 13)
                                    {
                                        TxtNitCC.Text = dataTable.Rows[0]["TercerosIdentificacion"].ToString();
                                        TxtNombreCompleto.Text = dataTable.Rows[0]["TercerosNombreCompleto"].ToString();
                                        TxtDV.Text = dataTable.Rows[0]["TercerosDV"].ToString();

                                    }
                                    else
                                    {
                                        TxtNitCC.Text = dataTable.Rows[0]["TercerosIdentificacion"].ToString();
                                        TxtNombreCompleto.Text = dataTable.Rows[0]["TerceroRazonSocial"].ToString();
                                        TxtDV.Text = dataTable.Rows[0]["TercerosDV"].ToString();
                                    }
                                }
                                else
                                {
                                    MessageBoxResult pregunta = MessageBox.Show("Cliente No Existe ,¿Desea Crearlo?", "MENSAJE", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                    if (pregunta == MessageBoxResult.Yes)
                                    {
                                        TxtNitCC.Text = "222222222222";
                                        VentanaCrearTerceros ventana = new VentanaCrearTerceros();
                                        ventana.Show();
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message);
            }
        }

        private void cmbArticuloBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                VentanaBuscarArticulos Ventana = new VentanaBuscarArticulos();
                bool? resultado = Ventana.ShowDialog();
                if (resultado == true && Ventana.Articulosseleccionado != null)
                {
                    cmbArticuloBusqueda.Text = Ventana.Articulosseleccionado.Codigo;
                    TxtPrecio.Text = Ventana.Articulosseleccionado.ValorVenta.ToString("N2");
                    txtNombreArticulo.Text = Ventana.Articulosseleccionado.Nombre.ToString();
                    TxtPrecio.IsEnabled = false;
                    txtNombreArticulo.IsEnabled = false;
                    txtCantidadBusqueda.Focus();
                }
            }
            else if (e.Key == Key.Enter)
            {
                try
                {
                    string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                    using (SqlConnection conexion = new SqlConnection(ConexionSql))
                    {
                        conexion.Open();
                        string consulta = @"Select  CodigoArticulo,NombreArticulo,ArticulosVenta from InveArticulos
                                            where ArticulosBarras=@filtro or CodigoArticulo=@filtro";
                        using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                        {
                            cmd.Parameters.AddWithValue("@filtro", cmbArticuloBusqueda.Text);
                            using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                            {
                                DataTable dt = new DataTable();
                                adp.Fill(dt);
                                if (dt.Rows.Count > 0)
                                {
                                    cmbArticuloBusqueda.Text = dt.Rows[0]["CodigoArticulo"].ToString();
                                    txtNombreArticulo.Text = dt.Rows[0]["NombreArticulo"].ToString();
                                    TxtPrecio.Text = dt.Rows[0]["ArticulosVenta"].ToString();
                                    txtCantidadBusqueda.Focus();
                                    txtNombreArticulo.IsEnabled = false;
                                    TxtPrecio.IsEnabled = false;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR" + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            if (e.Key == Key.F5)
            {
                VentanaClave ventanaClave = new VentanaClave(1, 0, 1);
                ventanaClave.Owner = this;
                bool? valida = ventanaClave.ShowDialog();
                if (valida == true)
                {
                    // eliminar rango de articulos (en BD y en la lista)
                    int ArticuloDesde = 0;
                    int ArticuloHasta = 0;

                    VentanaEliminaArticulos ventana = new VentanaEliminaArticulos();
                    bool? resultado = ventana.ShowDialog();
                    if (resultado != null)
                    {
                        ArticuloDesde = ventana.ArtDesde;
                        ArticuloHasta = ventana.ArtHasta;

                        // obtener lista de articulos a eliminar por ArticulosAgregados.ArticuloID (o por los Items seleccionados)
                        var aEliminar = ListadoArticulos.Where(n => n.Item >= ArticuloDesde && n.Item <= ArticuloHasta).ToList();
                        foreach (var art in aEliminar)
                        {
                            // eliminar en BD si existe encabezado
                            EliminarDetalleArticulo(art.ArticuloID);
                        }

                        ListadoArticulos.RemoveAll(n => n.Item >= ArticuloDesde && n.Item <= ArticuloHasta);
                        dgProductos.ItemsSource = null;
                        dgProductos.ItemsSource = ListadoArticulos;
                        int contador = 1;
                        foreach (var art in ListadoArticulos)
                        {
                            art.Item = contador++;
                        }
                        MuestraTotales();
                        cmbArticuloBusqueda.Focus();
                    }
                }
            }
            if (e.Key == Key.F4)
            {
                if (ListadoArticulos.Count > 0)
                {
                    int TipoDocumentoSeleccionado;
                    string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                    using (SqlConnection conexion = new SqlConnection(ConexionSql))
                    {
                        conexion.Open();
                        string consulta = "select TipoDocumento from ConfDocumentos where DocumentoID=@filtro";
                        using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                        {
                            cmd.Parameters.AddWithValue("@filtro", DocumentoID);
                            SqlDataAdapter adp = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adp.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                TipoDocumentoSeleccionado = Convert.ToInt32(dt.Rows[0]["TipoDocumento"]);
                                VentanaSelecionarFormaPago ventana = new VentanaSelecionarFormaPago(TipoDocumentoSeleccionado, ValorTotalFactura);
                                bool? resultado = ventana.ShowDialog();
                                if (resultado == true)
                                {
                                    FormaPagoIDseleccionado = ventana.FormaPagoIDFinal;
                                    _Entregado = ventana._Entregado;
                                    _Cambio = ventana._Cambio;
                                    FinalizarVenta();
                                }
                            }
                        }
                    }
                }
            }

            if (e.Key == Key.F3)
            {
                TxtNitCC.Focus();
                TxtNitCC.IsEnabled = true;
                TxtNitCC.Text = "";
            }

            if (e.Key == Key.F6)
            {
                dgProductos.IsReadOnly = false;
            }
        }

        private void txtCantidadBusqueda_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !EsNumero(e.Text);
        }

        private bool EsNumero(string texto)
        {
            return texto.All(char.IsDigit);
        }

        //-------------------------------LLENADO DE  TABLA DE ARTICULOS ------------------------------------------//

        public class ArticulosAgregados
        {
            public int ArticuloID { get; set; }
            public int Item { get; set; }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public string AlmacenVenta { get; set; }
            public decimal CantidadVenta { get; set; }
            public decimal IVA { get; set; }
            public decimal PrecioUnitario { get; set; }
            public decimal PrecioTotal { get; set; }
            public decimal TotalSinIva { get; set; }
            public decimal TotalConIva { get; set; }
            public decimal CostoSinIva { get; set; }
            public decimal PrecioMinimo { get; set; }
        }

        public List<ArticulosAgregados> ListadoArticulos = new List<ArticulosAgregados>();

        private void txtCantidadBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    if (string.IsNullOrEmpty(cmbArticuloBusqueda.Text))
                    {
                        MessageBox.Show("Error Debe Selecionar Artiulo", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    if (!int.TryParse(txtCantidadBusqueda.Text, out int cantidad) || cantidad <= 0)
                    {
                        MessageBox.Show("Digita Cantidad", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    if (!Double.TryParse(TxtPrecio.Text, out double PrecioVenta) || PrecioVenta <= 0)
                    {
                        MessageBox.Show("Revisa valor de Venta En 0", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    //-----------consultaDatosArticulos----------------//
                    string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                    using (SqlConnection conexion = new SqlConnection(ConexionSql))
                    {
                        conexion.Open();
                        string Consulta = @"select AR.ArticulosID,AR.CostoArticuloSinIva,AR.ArticulosVentaMinima ,CAT.TarifaImpuesto
                                            from  InveArticulos AR, ConfCategoriasInve CAT 
                                            where AR.CategoriasID=CAT.CategoriasID AND AR.CodigoArticulo=@codigo";
                        using (SqlCommand cmd = new SqlCommand(Consulta, conexion))
                        {
                            cmd.Parameters.AddWithValue("codigo", cmbArticuloBusqueda.Text);
                            SqlDataAdapter adp = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adp.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                int IDArticulos = Convert.ToInt32(dt.Rows[0]["ArticulosID"]);
                                decimal CostoSinIva = Convert.ToDecimal(dt.Rows[0]["CostoArticuloSinIva"]);
                                decimal TarifaIva = Convert.ToDecimal(dt.Rows[0]["TarifaImpuesto"]);
                                decimal FactorIva = 1 + (TarifaIva / 100);
                                decimal PrecioMinimo = Convert.ToDecimal(dt.Rows[0]["ArticulosVentaMinima"]);

                                // construir objeto articulo
                                ArticulosAgregados Articulo = new ArticulosAgregados()
                                {
                                    ArticuloID = IDArticulos,
                                    Item = ListadoArticulos.Count + 1,
                                    Codigo = cmbArticuloBusqueda.Text,
                                    Nombre = txtNombreArticulo.Text,
                                    CantidadVenta = Convert.ToDecimal(txtCantidadBusqueda.Text),
                                    AlmacenVenta = CodigoAlmacen,
                                    IVA = TarifaIva,
                                    CostoSinIva = CostoSinIva,
                                    PrecioUnitario = Math.Round(Convert.ToDecimal(TxtPrecio.Text) / FactorIva, 2),
                                    PrecioTotal = Convert.ToDecimal(TxtPrecio.Text),
                                    TotalSinIva = Math.Round(Convert.ToDecimal(TxtPrecio.Text) / FactorIva, 2) * Convert.ToDecimal(txtCantidadBusqueda.Text),
                                    TotalConIva = Convert.ToDecimal(TxtPrecio.Text) * Convert.ToDecimal(txtCantidadBusqueda.Text),
                                    PrecioMinimo = PrecioMinimo

                                };

                                if (isEditing && editingArticulo != null && editingArticulo.ArticuloID == Articulo.ArticuloID)
                                {
                                    // actualizar en BD y en lista
                                    ActualizarDetalleArticulo(Articulo);

                                    // actualizar en la lista local (reemplazar el registro que corresponde)
                                    var existente = ListadoArticulos.FirstOrDefault(x => x.ArticuloID == editingArticulo.ArticuloID && x.Item == editingArticulo.Item);
                                    if (existente != null)
                                    {
                                        existente.CantidadVenta = Articulo.CantidadVenta;
                                        existente.TotalSinIva = Articulo.TotalSinIva;
                                        existente.TotalConIva = Articulo.TotalConIva;
                                        existente.PrecioUnitario = Articulo.PrecioUnitario;
                                        existente.PrecioTotal = Articulo.PrecioTotal;
                                        existente.IVA = Articulo.IVA;
                                    }

                                    isEditing = false;
                                    editingArticulo = null;
                                }
                                else
                                {
                                    // si aún no hay encabezado, crear encabezado (genera consecutivo internamente)
                                    CrearEncabezadoVenta(conexion);

                                    // insertar detalle en BD
                                    InsertarDetalleArticulo(Articulo);

                                    // agregar a la lista local
                                    ListadoArticulos.Add(Articulo);
                                }

                                // Refrescar DataGrid
                                dgProductos.ItemsSource = null;
                                dgProductos.ItemsSource = ListadoArticulos;

                                // Limpiar buscador para el siguiente artículo 
                                cmbArticuloBusqueda.Clear();
                                txtNombreArticulo.Clear();
                                TxtPrecio.Clear();
                                txtCantidadBusqueda.Clear();
                                cmbArticuloBusqueda.Focus();

                                //Actualiza Campos de totales
                                MuestraTotales();

                                //Si se ha cambiado un precio con el boton editar lo inactivo 
                                BtnIditarPrecio.IsEnabled = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message);
            }
        }

        private void MuestraTotales()
        {
            try
            {
                string Signo = "$";
                decimal Base19 = ListadoArticulos.Where(p => p.IVA == 19m).Sum(p => p.TotalSinIva);
                decimal Base5 = ListadoArticulos.Where(p => p.IVA == 5m).Sum(p => p.TotalSinIva);
                decimal Subtotal = ListadoArticulos.Sum(p => p.TotalSinIva);
                decimal Totalventa = ListadoArticulos.Sum(p => p.TotalConIva);
                decimal TotalIva = ListadoArticulos.Sum(p => p.TotalConIva - p.TotalSinIva);
                decimal TotalIva19 = ListadoArticulos.Where(p => p.IVA == 19m).Sum(p => p.TotalConIva - p.TotalSinIva);
                decimal TotalIva5 = ListadoArticulos.Where(p => p.IVA == 5m).Sum(p => p.TotalConIva - p.TotalSinIva);
                decimal BaseExento = ListadoArticulos.Where(p => p.IVA == 0m).Sum(p => p.TotalSinIva);

                txtSubtotal.Text = Signo + Subtotal.ToString("N2");
                txtTotalIva.Text = Signo + TotalIva.ToString("N2");
                TxtTotalGeneral.Text = Signo + Totalventa.ToString("N2");
                txtBase19.Text = Signo + Base19.ToString("N2");
                txtBase5.Text = Signo + Base5.ToString("N2");
                txtIVA19.Text = Signo + TotalIva19.ToString("N2");
                txtIVA5.Text = Signo + TotalIva5.ToString("N2");
                txtReteFuente.Text = Signo + 0.00m.ToString("N2");
                txtDescuentos.Text = Signo + 0.00m.ToString("N2");
                txtExento.Text = Signo + BaseExento.ToString("N2");

                ValorTotalFactura = Convert.ToDouble(Totalventa);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message);
            }
        }

        private void TxtNitCC_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                montarTercero();
                cmbArticuloBusqueda.Focus();
                TxtNitCC.IsEnabled = false;
            }
        }

        private void LeerConsumudor()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Consulta = "select TercerosTipoDocumento,TercerosIdentificacion,TercerosDV,TerceroRazonSocial ,TercerosNombreCompleto TercerosID from InveTerceros where TercerosID=@filtro";
                    using (SqlCommand cmd = new SqlCommand(Consulta, conexion))
                    {
                        cmd.Parameters.AddWithValue("@filtro", NitConsumidorfinal.IDconsumidor);
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            int TipoDocumento = Convert.ToInt32(dt.Rows[0]["TercerosTipoDocumento"]);
                            if (TipoDocumento != 31)
                            {
                                TxtNitCC.Text = dt.Rows[0]["TercerosIdentificacion"].ToString();
                                TxtDV.Text = dt.Rows[0]["TercerosDV"].ToString();
                                TxtNombreCompleto.Text = dt.Rows[0]["TercerosNombreCompleto"].ToString();
                                idTerceroSeleccionado = NitConsumidorfinal.IDconsumidor;
                            }
                            else
                            {
                                TxtNitCC.Text = dt.Rows[0]["TercerosIdentificacion"].ToString();
                                TxtDV.Text = dt.Rows[0]["TercerosDV"].ToString();
                                TxtNombreCompleto.Text = dt.Rows[0]["TerceroRazonSocial"].ToString();
                                idTerceroSeleccionado = NitConsumidorfinal.IDconsumidor;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ListadoArticulos.Count <= 0)
            {
                MessageBoxResult Pregunta = MessageBox.Show("Estas Seguro de salir de la aplicacion", "MENSAJE", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (Pregunta == MessageBoxResult.Yes)
                {
                    e.Cancel = false;
                }
            }
            else
            {
                MessageBox.Show("Hay productos pendientes por facturar", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
                e.Cancel = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LlenarParaImprimir();
        }

        // --------------------------
        // Operaciones en BD progresivas
        // --------------------------

        private void CrearEncabezadoVenta(SqlConnection conexion, SqlTransaction transaction = null)
        {
            try
            {
                if (VentaID > 0) return; // ya existe

                //  Generar consecutivo (SP Sp_ActializarConsecutivo)
                using (SqlCommand cmdCons = new SqlCommand("Sp_ActializarConsecutivo", conexion, transaction))
                {
                    cmdCons.CommandType = CommandType.StoredProcedure;
                    cmdCons.Parameters.Add(new SqlParameter("@codigoDocumento", SqlDbType.NVarChar, 4) { Value = CodigoDocto });

                    SqlParameter parameter = new SqlParameter("nuevoconsecutivo", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmdCons.Parameters.Add(parameter);

                    cmdCons.ExecuteNonQuery();

                    object Valida = cmdCons.Parameters["nuevoconsecutivo"].Value;
                    if (Valida == DBNull.Value)
                    {
                        throw new Exception("No se pudo generar consecutivo.");
                    }

                    NumeroConsecutivo = (int)Valida;
                    txtNumero.Text = NumeroConsecutivo.ToString();
                }

                // 2) Insertar encabezado y obtener VentaID (SP Sp_InsertarVenta)
                DateTime Fecha = DateTime.Parse(fechaTexto);

                using (SqlCommand cmd = new SqlCommand("Sp_InsertarVenta", conexion, transaction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("documentoid", DocumentoID));
                    cmd.Parameters.Add(new SqlParameter("numerodocumento", NumeroConsecutivo));
                    cmd.Parameters.Add(new SqlParameter("fecha", Fecha));
                    cmd.Parameters.Add(new SqlParameter("terceroid", idTerceroSeleccionado ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("vendedorid", VendedorID));
                    cmd.Parameters.Add(new SqlParameter("detalle", DetalleVenta ?? (object)DBNull.Value));

                    object IDDevuelto = cmd.ExecuteScalar();
                    VentaID = Convert.ToInt32(IDDevuelto);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR creando encabezado: " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void InsertarDetalleArticulo(ArticulosAgregados articulo)
        {
            try
            {
                if (articulo == null) return;

                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();

                    // asegurar encabezado
                    CrearEncabezadoVenta(conexion);

                    using (SqlCommand cmd = new SqlCommand("Sp_InsertaVentaDetalle", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ventaid", VentaID));
                        cmd.Parameters.Add(new SqlParameter("@articulid", articulo.ArticuloID));
                        cmd.Parameters.Add(new SqlParameter("@almacenid", AlmacenID));
                        cmd.Parameters.Add(new SqlParameter("@costoSinIva", articulo.CostoSinIva));
                        var pIva = new SqlParameter("@ivaArticulo", SqlDbType.Decimal) { Precision = 5, Scale = 2, Value = articulo.IVA };
                        cmd.Parameters.Add(pIva);
                        cmd.Parameters.Add(new SqlParameter("@ventasiniva", articulo.TotalSinIva));
                        cmd.Parameters.Add(new SqlParameter("@ventamasiva", articulo.TotalConIva));
                        cmd.Parameters.Add(new SqlParameter("@cantidad", articulo.CantidadVenta));
                        cmd.Parameters.Add(new SqlParameter("@detallearticulo", SqlDbType.NVarChar, 200) { Value = articulo.Nombre });
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR insertando detalle: " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void EliminarDetalleArticulo(int articuloID)
        {
            try
            {
                if (VentaID == 0) return;

                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string query = "DELETE FROM InveVentasDetalle WHERE VentaID=@VentaID AND ArticuloID=@Articulid";
                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@VentaID", VentaID);
                        cmd.Parameters.AddWithValue("@Articulid", articuloID);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR eliminando detalle: " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActualizarDetalleArticulo(ArticulosAgregados articulo)
        {
            try
            {
                if (VentaID == 0) return;

                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string query = @"UPDATE InveVentasDetalle 
                                    SET cantidad=@cantidad, ventasiniva=@ventasiniva, ventamasiva=@ventamasiva, costoSinIva=@costoSinIva, ivaArticulo=@ivaArticulo, detallearticulo=@detallearticulo
                                    WHERE VentaID=@VentaID AND ArticuloID=@Articulid";
                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@cantidad", articulo.CantidadVenta);
                        cmd.Parameters.AddWithValue("@ventasiniva", articulo.TotalSinIva);
                        cmd.Parameters.AddWithValue("@ventamasiva", articulo.TotalConIva);
                        cmd.Parameters.AddWithValue("@costoSinIva", articulo.CostoSinIva);
                        cmd.Parameters.Add(new SqlParameter("@ivaArticulo", SqlDbType.Decimal) { Precision = 5, Scale = 2, Value = articulo.IVA });
                        cmd.Parameters.AddWithValue("@detallearticulo", articulo.Nombre);
                        cmd.Parameters.AddWithValue("@VentaID", VentaID);
                        cmd.Parameters.AddWithValue("@Articulid", articulo.ArticuloID);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR actualizando detalle: " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void dgProductos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            try
            {
                if (dgProductos.SelectedItem is ArticulosAgregados ProductoSeleccionado)
                {
                    // Preparamos la edición: llenamos los campos y activamos modo edición.
                    cmbArticuloBusqueda.Text = ProductoSeleccionado.Codigo;
                    txtNombreArticulo.Text = ProductoSeleccionado.Nombre;
                    txtCantidadBusqueda.Text = ProductoSeleccionado.CantidadVenta.ToString();
                    txtCantidadBusqueda.Focus();
                    txtCantidadBusqueda.SelectAll();
                    BtnIditarPrecio.IsEnabled = true;
                    TxtPrecio.Text = ProductoSeleccionado.PrecioTotal.ToString();

                    // marcar edición
                    isEditing = true;
                    editingArticulo = ProductoSeleccionado;

                    // No borramos todavía de la lista local hasta que confirme la edición.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.ToString(), "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //insertar totales y pagos, actualizar Entregado/Cambio, imprimir y reiniciar vista
        private void FinalizarVenta()
        {
            if (VentaID == 0)
            {
                MessageBox.Show("No existe una venta iniciada para finalizar.", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();

                    // =============================
                    // Calcular totales
                    // =============================
                    decimal subtotal = decimal.TryParse(txtSubtotal.Text.Replace("$", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var sub) ? sub : 0;
                    decimal exento = decimal.TryParse(txtExento.Text.Replace("$", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var exen) ? exen : 0;
                    decimal base19 = decimal.TryParse(txtBase19.Text.Replace("$", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var ba19) ? ba19 : 0;
                    decimal base5 = decimal.TryParse(txtBase5.Text.Replace("$", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var bas5) ? bas5 : 0;
                    decimal iva19 = decimal.TryParse(txtIVA19.Text.Replace("$", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var iv19) ? iv19 : 0;
                    decimal iva5 = decimal.TryParse(txtIVA5.Text.Replace("$", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var iv5) ? iv5 : 0;
                    decimal descuentos = decimal.TryParse(txtDescuentos.Text.Replace("$", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var descu) ? descu : 0;
                    decimal retefuente = decimal.TryParse(txtReteFuente.Text.Replace("$", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var retfue) ? retfue : 0;
                    decimal total = decimal.TryParse(TxtTotalGeneral.Text.Replace("$", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var tot) ? tot : 0;

                    // =============================
                    // 1. Insertar totales (SP Sp_InsertarVentaTotales)
                    // =============================
                    using (SqlCommand cmd = new SqlCommand("Sp_InsertarVentaTotales", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("ventaid", VentaID));
                        cmd.Parameters.Add(new SqlParameter("subtotal", subtotal));
                        cmd.Parameters.Add(new SqlParameter("exento", exento));
                        cmd.Parameters.Add(new SqlParameter("gravado19", base19));
                        cmd.Parameters.Add(new SqlParameter("gravado5", base5));
                        cmd.Parameters.Add(new SqlParameter("iva19", iva19));
                        cmd.Parameters.Add(new SqlParameter("iva5", iva5));
                        cmd.Parameters.Add(new SqlParameter("descuentos", descuentos));
                        cmd.Parameters.Add(new SqlParameter("rtefuente", retefuente));
                        cmd.Parameters.Add(new SqlParameter("totalventa", total));
                        cmd.ExecuteNonQuery();
                    }

                    // =============================
                    // 2. Insertar pago (SP Sp_InsertaGuardaPagos)
                    // =============================
                    using (SqlCommand cmd = new SqlCommand("Sp_InsertaGuardaPagos", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("formaPagoid", FormaPagoIDseleccionado));
                        cmd.Parameters.Add(new SqlParameter("documentoid", DocumentoID));
                        cmd.Parameters.Add(new SqlParameter("numerodocumento", NumeroConsecutivo));
                        cmd.Parameters.Add(new SqlParameter("fecha", DateTime.Parse(fechaTexto)));
                        cmd.Parameters.Add(new SqlParameter("vendedorid", VendedorID));
                        cmd.Parameters.Add(new SqlParameter("Valorpagado", total));
                        cmd.ExecuteNonQuery();
                    }

                    // =============================
                    // 3. Guardar Entregado / Cambio (update a InveVentasTotales)
                    // =============================
                    using (SqlCommand cmd = new SqlCommand(@"update InveVentasTotales set Entregado=@filtro1 , Cambio=@filtro2  where VentaID=@ventaid", conexion))
                    {
                        cmd.Parameters.AddWithValue("@filtro1", _Entregado);
                        cmd.Parameters.AddWithValue("@filtro2", _Cambio);
                        cmd.Parameters.AddWithValue("@ventaid", VentaID);
                        cmd.ExecuteNonQuery();
                    }
                }

                // =============================
                // Imprimir
                // =============================
                VentanaImpresion impresion = new VentanaImpresion(VentaID);
                impresion.ShowDialog();

                // =============================
                // Reiniciar variables y UI
                // =============================
                ListadoArticulos.Clear();
                dgProductos.ItemsSource = null;
                txtSubtotal.Text = "$0.00";
                txtTotalIva.Text = "$0.00";
                TxtTotalGeneral.Text = "$0.00";
                txtBase19.Text = "$0.00";
                txtBase5.Text = "$0.00";
                txtIVA19.Text = "$0.00";
                txtIVA5.Text = "$0.00";
                txtReteFuente.Text = "$0.00";
                txtDescuentos.Text = "$0.00";
                txtNumero.Clear();
                TxtDetalle.Clear();
                ValorTotalFactura = 0.00;
                idTerceroSeleccionado = null;

                // reinciar encabezado estado
                VentaID = 0;
                NumeroConsecutivo = 0;

                LeerConsumudor();
                cmbArticuloBusqueda.Focus();
                MessageBox.Show("Venta Finalizada Con Exito", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR finalizando venta: " + ex.ToString(), "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LlenarCambio()
        {
            try
            {
                // Este método ya quedó integrado dentro de FinalizarVenta (update InveVentasTotales).
                // Lo dejo como respaldo por compatibilidad con otras partes del código.
                if (VentaID == 0) return;

                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Consulta = @"update InveVentasTotales set Entregado=@filtro1 , Cambio=@filtro2  where VentaID=@ventaid";
                    using (SqlCommand cmd = new SqlCommand(Consulta, conexion))
                    {
                        cmd.Parameters.AddWithValue("@filtro1", _Entregado);
                        cmd.Parameters.AddWithValue("@filtro2", _Cambio);
                        cmd.Parameters.AddWithValue("@ventaid", VentaID);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.ToString(), "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //=========================================================
        //            DATOS ENCABEZADO FACTURA
        //=========================================================
        private void LlenarParaImprimir()
        {
            var ventana = new VentanaBuscarDocumentos();
            bool? result = ventana.ShowDialog();
            if (result == true)
            {
                var DetalleArticulos = ventana._ArticulosAgregados;
                dgProductos.ItemsSource = DetalleArticulos;

                DataTable dt = ventana._DatosEncabezado;
                int TipoDocumento = Convert.ToInt32(dt.Rows[0]["TercerosTipoDocumento"]);
                int VentaID = Convert.ToInt32(dt.Rows[0]["VentaID"]);

                if (TipoDocumento != 31)
                {
                    txtDocumento.Text = dt.Rows[0]["CodigoDocumento"].ToString();
                    txtNumero.Text = dt.Rows[0]["NumeroDocumento"].ToString();
                    TxtNitCC.Text = dt.Rows[0]["TercerosIdentificacion"].ToString();
                    TxtNombreCompleto.Text = dt.Rows[0]["TercerosNombreCompleto"].ToString();
                    txtVendedor.Text = dt.Rows[0]["Vendedor"].ToString();
                    dpFecha.Text = dt.Rows[0]["Fecha"].ToString();

                }
                else
                {
                    txtDocumento.Text = dt.Rows[0]["CodigoDocumento"].ToString();
                    txtNumero.Text = dt.Rows[0]["NumeroDocumento"].ToString();
                    TxtNitCC.Text = dt.Rows[0]["TercerosIdentificacion"].ToString();
                    TxtNombreCompleto.Text = dt.Rows[0]["TerceroRazonSocial"].ToString();
                    txtVendedor.Text = dt.Rows[0]["Vendedor"].ToString();
                    dpFecha.Text = dt.Rows[0]["Fecha"].ToString();
                }
                MuestraTotales();
                // =============================
                // Imprimir
                // =============================
                VentanaImpresion impresion = new VentanaImpresion(VentaID);
                impresion.ShowDialog();

                // =============================
                // Reiniciar variables y UI
                // =============================
                ListadoArticulos.Clear();
                dgProductos.ItemsSource = null;
                txtSubtotal.Text = "$0.00";
                txtTotalIva.Text = "$0.00";
                TxtTotalGeneral.Text = "$0.00";
                txtBase19.Text = "$0.00";
                txtBase5.Text = "$0.00";
                txtIVA19.Text = "$0.00";
                txtIVA5.Text = "$0.00";
                txtReteFuente.Text = "$0.00";
                txtDescuentos.Text = "$0.00";
                txtNumero.Clear();
                TxtDetalle.Clear();
                ValorTotalFactura = 0.00;
                idTerceroSeleccionado = null;

                // reinciar encabezado estado
                VentaID = 0;
                NumeroConsecutivo = 0;

                LeerConsumudor();
                cmbArticuloBusqueda.Focus();
                MessageBox.Show("Documento Reimpreso Correctamente ", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void ButtonCierre_Click(object sender, RoutedEventArgs e)
        {
            if (ListadoArticulos.Count > 0)
            {
                MessageBox.Show("Debe Finalizar Venta Para Generar Cierre", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            VentanaClave VentanaClave = new VentanaClave(1, 0, 1);
            VentanaClave.Owner = this;
            bool? valida = VentanaClave.ShowDialog();
            if (valida == true)
            {
                VentanaTestigoDeVentas ventana = new VentanaTestigoDeVentas(DocumentoID, VendedorID);
                ventana.Owner = this;
                ventana.ShowDialog();
            }
        }

        private void BtnRetiros_Click(object sender, RoutedEventArgs e)
        {
            if (ListadoArticulos.Count > 0)
            {
                MessageBox.Show("Debe Finalizar Venta Para Generar Retiros", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
                return;

            }
            VentanaClave Ventana = new VentanaClave(1, 0, 1);
            Ventana.Owner = this;
            bool? Valida = Ventana.ShowDialog();
            if (Valida == true)
            {
                VentanaRetirosCaja VentanaRetiros = new VentanaRetirosCaja(VendedorID);
                VentanaRetiros.Owner = this;
                bool? VentanaAbierta = VentanaRetiros.ShowDialog();
                if (VentanaAbierta == true)
                {
                    cmbArticuloBusqueda.Focus();
                }
            }
        }

        private void TxtPrecio_LostFocus(object sender, RoutedEventArgs e)
        {
            string ValorNuevo = TxtPrecio.Text;
            if (string.IsNullOrEmpty(ValorNuevo))
            {
                MessageBox.Show("No Puede Dejar Precio en 0", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                TxtPrecio.IsEnabled = false;
            }
        }

        private void BtnIditarPrecio_Click(object sender, RoutedEventArgs e)
        {
            VentanaClave Ventana = new VentanaClave(1, 0, 1);
            Ventana.Owner = this;
            bool? Valida = Ventana.ShowDialog();
            if (Valida == true)
            {
                TxtPrecio.Focus();
                TxtPrecio.IsEnabled = true;
            }
        }

        private void TxtPrecio_KeyDown(object sender, KeyEventArgs e)
        {
            string ValorNuevo = TxtPrecio.Text;
            if (string.IsNullOrEmpty(ValorNuevo))
            {
                MessageBox.Show("No Puede Dejar Precio en 0", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                if (e.Key == Key.Enter)
                {
                    if (dgProductos.SelectedItem is ArticulosAgregados ProductoSeleccionado)
                    {
                        decimal PrecioNuevo = Convert.ToDecimal(TxtPrecio.Text);
                        if (PrecioNuevo < ProductoSeleccionado.PrecioMinimo)
                        {
                            MessageBox.Show("No Puede Dejar Precio Por Debajo Del Minimo De Venta", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                            TxtPrecio.Focus();
                            return;
                        }
                    }
                    else
                    {
                        txtCantidadBusqueda.Focus();
                    }
                }
            }
        }
    }
}
