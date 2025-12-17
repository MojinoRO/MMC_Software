using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaCentros_SubCentro.xaml
    /// Versión corregida y con mejores prácticas para .NET Framework 4.8
    /// </summary>
    public partial class VentanaCentros_SubCentro : UserControl
    {
        public VentanaCentros_SubCentro()
        {
            InitializeComponent();
            CargarCentrosCosto();
            CargarSubCentros();
        }

        // Obtiene la cadena de conexión centralizada
        private string GetConnectionString()
        {
            return ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
        }

        #region --- Centros de costo ---

        private void CargarCentrosCosto()
        {
            try
            {
                string conexionStr = GetConnectionString();
                string sql = "SELECT * FROM ConfCentroCostos ORDER BY CodigoCosto";

                using (SqlConnection cn = new SqlConnection(conexionStr))
                using (SqlDataAdapter da = new SqlDataAdapter(sql, cn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    listboxCentrosCosto.DisplayMemberPath = "NombreCentroCosto";
                    listboxCentrosCosto.SelectedValuePath = "IdCentroCosto";
                    listboxCentrosCosto.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando centros de costo: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MontarInfoCentro()
        {
            try
            {
                if (!TryGetSelectedId(listboxCentrosCosto, out int centroId))
                {
                    MessageBox.Show("Selecciona un centro de costo válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string sql = "SELECT Nit, CodigoCosto, NombreCentroCosto FROM ConfCentroCostos WHERE IdCentroCosto = @CentroCostoID";
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@CentroCostoID", centroId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            txtNitCentroCosto.Text = dt.Rows[0]["Nit"].ToString();
                            txtCodigoCentroCosto.Text = dt.Rows[0]["CodigoCosto"].ToString();
                            txtNombreCentroCosto.Text = dt.Rows[0]["NombreCentroCosto"].ToString();
                        }
                        else
                        {
                            MessageBox.Show("No se encontró información del centro de costo seleccionado.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error obteniendo información del centro: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CrearCentroCostos()
        {
            try
            {
                string nit = txtNitCentroCosto.Text.Trim();
                string codigo = txtCodigoCentroCosto.Text.Trim();
                string nombre = txtNombreCentroCosto.Text.Trim();

                if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(nombre))
                {
                    MessageBox.Show("Código y Nombre son obligatorios.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string conexionStr = GetConnectionString();

                using (SqlConnection cn = new SqlConnection(conexionStr))
                {
                    cn.Open();

                    // Verificar duplicado
                    using (SqlCommand chk = new SqlCommand("SELECT COUNT(*) FROM ConfCentroCostos WHERE CodigoCosto = @codigo", cn))
                    {
                        chk.Parameters.AddWithValue("@codigo", codigo);
                        int existe = Convert.ToInt32(chk.ExecuteScalar());
                        if (existe > 0)
                        {
                            MessageBox.Show("Ya existe un centro de costo con ese código.", "Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }

                    // Insertar
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO ConfCentroCostos (Nit, CodigoCosto, NombreCentroCosto) VALUES (@nit, @codigoCosto, @nombrecentrocosto)", cn))
                    {
                        cmd.Parameters.AddWithValue("@nit", nit);
                        cmd.Parameters.AddWithValue("@codigoCosto", codigo);
                        cmd.Parameters.AddWithValue("@nombrecentrocosto", nombre);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Centro de costo creado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // refrescar lista y limpiar
                CargarCentrosCosto();
                txtNitCentroCosto.Text = txtCodigoCentroCosto.Text = txtNombreCentroCosto.Text = string.Empty;
                txtNitCentroCosto.IsEnabled = txtCodigoCentroCosto.IsEnabled = txtNombreCentroCosto.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear centro de costo: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EliminarCentroCosto()
        {
            try
            {
                if (!TryGetSelectedId(listboxCentrosCosto, out int id))
                {
                    MessageBox.Show("Debes seleccionar un centro de costo.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var r = MessageBox.Show("¿Estás seguro que deseas eliminar el centro de costo seleccionado?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (r != MessageBoxResult.Yes) return;

                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                using (SqlCommand cmd = new SqlCommand("DELETE FROM ConfCentroCostos WHERE IdCentroCosto = @ID", cn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Centro de costo eliminado.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                CargarCentrosCosto();
                CargarSubCentros(); // refrescar subcentros por si dependían del centro eliminado
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar centro de costo: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region --- Subcentros ---

        private void CargarSubCentros()
        {
            try
            {
                string sql = @"
                    SELECT SCC.SubCentroCostoID,
                           CONCAT(CC.CodigoCosto, '-', SCC.CodigoSubcentro, ' ', SCC.NombreSubCentroCosto) AS InfoCompleta
                      FROM ConfCentroCostos CC
                      INNER JOIN ConfSubCentroCosto SCC ON SCC.IdCentroCosto = CC.IdCentroCosto
                     ORDER BY CC.CodigoCosto, SCC.CodigoSubcentro";

                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ListBoxSCC.DisplayMemberPath = "InfoCompleta";
                    ListBoxSCC.SelectedValuePath = "SubCentroCostoID";
                    ListBoxSCC.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando subcentros: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MontarInfoSCC()
        {
            try
            {
                if (!TryGetSelectedId(ListBoxSCC, out int subId))
                {
                    MessageBox.Show("Selecciona un subcentro válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string sql = "SELECT CodigoSubcentro, NombreSubCentroCosto, IdCentroCosto FROM ConfSubCentroCosto WHERE SubCentroCostoID = @ID";
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@ID", subId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            txtCodigoSubCentroCosto.Text = dt.Rows[0]["CodigoSubcentro"].ToString();
                            txtNombreSubCentroCosto.Text = dt.Rows[0]["NombreSubCentroCosto"].ToString();

                            txtCodigoSubCentroCosto.IsEnabled = false;
                            txtNombreSubCentroCosto.IsEnabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error obteniendo información del subcentro: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CrearSubCC()
        {
            string codigoSubCentro = txtCodigoSubCentroCosto.Text.Trim();
            string nombreSubCentro = txtNombreSubCentroCosto.Text.Trim();

            if (string.IsNullOrWhiteSpace(codigoSubCentro) || string.IsNullOrWhiteSpace(nombreSubCentro))
            {
                MessageBox.Show("Los campos Código y Nombre del SubCentro no pueden estar vacíos.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TryGetSelectedId(listboxCentrosCosto, out int centroId))
            {
                MessageBox.Show("Debe seleccionar un Centro de Costo para asociar el SubCentro.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    cn.Open();

                    // Validar existencia
                    using (SqlCommand chk = new SqlCommand("SELECT COUNT(*) FROM ConfSubCentroCosto WHERE CodigoSubcentro = @codigo AND IdCentroCosto = @idCentro", cn))
                    {
                        chk.Parameters.AddWithValue("@codigo", codigoSubCentro);
                        chk.Parameters.AddWithValue("@idCentro", centroId);
                        int existe = Convert.ToInt32(chk.ExecuteScalar());
                        if (existe > 0)
                        {
                            MessageBox.Show("Ya existe un SubCentro de Costo con ese código para el centro seleccionado.", "Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }

                    // Insertar
                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO ConfSubCentroCosto (CodigoSubcentro, NombreSubCentroCosto, IdCentroCosto) 
                        VALUES (@codigo, @nombre, @id)", cn))
                    {
                        cmd.Parameters.AddWithValue("@codigo", codigoSubCentro);
                        cmd.Parameters.AddWithValue("@nombre", nombreSubCentro);
                        cmd.Parameters.AddWithValue("@id", centroId);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("SubCentro de Costo creado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // refrescar y limpiar
                CargarSubCentros();
                txtCodigoSubCentroCosto.Text = txtNombreSubCentroCosto.Text = string.Empty;
                txtCodigoSubCentroCosto.IsEnabled = txtNombreSubCentroCosto.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear SubCentro: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EliminarSubCentro()
        {
            try
            {
                if (!TryGetSelectedId(ListBoxSCC, out int id))
                {
                    MessageBox.Show("Debe seleccionar un SubCentro para eliminar.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var r = MessageBox.Show("¿Estás seguro de eliminar el SubCentro de costo seleccionado?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (r != MessageBoxResult.Yes) return;

                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                using (SqlCommand cmd = new SqlCommand("DELETE FROM ConfSubCentroCosto WHERE SubCentroCostoID = @id", cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("SubCentro eliminado.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                CargarSubCentros();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar SubCentro: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region --- Helpers / Eventos UI ---

        // Intenta obtener el SelectedValue convertido a entero desde un ListBox
        private bool TryGetSelectedId(ListBox listBox, out int id)
        {
            id = 0;
            if (listBox?.SelectedValue == null) return false;

            try
            {
                // SelectedValue puede venir como DataRowView o string/int dependiendo del ItemsSource
                if (int.TryParse(listBox.SelectedValue.ToString(), out int parsed))
                {
                    id = parsed;
                    return true;
                }
            }
            catch
            {
                // Ignorar parse exceptions y devolver false
            }

            return false;
        }

        // Encabezado / cerrar ventana (no modifica nada por ahora)
        private void ButtonCerrar_Click(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow is ModuloAdministracion mainWindow)
            {
                // mainWindow.PanelMenus.Content = null; // si quieres cerrar el control, usar esa línea
            }
        }

        // Eventos UI enlazados al XAML (se mantienen nombres originales para no romper referencias)
        private void txtCentrosCosto_MouseDoubleClick(object sender, MouseButtonEventArgs e) => MontarInfoCentro();
        private void ButtonEliminar_Click(object sender, RoutedEventArgs e) => EliminarCentroCosto();
        private void ButtonCrear_Click(object sender, RoutedEventArgs e)
        {
            // Habilitar campos para crear nuevo
            txtNitCentroCosto.IsEnabled = true;
            txtCodigoCentroCosto.IsEnabled = true;
            txtNombreCentroCosto.IsEnabled = true;

            txtNitCentroCosto.Text = string.Empty;
            txtCodigoCentroCosto.Text = string.Empty;
            txtNombreCentroCosto.Text = string.Empty;
        }
        private void ButtonModifica_Click(object sender, RoutedEventArgs e)
        {
            // Habilitar campos para modificar (si se requiere, puedes cargar el registro primero)
            txtNitCentroCosto.IsEnabled = true;
            txtCodigoCentroCosto.IsEnabled = true;
            txtNombreCentroCosto.IsEnabled = true;
        }
        private void ButtonGuardar_Click(object sender, RoutedEventArgs e) => CrearCentroCostos();

        // Subcentros - UI events
        private void ButtonSubGuardar_Click_1(object sender, RoutedEventArgs e) => CrearSubCC();
        private void ButtonSubCrear_Click(object sender, RoutedEventArgs e)
        {
            txtCodigoSubCentroCosto.IsEnabled = true;
            txtNombreSubCentroCosto.IsEnabled = true;

            txtCodigoSubCentroCosto.Text = string.Empty;
            txtNombreSubCentroCosto.Text = string.Empty;
        }
        private void ListBoxSCC_MouseDoubleClick(object sender, MouseButtonEventArgs e) => MontarInfoSCC();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Modificar subcentro seleccionado
            if (!TryGetSelectedId(ListBoxSCC, out int _))
            {
                MessageBox.Show("Debe seleccionar SubcentroCosto a modificar", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            txtCodigoSubCentroCosto.IsEnabled = true;
            txtNombreSubCentroCosto.IsEnabled = true;
            // no deshabilitar la lista para mantener navegación
        }
        private void Button_Click_1(object sender, RoutedEventArgs e) => EliminarSubCentro();

        #endregion

        private void ButtonSalir_Click(object sender, RoutedEventArgs e)
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
    }
}
