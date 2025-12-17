using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaCreacionAlmacen.xaml
    /// </summary>
    public partial class VentanaCreacionAlmacen : UserControl
    {
        public VentanaCreacionAlmacen()
        {
            InitializeComponent();
            MuestraAlmacen();
        }

        #region --- Eventos de Botones ---

        private void ButtonCerrar_Click(object sender, RoutedEventArgs e)
        {
            var TabAbierto = this.Parent as TabItem;
            if (TabAbierto != null)
            {
                var Item = TabAbierto.Parent as TabControl;
                if (Item != null)
                {
                    Item.Items.Remove(TabAbierto);
                }
            }
        }

        private void ButtonCrear_Click(object sender, RoutedEventArgs e)
        {
            txtCodigoAlmacen.Clear();
            txtNobreAlmacen.Clear();
            txtCodigoAlmacen.IsEnabled = true;
            txtNobreAlmacen.IsEnabled = true;
            txtCodigoAlmacen.Focus();
        }

        private void ButtonGuardar_Click(object sender, RoutedEventArgs e)
        {
            GuardarOActualizarAlmacen();
        }

        private void ButtonEliminar_Click(object sender, RoutedEventArgs e)
        {
            EliminarAlmacen();
        }

        private void ButtonModifica_Click(object sender, RoutedEventArgs e)
        {
            if (listboxAlmacenes.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar un almacén para modificar.", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            txtCodigoAlmacen.IsEnabled = true;
            txtNobreAlmacen.IsEnabled = true;
        }

        #endregion

        #region --- Métodos Principales ---

        /// <summary>
        /// Inserta un nuevo almacén en la base de datos.
        /// </summary>
        private void CreaAlmacen()
        {
            try
            {
                string codigo = txtCodigoAlmacen.Text.Trim();
                string nombre = txtNobreAlmacen.Text.Trim();

                if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(nombre))
                {
                    MessageBox.Show("Debe ingresar código y nombre para crear un almacén.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string query = "INSERT INTO ConfAlmacen (CodigoAlmacen, NombreAlmacen) VALUES (@codigo, @nombre)";
                string conexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(conexionSql))
                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    conexion.Open();
                    cmd.Parameters.AddWithValue("@codigo", codigo);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Almacén creado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                MuestraAlmacen();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear el almacén: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Muestra todos los almacenes en el ListBox.
        /// </summary>
        private void MuestraAlmacen()
        {
            try
            {
                string query = "SELECT IdAlmacen, CONCAT(CodigoAlmacen, ' - ', NombreAlmacen) AS DatosAlmacen FROM ConfAlmacen ORDER BY NombreAlmacen";
                string conexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(conexionSql))
                using (SqlDataAdapter adp = new SqlDataAdapter(query, conexion))
                {
                    DataTable dt = new DataTable();
                    adp.Fill(dt);

                    listboxAlmacenes.DisplayMemberPath = "DatosAlmacen";
                    listboxAlmacenes.SelectedValuePath = "IdAlmacen";
                    listboxAlmacenes.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar almacenes: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Guarda un nuevo almacén o actualiza si ya existe.
        /// </summary>
        private void GuardarOActualizarAlmacen()
        {
            try
            {
                string codigo = txtCodigoAlmacen.Text.Trim();
                string nombre = txtNobreAlmacen.Text.Trim();

                if (string.IsNullOrWhiteSpace(codigo))
                {
                    MessageBox.Show("El código del almacén no puede estar vacío.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string conexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(conexionSql))
                {
                    conexion.Open();

                    // Verificar si existe
                    string consultaExiste = "SELECT COUNT(*) FROM ConfAlmacen WHERE CodigoAlmacen = @codigo";
                    using (SqlCommand cmdExiste = new SqlCommand(consultaExiste, conexion))
                    {
                        cmdExiste.Parameters.AddWithValue("@codigo", codigo);
                        int existe = (int)cmdExiste.ExecuteScalar();

                        if (existe > 0)
                        {
                            // Verificar si hay cambios
                            string consultaNombre = "SELECT NombreAlmacen FROM ConfAlmacen WHERE CodigoAlmacen = @codigo";
                            SqlCommand cmdNombre = new SqlCommand(consultaNombre, conexion);
                            cmdNombre.Parameters.AddWithValue("@codigo", codigo);
                            string nombreActual = cmdNombre.ExecuteScalar()?.ToString();

                            if (nombreActual != nombre)
                            {
                                string update = "UPDATE ConfAlmacen SET NombreAlmacen = @nombre WHERE CodigoAlmacen = @codigo";
                                SqlCommand cmdUpdate = new SqlCommand(update, conexion);
                                cmdUpdate.Parameters.AddWithValue("@nombre", nombre);
                                cmdUpdate.Parameters.AddWithValue("@codigo", codigo);
                                cmdUpdate.ExecuteNonQuery();

                                MessageBox.Show("Almacén actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("No hay cambios que guardar.", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        else
                        {
                            CreaAlmacen();
                        }
                    }
                }

                MuestraAlmacen();
                txtCodigoAlmacen.IsEnabled = false;
                txtNobreAlmacen.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el almacén: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Elimina un almacén seleccionado.
        /// </summary>
        private void EliminarAlmacen()
        {
            try
            {
                if (listboxAlmacenes.SelectedValue == null)
                {
                    MessageBox.Show("Debe seleccionar un almacén para eliminar.", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                MessageBoxResult confirm = MessageBox.Show("¿Está seguro de eliminar este almacén?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirm != MessageBoxResult.Yes)
                    return;

                string id = listboxAlmacenes.SelectedValue.ToString();
                string conexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(conexionSql))
                {
                    conexion.Open();
                    string delete = "DELETE FROM ConfAlmacen WHERE IdAlmacen = @id";
                    SqlCommand cmd = new SqlCommand(delete, conexion);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Almacén eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                MuestraAlmacen();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el almacén: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region --- Métodos Auxiliares ---

        private void MontaInfoAlmacen()
        {
            try
            {
                if (listboxAlmacenes.SelectedValue == null)
                    return;

                string conexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                string query = "SELECT CodigoAlmacen, NombreAlmacen FROM ConfAlmacen WHERE IdAlmacen = @id";

                using (SqlConnection conexion = new SqlConnection(conexionSql))
                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", listboxAlmacenes.SelectedValue);
                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtCodigoAlmacen.Text = reader["CodigoAlmacen"].ToString();
                            txtNobreAlmacen.Text = reader["NombreAlmacen"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al mostrar información: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void listboxAlmacenes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MontaInfoAlmacen();
        }

        private void LimpiarCampos()
        {
            txtCodigoAlmacen.Clear();
            txtNobreAlmacen.Clear();
            txtCodigoAlmacen.IsEnabled = false;
            txtNobreAlmacen.IsEnabled = false;
        }

        #endregion
    }
}
