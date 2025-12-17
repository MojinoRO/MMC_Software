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
    /// Lógica de interacción para VentanaCrearTipoVehiculos.xaml
    /// </summary>
    public partial class VentanaCrearTipoVehiculos : UserControl
    {
        public VentanaCrearTipoVehiculos()
        {
            InitializeComponent();
            BloquearCampos();
            EstadoTipoVehiculos();
            ListadoTiposVehiculos();
        }

        private void BloquearCampos()
        {
            txtCodigo.IsEnabled = false;
            txtNombreTipo.IsEnabled = false;
            cmbEstado.IsEnabled = false;
        }

        private void DesbloquearCampos(bool editarCodigo = true)
        {
            txtCodigo.IsEnabled = editarCodigo;
            txtNombreTipo.IsEnabled = true;
            cmbEstado.IsEnabled = true;
        }

        private void LimpiarCampos()
        {
            txtCodigo.Clear();
            txtNombreTipo.Clear();
            cmbEstado.SelectedIndex = -1;
            lstTiposVehiculos.SelectedValue = null;
        }

        private void EstadoTipoVehiculos()
        {
            var estado = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("Activo", 0),
                new KeyValuePair<string, int>("Inactivo", 1)
            };
            cmbEstado.ItemsSource = estado;
            cmbEstado.DisplayMemberPath = "Key";
            cmbEstado.SelectedValuePath = "Value";
        }

        private void ListadoTiposVehiculos()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string consulta = @"SELECT CONCAT(CodigoTipoVehiculo, ' ', NombreTipoVehiculo) AS infocompleta, TipoVehiculoID 
                                        FROM TallTiposVehiculos";
                    using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        lstTiposVehiculos.DisplayMemberPath = "infocompleta";
                        lstTiposVehiculos.SelectedValuePath = "TipoVehiculoID";
                        lstTiposVehiculos.ItemsSource = dt.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LlenarCampos()
        {
            try
            {
                if (lstTiposVehiculos.SelectedValue == null) return;

                int TipoVehiculoID = Convert.ToInt32(lstTiposVehiculos.SelectedValue);
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Consulta = @"SELECT CodigoTipoVehiculo, NombreTipoVehiculo, Estado  
                                        FROM TallTiposVehiculos 
                                        WHERE TipoVehiculoID=@filtro";
                    using (SqlCommand cmd = new SqlCommand(Consulta, conexion))
                    {
                        cmd.Parameters.AddWithValue("@filtro", TipoVehiculoID);
                        using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adp.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                txtCodigo.Text = dt.Rows[0]["CodigoTipoVehiculo"].ToString();
                                txtNombreTipo.Text = dt.Rows[0]["NombreTipoVehiculo"].ToString();
                                cmbEstado.SelectedValue = Convert.ToInt32(dt.Rows[0]["Estado"]);
                                BloquearCampos();
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

        private bool CodigoExiste(string codigo)
        {
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
            using (SqlConnection conexion = new SqlConnection(ConexionSql))
            {
                conexion.Open();
                string Consulta = @"SELECT COUNT(*) FROM TallTiposVehiculos WHERE CodigoTipoVehiculo=@filtro";
                using (SqlCommand cmd = new SqlCommand(Consulta, conexion))
                {
                    cmd.Parameters.AddWithValue("@filtro", codigo);
                    int Cantidad = (int)cmd.ExecuteScalar();
                    return Cantidad > 0;
                }
            }
        }

        private void CrearTiposVehiculos()
        {
            try
            {
                string Codigo = txtCodigo.Text.Trim();
                string Nombre = txtNombreTipo.Text.Trim();
                int Estado = Convert.ToInt32(cmbEstado.SelectedValue);

                if (string.IsNullOrWhiteSpace(Codigo) || string.IsNullOrWhiteSpace(Nombre))
                {
                    MessageBox.Show("Revisa campos en blanco", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Insert = @"INSERT INTO TallTiposVehiculos (CodigoTipoVehiculo, NombreTipoVehiculo, Estado) 
                                      VALUES (@codigo, @nombre, @estado)";
                    using (SqlCommand cmd = new SqlCommand(Insert, conexion))
                    {
                        cmd.Parameters.AddWithValue("@codigo", Codigo);
                        cmd.Parameters.AddWithValue("@nombre", Nombre);
                        cmd.Parameters.AddWithValue("@estado", Estado);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Registro creado correctamente", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActualizarTiposVehiculos(int TipoVehiculoID, string Nombre, int Estado)
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Update = @"UPDATE TallTiposVehiculos 
                                      SET NombreTipoVehiculo=@nombre, Estado=@estado 
                                      WHERE TipoVehiculoID=@id";
                    using (SqlCommand cmd = new SqlCommand(Update, conexion))
                    {
                        cmd.Parameters.AddWithValue("@nombre", Nombre);
                        cmd.Parameters.AddWithValue("@estado", Estado);
                        cmd.Parameters.AddWithValue("@id", TipoVehiculoID);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Registro actualizado correctamente", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EliminarTiposVehiculos(int TipoVehiculoID)
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Delete = @"DELETE FROM TallTiposVehiculos WHERE TipoVehiculoID=@id";
                    using (SqlCommand cmd = new SqlCommand(Delete, conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", TipoVehiculoID);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Registro eliminado correctamente", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // -------------------
        // EVENTOS DE BOTONES
        // -------------------

        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {
            DesbloquearCampos(true);
            LimpiarCampos();
            txtCodigo.Focus();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Codigo = txtCodigo.Text.Trim();
                string Nombre = txtNombreTipo.Text.Trim();
                int Estado = Convert.ToInt32(cmbEstado.SelectedValue);

                if (string.IsNullOrWhiteSpace(Codigo) || string.IsNullOrWhiteSpace(Nombre))
                {
                    MessageBox.Show("Revisa campos en blanco", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (lstTiposVehiculos.SelectedValue != null) // UPDATE
                {
                    int TipoVehiculoID = Convert.ToInt32(lstTiposVehiculos.SelectedValue);
                    ActualizarTiposVehiculos(TipoVehiculoID, Nombre, Estado);
                }
                else // INSERT
                {
                    if (CodigoExiste(Codigo))
                    {
                        MessageBox.Show("Código ya existe", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    CrearTiposVehiculos();
                }

                ListadoTiposVehiculos();
                LimpiarCampos();
                BloquearCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (lstTiposVehiculos.SelectedValue != null)
            {
                DesbloquearCampos(false); // Código NO editable
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (lstTiposVehiculos.SelectedValue != null)
            {
                int TipoVehiculoID = Convert.ToInt32(lstTiposVehiculos.SelectedValue);
                if (MessageBox.Show("¿Seguro que deseas eliminar este registro?", "Confirmar",
                                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    EliminarTiposVehiculos(TipoVehiculoID);
                    ListadoTiposVehiculos();
                    LimpiarCampos();
                    BloquearCampos();
                }
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarCampos();
            BloquearCampos();
        }

        // -------------------
        // EVENTOS DE CAMPOS
        // -------------------

        private void lstTiposVehiculos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstTiposVehiculos.SelectedValue != null)
            {
                LlenarCampos();
            }
        }

        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtNombreTipo.Focus();
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
    }
}
