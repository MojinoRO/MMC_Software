using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaMarcas.xaml
    /// </summary>
    public partial class VentanaMarcas : UserControl
    {
        public VentanaMarcas()
        {
            InitializeComponent();
            MuestraMarcas();
        }

        private bool EsNumero(string texto)
        {
            return texto.All(char.IsDigit);
        }

        private void txtCodigoMarca_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !EsNumero(e.Text);
        }

        private void txtNombreMarca_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox Texto = sender as TextBox;
            if (Texto != null)
            {
                int Posicion = Texto.CaretIndex;
                string TextoOriginal = Texto.Text;
                string TextoMayuscula = TextoOriginal.ToUpper();
                if (TextoOriginal != TextoMayuscula)
                {
                    Texto.Text = TextoMayuscula;
                    Texto.CaretIndex = Posicion;
                }
            }
        }

        private void bloquearCampos()
        {
            txtCodigoMarca.IsEnabled = false;
            txtNombreMarca.IsEnabled = false;
            btnGuardar.IsEnabled = false;
        }

        private void DesbloquearCampos()
        {
            txtCodigoMarca.IsEnabled = true;
            txtNombreMarca.IsEnabled = true;
            btnGuardar.IsEnabled = true;
        }

        private void LlenarInfoMarcas()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Consulta = "select  CodigoMarca, NombreMarca from ConfMarcas where MarcasID=@id";
                    SqlCommand cmd = new SqlCommand(Consulta, conexion);
                    cmd.Parameters.AddWithValue("@id", lstMarcas.SelectedValue);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adp.Fill(dataTable);
                    if (dataTable.Rows.Count > 0)
                    {
                        txtCodigoMarca.Text = dataTable.Rows[0]["CodigoMarca"].ToString();
                        txtNombreMarca.Text = dataTable.Rows[0]["NombreMarca"].ToString();
                    }
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void MuestraMarcas()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Consulta = "select CONCAT(CodigoMarca,'  ',NombreMarca) as InfoCompleta, MarcasID from ConfMarcas";
                    SqlDataAdapter adp = new SqlDataAdapter(Consulta, conexion);
                    DataTable dataTable = new DataTable();
                    adp.Fill(dataTable);
                    lstMarcas.DisplayMemberPath = "InfoCompleta";
                    lstMarcas.SelectedValuePath = "MarcasID";
                    lstMarcas.ItemsSource = dataTable.DefaultView;
                    conexion.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GuardaActualiza()
        {
            string codigo = txtCodigoMarca.Text.Trim();
            string nombre = txtNombreMarca.Text.Trim();

            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Revisa campos en blanco en el formulario", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();

                    // Si estoy editando
                    if (lstMarcas.SelectedValue != null)
                    {
                        string checkQuery = "SELECT COUNT(*) FROM ConfMarcas WHERE CodigoMarca = @codigo AND MarcasID <> @id";
                        SqlCommand cmdCheck = new SqlCommand(checkQuery, conexion);
                        cmdCheck.Parameters.AddWithValue("@codigo", codigo);
                        cmdCheck.Parameters.AddWithValue("@id", lstMarcas.SelectedValue);
                        int existe = Convert.ToInt32(cmdCheck.ExecuteScalar());

                        if (existe > 0)
                        {
                            MessageBox.Show("Ya existe otra marca con ese código", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        // Actualiza
                        string updateQuery = "UPDATE ConfMarcas SET CodigoMarca=@codigo, NombreMarca=@nombre WHERE MarcasID=@id";
                        SqlCommand cmdUpdate = new SqlCommand(updateQuery, conexion);
                        cmdUpdate.Parameters.AddWithValue("@codigo", codigo);
                        cmdUpdate.Parameters.AddWithValue("@nombre", nombre);
                        cmdUpdate.Parameters.AddWithValue("@id", lstMarcas.SelectedValue);
                        cmdUpdate.ExecuteNonQuery();
                    }
                    else // Estoy creando
                    {
                        string checkQuery = "SELECT COUNT(*) FROM ConfMarcas WHERE CodigoMarca = @codigo";
                        SqlCommand cmdCheck = new SqlCommand(checkQuery, conexion);
                        cmdCheck.Parameters.AddWithValue("@codigo", codigo);
                        int existe = Convert.ToInt32(cmdCheck.ExecuteScalar());

                        if (existe > 0)
                        {
                            MessageBox.Show("Ya existe una marca con ese código", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        // Inserta
                        string insertQuery = "INSERT INTO ConfMarcas (CodigoMarca, NombreMarca) VALUES (@codigo, @nombre)";
                        SqlCommand cmdInsert = new SqlCommand(insertQuery, conexion);
                        cmdInsert.Parameters.AddWithValue("@codigo", codigo);
                        cmdInsert.Parameters.AddWithValue("@nombre", nombre);
                        cmdInsert.ExecuteNonQuery();
                    }

                    MuestraMarcas();
                    bloquearCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
            }
        }


        private void CrearMarcas()
        {
            string codigo = txtCodigoMarca.Text;
            string Nombre = txtNombreMarca.Text;
            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(Nombre))
            {
                MessageBox.Show("Revisa campos en blanco en el formulario", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else
            {
                try
                {
                    string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                    using (SqlConnection conexion = new SqlConnection(ConexionSql))
                    {
                        conexion.Open();
                        string InsertMarcas = @"insert into ConfMarcas(CodigoMarca,NombreMarca) values(@codigo,@nombre)";
                        SqlCommand cmd = new SqlCommand(InsertMarcas, conexion);
                        cmd.Parameters.AddWithValue("@codigo", codigo);
                        cmd.Parameters.AddWithValue("@nombre", Nombre);
                        cmd.ExecuteNonQuery();
                        bloquearCampos();
                        MuestraMarcas();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR" + ex.Message);
                }

            }
        }

        private void EliminaMarca()
        {
            try
            {
                int ID = Convert.ToInt32(lstMarcas.SelectedValue);
                string Nombre = txtNombreMarca.Text;

                if (lstMarcas.SelectedValue == null)
                {
                    MessageBox.Show("Debe seleccionar marca a eliminar", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBoxResult Pregunta = MessageBox.Show("Estas Seguro de Eliminar Marca  " + Nombre, "MENSAJE", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (Pregunta == MessageBoxResult.Yes)
                    {
                        string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                        using (SqlConnection conexion = new SqlConnection(ConexionSql))
                        {
                            conexion.Open();
                            string DeleteMarcas = "delete from ConfMarcas where MarcasID=@id";
                            SqlCommand cmd = new SqlCommand(DeleteMarcas, conexion);
                            cmd.Parameters.AddWithValue("@id", ID);
                            cmd.ExecuteNonQuery();
                            MuestraMarcas();
                            bloquearCampos();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Proceso Cancelado Por El usuario", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {
            DesbloquearCampos();
            txtCodigoMarca.Text = "";
            txtNombreMarca.Text = "";
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            DesbloquearCampos();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            GuardaActualiza();
        }



        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            EliminaMarca();
        }

        private void lstMarcas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LlenarInfoMarcas();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
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
    }
}
