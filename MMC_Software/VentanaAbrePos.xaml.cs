using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaAbrePos.xaml
    /// </summary>
    public partial class VentanaAbrePos : Window
    {
        private int _UsuarioID;
        public VentanaAbrePos(int usuarioID)
        {
            _UsuarioID = usuarioID;
            InitializeComponent();
            MuestraAlmacen();
            MuestraVendedor();
            MuestraDocumentos();
            dtFecha.SelectedDate = DateTime.Now;
        }

        private void MuestraDocumentos()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                var DocumentoVenta = new DocumentosVentaRespositorio(ConexionSql);
                var IngresoApp = new RepositorioIngresoApp(ConexionSql);
                int? datosUsuario = RepositorioIngresoApp.DatosPredeterminadosUsuarios.DocumentoID;
                DataTable dt;
                if (datosUsuario == null)
                {
                    dt = DocumentoVenta.ListarDocumentoVentas();
                }
                else
                {
                    dt = DocumentoVenta.DevolverDoctoVentaXUsuario(_UsuarioID);
                    cmbDocumento.SelectedIndex = 0;
                    cmbDocumento.IsEnabled = false;
                }

                cmbDocumento.DisplayMemberPath = "INFODOCUMENTO";
                cmbDocumento.SelectedValuePath = "DocumentoID";
                cmbDocumento.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void MuestraVendedor()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                var DatosIngreso = new RepositorioIngresoApp(ConexionSql);
                int? VendedorID = RepositorioIngresoApp.DatosPredeterminadosUsuarios.VendedorID;
                var DatosVendedor = new VendedoresRepositorio(ConexionSql);
                DataTable dt;
                if (VendedorID == null)
                {
                    dt = DatosVendedor.GenerarVendedores();
                }
                else
                {
                    dt = DatosVendedor.GenerarVendedoresXUsuario(_UsuarioID);
                    cmbVendedor.SelectedIndex = 0;
                    cmbVendedor.IsEnabled = false;
                }

                cmbVendedor.DisplayMemberPath = "INFOVENDEDOR";
                cmbVendedor.SelectedValuePath = "VendedorID";
                cmbVendedor.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        public void MuestraAlmacen()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                var Almacen = new AlmacenRepositorio(ConexionSql);
                int? DatosUsuarios = RepositorioIngresoApp.DatosPredeterminadosUsuarios.AlmacenID;
                DataTable dt;
                if (DatosUsuarios == null)
                {
                    dt = Almacen.DevolverAlmacenes(1, 0);
                }
                else
                {
                    dt = Almacen.DevolverAlmacenXUsuario(_UsuarioID);
                    cmbAlmacen.SelectedIndex = 0;
                    cmbAlmacen.IsEnabled = false;
                }

                cmbAlmacen.DisplayMemberPath = "Almacen";
                cmbAlmacen.SelectedValuePath = "IdAlmacen";
                cmbAlmacen.ItemsSource = dt.DefaultView;

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }



        private void btnEntrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int AlmacenID = 0;
                string CodigoAlmacen = "";
                string NombreAlmacen = "";
                string CodigoVendedor = "";
                string NombreVendedor = "";
                int VendedorID = 0;
                string CodidoDocto = "";
                string NombreDocto = "";
                int DocumentoID = 0;
                string fechaTexto = dtFecha.SelectedDate?.ToString("yyyy-MM-dd");
                if (cmbAlmacen.SelectedValue != null && cmbVendedor.SelectedValue != null && !string.IsNullOrEmpty(fechaTexto))
                {
                    string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                    using (SqlConnection conexion = new SqlConnection(ConexionSql))
                    {
                        conexion.Open();
                        string consulta = "select CodigoAlmacen,NombreAlmacen,IdAlmacen from ConfAlmacen where IdAlmacen=@filtro";
                        using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                        {
                            cmd.Parameters.AddWithValue("@filtro", cmbAlmacen.SelectedValue);
                            SqlDataAdapter adp = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adp.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                AlmacenID = Convert.ToInt32(dt.Rows[0]["IdAlmacen"]);
                                CodigoAlmacen = dt.Rows[0]["CodigoAlmacen"].ToString();
                                NombreAlmacen = dt.Rows[0]["NombreAlmacen"].ToString();
                            }
                        }
                        string ConsultaVendedir = "select CodigoVendedor,NombreVendedor,VendedorID from ConfVendedores where VendedorID=@filtro";
                        using (SqlCommand cmd = new SqlCommand(ConsultaVendedir, conexion))
                        {
                            cmd.Parameters.AddWithValue("@filtro", cmbVendedor.SelectedValue);
                            SqlDataAdapter adp = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adp.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                VendedorID = Convert.ToInt32(dt.Rows[0]["VendedorID"]);
                                CodigoVendedor = dt.Rows[0]["CodigoVendedor"].ToString();
                                NombreVendedor = dt.Rows[0]["NombreVendedor"].ToString();
                            }
                        }
                        string ConsultaDocto = "SELECT CodigoDocumento, NombreDocumento , DocumentoID FROM ConfDocumentos where DocumentoID=@filtro";
                        using (SqlCommand cmd = new SqlCommand(ConsultaDocto, conexion))
                        {
                            cmd.Parameters.AddWithValue("@filtro", cmbDocumento.SelectedValue);
                            SqlDataAdapter adp = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adp.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                CodidoDocto = dt.Rows[0]["CodigoDocumento"].ToString();
                                NombreDocto = dt.Rows[0]["NombreDocumento"].ToString();
                                DocumentoID = Convert.ToInt32(dt.Rows[0]["DocumentoID"]);
                            }
                        }
                    }
                    // aqui pongo el 2 por que es tipo de permiso que se va leer en el cuadro de contraseñas
                    VentanaClave Ventana = new VentanaClave(2, VendedorID, 0);
                    Ventana.Owner = this;
                    bool? Valido = Ventana.ShowDialog();
                    if (Valido == true)
                    {
                        ModuloPuntoVenta ventanaPuntoVenta = new ModuloPuntoVenta(
                       AlmacenID,
                       CodigoAlmacen,
                       NombreAlmacen,
                       VendedorID,
                       CodigoVendedor,
                       NombreVendedor,
                       fechaTexto,
                       CodidoDocto,
                       NombreDocto,
                       DocumentoID);

                        ventanaPuntoVenta.Show();
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Se deben seleccionar todos los campos", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("DEBE LLENAR TODOS LOS CAMPOS " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
