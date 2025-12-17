using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaCrearTecnicos.xaml
    /// </summary>
    public partial class VentanaCrearTecnicos : UserControl
    {
        public VentanaCrearTecnicos()
        {
            InitializeComponent();
            CargarEstado();
            MuestraTecnicos();
            BloquearCampos();
        }


        private void BloquearCampos()
        {
            txtCedula.IsEnabled = false;
            txtNombre.IsEnabled = false;
            txtcelular.IsEnabled = false;
            txtCodigo.IsEnabled = false;
        }

        private void DesbloquearCampos(bool CodigoTecnico)
        {
            txtCedula.IsEnabled = true;
            txtNombre.IsEnabled = true;
            txtcelular.IsEnabled = true;
            txtCodigo.IsEnabled = CodigoTecnico;
        }

        private void LimpiarCampos()
        {
            txtCedula.Clear();
            txtNombre.Clear();
            txtcelular.Clear();
            txtCodigo.Clear();
        }

        private void CargarEstado()
        {
            try
            {
                var Estado = new List<KeyValuePair<int, string>>
                {
                    new KeyValuePair<int, string>(0,"Activo"),
                    new KeyValuePair<int,string>(1,"Inactivo")
                };

                cmbEstado.ItemsSource = Estado;
                cmbEstado.DisplayMemberPath = "Value";
                cmbEstado.SelectedValuePath = "Key";

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MuestraTecnicos()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Consulta = @"select CONCAT(Codigo,' ',NombreTecnico) aS infocompleta ,TecnicoID from TallTecnicos";
                    using (SqlCommand cmd = new SqlCommand(Consulta, conexion))
                    {
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            lstTecnicosCreados.DisplayMemberPath = "infocompleta";
                            lstTecnicosCreados.SelectedValuePath = "TecnicoID";
                            lstTecnicosCreados.ItemsSource = dt.DefaultView;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CodigoExiste(string Codigo)
        {

            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

            using (SqlConnection conexion = new SqlConnection(ConexionSql))
            {
                conexion.Open();
                string Consulta = @"select COUNT (*) from TallTecnicos where  Codigo=@filtro";
                using (SqlCommand cmd = new SqlCommand(Consulta, conexion))
                {
                    cmd.Parameters.AddWithValue("@filtro", Codigo);
                    int Cantidad = (int)cmd.ExecuteScalar();
                    return Cantidad > 0;
                }

            }
        }

        private void LlenarCampos()
        {
            try
            {
                if (lstTecnicosCreados.SelectedValue != null)
                {
                    string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                    using (SqlConnection conexion = new SqlConnection(ConexionSql))
                    {
                        conexion.Open();
                        string Consulta = @"select Ter.TercerosTipoDocumento,Ter.TercerosIdentificacion, TercerosNombreCompleto,Ter.TercerosTelefono ,tall.Codigo,Tall.Estado,tall.Codigo,tall.Estado
                                            from  InveTerceros Ter, TallTecnicos tall
                                            where Ter.TercerosID=tall.TercerosID and tall.TecnicoID=@filtro";
                        using (SqlCommand cmd = new SqlCommand(Consulta, conexion))
                        {
                            cmd.Parameters.AddWithValue("@filtro", lstTecnicosCreados.SelectedValue);
                            SqlDataAdapter adp = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adp.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                txtCedula.Text = dt.Rows[0]["TercerosIdentificacion"].ToString();
                                txtNombre.Text = dt.Rows[0]["TercerosNombreCompleto"].ToString();
                                txtcelular.Text = dt.Rows[0]["TercerosTelefono"].ToString();
                                txtCodigo.Text = dt.Rows[0]["Codigo"].ToString();
                                int EstadoActual = Convert.ToInt32(dt.Rows[0]["Estado"]);
                                CargarEstado();
                                cmbEstado.SelectedValue = EstadoActual;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "Mensaje", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CrearTecnicos()
        {
            try
            {
                int TercerosID = 0;
                string Cedula = txtCedula.Text;
                string Nombre = txtNombre.Text;
                string Codigo = txtCodigo.Text;
                int Estado = Convert.ToInt32(cmbEstado.SelectedValue);

                if (string.IsNullOrEmpty(Cedula) || string.IsNullOrEmpty(Nombre) || string.IsNullOrEmpty(Codigo))
                {
                    MessageBox.Show("Hay campos en blanco en el formulario", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtCedula.Focus();
                    return;
                }
                if (cmbEstado.SelectedValue == null)
                {
                    MessageBox.Show("Debe selecionar Estado del tecnico ", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Consulta = @"select TercerosID from InveTerceros where TercerosIdentificacion=@filtro";
                    using (SqlCommand cmd = new SqlCommand(Consulta, conexion))
                    {
                        cmd.Parameters.AddWithValue("@filtro", Cedula);
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            TercerosID = Convert.ToInt32(dt.Rows[0]["TercerosID"]);
                        }
                        if (TercerosID == 0)
                        {
                            MessageBox.Show("Se debe diligenciar Campo de Cedula", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Stop);
                            txtCedula.Focus();
                            return;
                        }
                        string Verifica = @"SELECT COUNT(*) FROM TallTecnicos WHERE TercerosID=@terceroid";
                        using (SqlCommand cmdVerifica = new SqlCommand(Verifica, conexion))
                        {
                            cmdVerifica.Parameters.AddWithValue("@terceroid", TercerosID);
                            int existe = (int)cmdVerifica.ExecuteScalar();
                            if (existe > 0)
                            {
                                MessageBox.Show("Este tercero ya está registrado como técnico", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Stop);
                                return;
                            }
                        }
                        string Insert = @"insert into TallTecnicos
                                            (TercerosID,
                                            NombreTecnico,
                                            Codigo,
                                            Estado) 
                                            values
                                            (@terceroid,
                                            @nombre,
                                            @codigo,
                                            @estado)";
                        using (SqlCommand cmdInsert = new SqlCommand(Insert, conexion))
                        {
                            cmdInsert.Parameters.AddWithValue("@terceroid", TercerosID);
                            cmdInsert.Parameters.AddWithValue("@nombre", Nombre);
                            cmdInsert.Parameters.AddWithValue("@codigo", Codigo);
                            cmdInsert.Parameters.AddWithValue("estado", cmbEstado.SelectedValue);
                            cmdInsert.ExecuteNonQuery();
                        }
                    }
                }
                MuestraTecnicos();
                BloquearCampos();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActualizaTecnicos(int TecnicoID, string Nombre, int Estado)
        {
            try
            {
                int Terceroid = 0;
                string cedula = txtCedula.Text;
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Consulta = @"select TercerosID from InveTerceros where TercerosIdentificacion=@filtro";
                    using (SqlCommand cmd = new SqlCommand(Consulta, conexion))
                    {
                        cmd.Parameters.AddWithValue("@filtro", cedula);
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            Terceroid = Convert.ToInt32(dt.Rows[0]["TercerosID"]);
                        }
                        if (Terceroid == 0)
                        {
                            MessageBox.Show("Revisa Campo de cedula del Tecnico", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    string Update = @"update TallTecnicos set
                                        TercerosID=@terceroid,
                                        NombreTecnico=@nombrenew,
                                        Estado=@estadonew
                                        where
                                        TecnicoID=@filtro";
                    using (SqlCommand cmd = new SqlCommand(Update, conexion))
                    {
                        cmd.Parameters.AddWithValue("@terceroid", Terceroid);
                        cmd.Parameters.AddWithValue("@nombrenew", Nombre);
                        cmd.Parameters.AddWithValue("@estadonew", Estado);
                        cmd.Parameters.AddWithValue("@filtro", TecnicoID);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Registro Actualizado Correctamente", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "Mensaje", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EliminarTecnicos(int TecnicoID)
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Delete = "delete from TallTecnicos where TecnicoID=@filtro";
                    using (SqlCommand cmd = new SqlCommand(Delete, conexion))
                    {
                        cmd.Parameters.AddWithValue("@filtro", TecnicoID);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Registro Eliminado Correctamente", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                MuestraTecnicos();
                LimpiarCampos();
                BloquearCampos();

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "Mensaje", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //BOTONES

        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {
            DesbloquearCampos(true);
            LimpiarCampos();
            txtCedula.Focus();
            lstTecnicosCreados.SelectedIndex = -1;

        }

        private void txtCedula_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    string Cedula = txtCedula.Text.Trim();
                    if (string.IsNullOrEmpty(Cedula))
                    {
                        MessageBox.Show("Digite Numero De cedula", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Error);
                        txtCedula.Focus();
                        return;
                    }

                    string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                    using (SqlConnection conexion = new SqlConnection(ConexionSql))
                    {
                        conexion.Open();
                        string Consulta = @"
                                            select TercerosTipoDocumento,TercerosIdentificacion,TercerosNombreCompleto,TercerosTelefono  from InveTerceros 
                                            where TercerosIdentificacion=@filtro";
                        using (SqlCommand cmd = new SqlCommand(Consulta, conexion))
                        {
                            cmd.Parameters.AddWithValue("@filtro", Cedula);
                            SqlDataAdapter adp = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adp.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                int Tipo = Convert.ToInt32(dt.Rows[0]["TercerosTipoDocumento"]);
                                if (Tipo != 13)
                                {
                                    MessageBox.Show("Tecnico seleccionado es una empresa , revisa cedula", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Stop);
                                    txtCedula.Focus();
                                    return;
                                }
                                else
                                {
                                    txtCedula.Text = dt.Rows[0]["TercerosIdentificacion"].ToString();
                                    txtNombre.Text = dt.Rows[0]["TercerosNombreCompleto"].ToString();
                                    txtcelular.Text = dt.Rows[0]["TercerosTelefono"].ToString();
                                }
                            }
                            else
                            {
                                MessageBoxResult Pregunta = MessageBox.Show("Tercero No Existe, ¿Desea Crearlo?", "Mensaje", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                if (Pregunta == MessageBoxResult.Yes)
                                {
                                    VentanaCrearTerceros ventana = new VentanaCrearTerceros();
                                    ventana.ShowDialog();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR " + ex.Message, "Mensaje", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Codigo = txtCodigo.Text;
                string Nombre = txtNombre.Text;
                string Cedula = txtCedula.Text;
                int Estado = Convert.ToInt32(cmbEstado.SelectedValue);



                if (string.IsNullOrEmpty(Codigo) || string.IsNullOrEmpty(Nombre) || string.IsNullOrEmpty(Cedula))
                {
                    MessageBox.Show("Revisa campos en blanco", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (lstTecnicosCreados.SelectedValue != null) // si es null quiere decir crear , si es dferente actualiza
                {
                    int TecnicoID = Convert.ToInt32(lstTecnicosCreados.SelectedValue);
                    ActualizaTecnicos(TecnicoID, Nombre, Estado);
                }
                else //como es nulo debe ser crear
                {
                    if (CodigoExiste(Codigo))
                    {
                        MessageBox.Show("Codigo Ya existe", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Stop);
                        txtCodigo.Focus();
                        return;
                    }
                    else
                    {
                        CrearTecnicos();
                    }
                }

                MuestraTecnicos();
                LimpiarCampos();
                BloquearCampos();

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "Mensaje", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (lstTecnicosCreados.SelectedValue != null)
            {
                DesbloquearCampos(false);
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (lstTecnicosCreados.SelectedValue != null)
            {
                int TecnicoID = Convert.ToInt32(lstTecnicosCreados.SelectedValue);
                MessageBoxResult Pregunta = MessageBox.Show("¿Estas Seguro De eliminar Tecnico?", "Mensaje", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (Pregunta == MessageBoxResult.Yes)
                {
                    EliminarTecnicos(TecnicoID);
                    MuestraTecnicos();
                    LimpiarCampos();
                    BloquearCampos();
                }
            }
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = this.Parent as TabItem;
            if (tabItem != null)
            {
                var tabControl = tabItem.Parent as TabControl;
                if (tabControl != null)
                {
                    tabControl.Items.Remove(tabItem); // Quita la pestaña
                }
            }
        }

        private void lstTecnicosCreados_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstTecnicosCreados.SelectedValue != null)
            {
                LlenarCampos();
            }
        }
    }
}
