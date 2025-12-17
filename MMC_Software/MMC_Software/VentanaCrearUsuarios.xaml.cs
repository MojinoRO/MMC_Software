using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaCrearUsuarios.xaml
    /// </summary>
    public partial class VentanaCrearUsuarios : UserControl
    {

        public VentanaCrearUsuarios()
        {
            InitializeComponent();
            llenarTipoUsuario();
            llenarAlmacen();
            llenarVendedor();
            LlenarDocumento();
            BloquearCampos();
            llenarListaUsuarios();
        }

        private void DesbloquearCampos()
        {
            cmbTipoUsuario.IsEnabled = true;
            TxtNombreUsuario.IsEnabled = true;
            cmbAlmacen.IsEnabled = true;
            cmbVendedores.IsEnabled = true;
            cmbDocumentos.IsEnabled = true;
            chkModuloPos.IsEnabled = true;
            chkModuloTaller.IsEnabled = true;
            chkModuloNomina.IsEnabled = true;
            chkModuloCartera.IsEnabled = true;
            chkModuloContabilidad.IsEnabled = true;
            chkModuloinventarios.IsEnabled = true;
        }

        private void BloquearCampos()
        {
            cmbTipoUsuario.IsEnabled = false;
            TxtNombreUsuario.IsEnabled = false;
            cmbAlmacen.IsEnabled = false;
            cmbVendedores.IsEnabled = false;
            cmbDocumentos.IsEnabled = false;
            chkModuloPos.IsEnabled = false;
            chkModuloTaller.IsEnabled = false;
            chkModuloNomina.IsEnabled = false;
            chkModuloCartera.IsEnabled = false;
            chkModuloContabilidad.IsEnabled = false;
            chkModuloinventarios.IsEnabled = false;
        }

        private void LimpiarCampos()
        {
            cmbTipoUsuario.SelectedValue = null;
            TxtNombreUsuario.Text = "";
            cmbAlmacen.SelectedValue = null;
            cmbVendedores.SelectedValue = null;
            cmbDocumentos.SelectedValue = null;
            chkModuloPos.IsChecked = false;
            chkModuloTaller.IsChecked = false;
            chkModuloNomina.IsChecked = false;
            chkModuloCartera.IsChecked = false;
            chkModuloContabilidad.IsChecked = false;
            chkModuloinventarios.IsChecked = false;
        }
        private void llenarTipoUsuario()
        {
            List<KeyValuePair<int, string>> TiposUsuarios = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(0,"Administrador"),
                new KeyValuePair<int,string>(1,"Usuario Regular"),
                new KeyValuePair<int, string>(2,"UsuarioPos")
            };

            cmbTipoUsuario.ItemsSource = TiposUsuarios;
            cmbTipoUsuario.DisplayMemberPath = "Value";
            cmbTipoUsuario.SelectedValuePath = "Key";
        }

        private void llenarAlmacen()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                var repositoryAlmacen = new AlmacenRepositorio(ConexionSql);
                DataTable dt = repositoryAlmacen.DevolverAlmacenes(1, 0);
                cmbAlmacen.DisplayMemberPath = "Almacen";
                cmbAlmacen.SelectedValuePath = "IdAlmacen";
                cmbAlmacen.ItemsSource = dt.DefaultView;

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message, "Mensaje", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void llenarVendedor()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                var repositoryVendedor = new VendedoresRepositorio(ConexionSql);
                DataTable dt = repositoryVendedor.GenerarVendedores();
                cmbVendedores.DisplayMemberPath = "INFOVENDEDOR";
                cmbVendedores.SelectedValuePath = "VendedorID";
                cmbVendedores.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "Mensaje", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LlenarDocumento()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                var RepositorioDocumentos = new DocumentosVentaRespositorio(ConexionSql);
                DataTable dt = RepositorioDocumentos.ListarDocumentoVentas();
                cmbDocumentos.DisplayMemberPath = "INFODOCUMENTO";
                cmbDocumentos.SelectedValuePath = "DocumentoID";
                cmbDocumentos.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.ToString(), "Mensaje", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {

            LimpiarCampos();
            DesbloquearCampos();
            lstListadoUsuarios.SelectedValue = null;
        }


        private void CrearUsuarios()
        {
            try
            {
                int TipoUsuario = Convert.ToInt32(cmbTipoUsuario.SelectedValue);
                string NombreUsuario = TxtNombreUsuario.Text.Trim();
                int? AlmacenID = cmbAlmacen.SelectedValue as int?;
                if (AlmacenID == 0) AlmacenID = null;

                int? VendedorID = cmbVendedores.SelectedValue as int?;
                if (VendedorID == 0) VendedorID = null;

                int? DocumentosID = cmbDocumentos.SelectedValue as int?;
                if (DocumentosID == 0) DocumentosID = null;

                int ModuloPos = chkModuloPos.IsChecked == true ? 1 : 0;
                int ModuloTaller = chkModuloTaller.IsChecked == true ? 1 : 0;
                int ModuloCartera = chkModuloCartera.IsChecked == true ? 1 : 0;
                int ModuloNomina = chkModuloNomina.IsChecked == true ? 1 : 0;
                int ModuloContabilidad = chkModuloContabilidad.IsChecked == true ? 1 : 0;
                int ModuloInventarios = chkModuloinventarios.IsChecked == true ? 1 : 0;
                if (string.IsNullOrEmpty(NombreUsuario))
                {
                    MessageBox.Show("Se deben diligenciar Nombre De Usuario");
                    TxtNombreUsuario.Focus();
                    return;
                }

                if (cmbTipoUsuario.SelectedValue == null)
                {
                    MessageBox.Show("Se deben seleccionar tipo Usuario");
                    cmbTipoUsuario.Focus();
                    return;
                }

                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                var repositorioCrear = new RespositoryInsertBD(ConexionSql);
                repositorioCrear.CrearUsuarios(TipoUsuario, NombreUsuario, AlmacenID, VendedorID, DocumentosID, ModuloPos, ModuloTaller,
                    ModuloCartera, ModuloNomina, ModuloContabilidad, ModuloInventarios);
                MessageBox.Show("Usuario Creado Correctamente , Su Clave Es 123");
                BloquearCampos();
                LimpiarCampos();
                llenarListaUsuarios();
                lstListadoUsuarios.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstListadoUsuarios.SelectedValue != null)
                {
                    string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                    int UsuarioID = Convert.ToInt32(lstListadoUsuarios.SelectedValue);
                    var Listadorepositorio = new ListadoUsuariosRepositorio(ConexionSql);
                    bool Existe = Listadorepositorio.ExisteUsuario(UsuarioID);
                    if (Existe == true)
                    {
                        string NombreUsuario = TxtNombreUsuario.Text;
                        int TipoUsuario = Convert.ToInt32(cmbTipoUsuario.SelectedValue);
                        int? AlmacenID = cmbAlmacen.SelectedValue as int?;
                        if (AlmacenID == 0) AlmacenID = null;

                        int? VendedorID = cmbVendedores.SelectedValue as int?;
                        if (VendedorID == 0) VendedorID = null;

                        int? DocumentosID = cmbDocumentos.SelectedValue as int?;
                        if (DocumentosID == 0) DocumentosID = null;
                        int ModuloPos = chkModuloPos.IsChecked == true ? 1 : 0;
                        int ModuloTaller = chkModuloTaller.IsChecked == true ? 1 : 0;
                        int ModuloCartera = chkModuloCartera.IsChecked == true ? 1 : 0;
                        int ModuloNomina = chkModuloNomina.IsChecked == true ? 1 : 0;
                        int ModuloContabilidad = chkModuloContabilidad.IsChecked == true ? 1 : 0;
                        int ModuloInventarios = chkModuloinventarios.IsChecked == true ? 1 : 0;
                        if (string.IsNullOrEmpty(NombreUsuario))
                        {
                            MessageBox.Show("Se deben diligenciar Nombre De Usuario");
                            TxtNombreUsuario.Focus();
                            return;
                        }

                        if (cmbTipoUsuario.SelectedValue == null)
                        {
                            MessageBox.Show("Se deben seleccionar tipo Usuario");
                            cmbTipoUsuario.Focus();
                            return;
                        }

                        Listadorepositorio.UpdateDatos(TipoUsuario, NombreUsuario, AlmacenID, VendedorID, DocumentosID, ModuloPos, ModuloTaller,
                        ModuloCartera, ModuloNomina, ModuloContabilidad, ModuloInventarios, UsuarioID);
                        MessageBox.Show("Usuario Actualizado Con Existo", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    CrearUsuarios();
                }

                BloquearCampos();
                LimpiarCampos();
                llenarListaUsuarios();

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void llenarListaUsuarios()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                var ListadoUsuariosRepositorio = new ListadoUsuariosRepositorio(ConexionSql);
                DataTable dt = ListadoUsuariosRepositorio.ListadoUsuarios();
                lstListadoUsuarios.DisplayMemberPath = "NombreUsuario";
                lstListadoUsuarios.SelectedValuePath = "UsuariosID";
                lstListadoUsuarios.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void MontarInfoUsuario()
        {
            try
            {
                int UsuarioID = Convert.ToInt32(lstListadoUsuarios.SelectedValue);
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                if (lstListadoUsuarios.SelectedValue != null)
                {
                    var ListadoUsuariosRepositorio = new ListadoUsuariosRepositorio(ConexionSql);
                    DataTable dt = ListadoUsuariosRepositorio.LlenarDatos(UsuarioID);
                    if (dt.Rows.Count > 0)
                    {
                        cmbTipoUsuario.SelectedValue = Convert.ToInt32(dt.Rows[0]["TipoUsuario"]);
                        TxtNombreUsuario.Text = dt.Rows[0]["NombreUsuario"].ToString();
                        cmbVendedores.SelectedValue = dt.Rows[0]["VendedorID"] == DBNull.Value ? (int?)null : Convert.ToInt32(dt.Rows[0]["VendedorID"]);
                        cmbAlmacen.SelectedValue = dt.Rows[0]["IdAlmacen"] == DBNull.Value ? (int?)null : Convert.ToInt32(dt.Rows[0]["IdAlmacen"]);
                        cmbDocumentos.SelectedValue = dt.Rows[0]["DocumentoID"] == DBNull.Value ? (int?)null : Convert.ToInt32(dt.Rows[0]["DocumentoID"]);
                        int Modulopos = Convert.ToInt32(dt.Rows[0]["ModuloPos"]);
                        int ModuloTaller = Convert.ToInt32(dt.Rows[0]["ModuloTaller"]);
                        int ModuloCartera = Convert.ToInt32(dt.Rows[0]["ModuloCartera"]);
                        int ModuloNomina = Convert.ToInt32(dt.Rows[0]["ModuloNomina"]);
                        int ModuloInventarios = Convert.ToInt32(dt.Rows[0]["ModuloInventarios"]);
                        int ModuloContabilidad = Convert.ToInt32(dt.Rows[0]["ModuloContabilidad"]);
                        chkModuloPos.IsChecked = Modulopos == 1;
                        chkModuloTaller.IsChecked = ModuloTaller == 1;
                        chkModuloCartera.IsChecked = ModuloCartera == 1;
                        chkModuloNomina.IsChecked = ModuloNomina == 1;
                        chkModuloContabilidad.IsChecked = ModuloContabilidad == 1;
                        chkModuloinventarios.IsChecked = ModuloInventarios == 1;
                    }
                }
                else
                {
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.Message, "Mensaje", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void lstListadoUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MontarInfoUsuario();
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (lstListadoUsuarios.SelectedValue != null)
            {
                DesbloquearCampos();
                btnCerrar.IsEnabled = false;
                btnCrear.IsEnabled = false;
                btnEliminar.IsEnabled = false;
            }
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            BloquearCampos();
            LimpiarCampos();
            var TabAbierto = this.Parent as TabItem;

            if (TabAbierto != null)
            {
                var item = TabAbierto.Parent as TabControl;
                if (item != null)
                {
                    item.Items.Remove(TabAbierto);
                }
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult Pregunta = MessageBox.Show("Estas Seguro De Eliminar Usuario?", "MENSAJE", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (Pregunta == MessageBoxResult.Yes)
                {
                    int UsuarioID = Convert.ToInt32(lstListadoUsuarios.SelectedValue);
                    string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                    var UsuariosRepositorio = new ListadoUsuariosRepositorio(ConexionSql);
                    UsuariosRepositorio.EliminarUsuarios(UsuarioID);
                    MessageBox.Show("Usuario Eliminado Correctamente");
                    BloquearCampos();
                    LimpiarCampos();
                    llenarListaUsuarios();
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.Message, "Mensaje", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TxtNombreUsuario_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ValidarExisteNombre();
        }

        private void ValidarExisteNombre()
        {
            string NombreUsuario = TxtNombreUsuario.Text;
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
            var UsuariosRepositorio = new ListadoUsuariosRepositorio(ConexionSql);
            bool Existe = UsuariosRepositorio.ConsultaNombreUsuario(NombreUsuario);
            if (Existe == true)
            {
                MessageBox.Show("El Nombre De Usuario Ya Existe", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Stop);
                TxtNombreUsuario.Focus();
                TxtNombreUsuario.SelectAll();
                return;
            }
        }
    }

}
