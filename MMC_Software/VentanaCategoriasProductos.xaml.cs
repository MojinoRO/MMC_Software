using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMC_Software
{
    public partial class VentanaCategoriasProductos : UserControl
    {
        public VentanaCategoriasProductos()
        {
            InitializeComponent();
            muestraTipoGravados();
            muestraCategoriasCreadas();
        }

        private void muestraCategoriasCreadas()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string consulta = @"SELECT CONCAT(CodigoCategoria,'   ',NombreCategoria) AS infocompleta, CategoriasID FROM ConfCategoriasInve";
                    SqlDataAdapter adp = new SqlDataAdapter(consulta, conexion);
                    DataTable dataTable = new DataTable();
                    adp.Fill(dataTable);
                    lstCategorias.DisplayMemberPath = "infocompleta";
                    lstCategorias.SelectedValuePath = "CategoriasID";
                    lstCategorias.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message);
            }
        }

        private void muestraTipoGravados()
        {
            var TiposGravados = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(1, "Gravado"),
                new KeyValuePair<int, string>(2, "Excento"),
                new KeyValuePair<int, string>(3, "Excluido")
            };

            cmbTipoGravado.ItemsSource = TiposGravados;
            cmbTipoGravado.DisplayMemberPath = "Value";
            cmbTipoGravado.SelectedValuePath = "Key";
        }

        private void BloquearCampos()
        {
            txtCodigoCategoria.IsEnabled = false;
            txtNombreCategoria.IsEnabled = false;
            txtTarifaImpuesto.IsEnabled = false;
            cmbTipoGravado.IsEnabled = false;
            btnGuardar.IsEnabled = false;
        }

        private void DesbloquearCampos()
        {
            txtCodigoCategoria.IsEnabled = true;
            txtNombreCategoria.IsEnabled = true;
            txtTarifaImpuesto.IsEnabled = true;
            cmbTipoGravado.IsEnabled = true;
            btnGuardar.IsEnabled = true;
        }

        private void txtNombreCategoria_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox texto = sender as TextBox;
            if (texto != null)
            {
                int Posicion = texto.CaretIndex;
                string TextoOriginal = texto.Text;
                string TextoMayus = TextoOriginal.ToUpper();

                if (TextoOriginal != TextoMayus)
                {
                    texto.Text = TextoMayus;
                    texto.CaretIndex = Posicion;
                }
            }
        }

        private bool esnumero(string Texto)
        {
            return Texto.All(char.IsDigit);
        }

        private void txtTarifaImpuesto_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !esnumero(e.Text);
        }

        private void CrearCategorias()
        {
            try
            {
                string CodigoCategoria = txtCodigoCategoria.Text;
                string NombreCategoria = txtNombreCategoria.Text;
                int TipoGravado = Convert.ToInt32(cmbTipoGravado.SelectedValue);
                decimal Porcentaje = Convert.ToDecimal(txtTarifaImpuesto.Text);

                if (string.IsNullOrEmpty(CodigoCategoria) || string.IsNullOrEmpty(NombreCategoria))
                {
                    MessageBox.Show("Revisa Campos En Blanco En El Formulario", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();

                    string InsertCategorias = @"INSERT INTO ConfCategoriasInve (CodigoCategoria, NombreCategoria, TipoGravado, TarifaImpuesto)
                                                VALUES (@codigo, @nombre, @tipogravado, @impuesto);
                                                SELECT SCOPE_IDENTITY();";
                    SqlCommand cmd = new SqlCommand(InsertCategorias, conexion);
                    cmd.Parameters.AddWithValue("@codigo", CodigoCategoria);
                    cmd.Parameters.AddWithValue("@nombre", NombreCategoria);
                    cmd.Parameters.AddWithValue("@tipogravado", TipoGravado);
                    cmd.Parameters.AddWithValue("@impuesto", Porcentaje);
                    cmd.ExecuteNonQuery();
                    conexion.Close();
                    muestraCategoriasCreadas();
                    BloquearCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message);
            }
        }


        private void MontarInfoCategorias()
        {
            try
            {
                if (lstCategorias.SelectedValue != null)
                {
                    string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                    using (SqlConnection conexion = new SqlConnection(ConexionSql))
                    {
                        conexion.Open();
                        string ConsultaMontaInfo = @"select CategoriasID,CodigoCategoria,NombreCategoria,TipoGravado, TarifaImpuesto
                                                     from ConfCategoriasInve
                                                     where CategoriasID=@id";
                        SqlCommand cmd = new SqlCommand(ConsultaMontaInfo, conexion);
                        cmd.Parameters.AddWithValue("@id", lstCategorias.SelectedValue);
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            txtCodigoCategoria.Text = dt.Rows[0]["CodigoCategoria"].ToString();
                            txtNombreCategoria.Text = dt.Rows[0]["NombreCategoria"].ToString();
                            txtTarifaImpuesto.Text = dt.Rows[0]["TarifaImpuesto"].ToString();
                            int TipoGravado = Convert.ToInt32(dt.Rows[0]["TipoGravado"]);
                            muestraTipoGravados();
                            cmbTipoGravado.SelectedValue = TipoGravado;

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message);
            }
        }

        private void EliminarCategorias()
        {
            try
            {
                string NombreCategoria = txtNombreCategoria.Text;

                if (lstCategorias.SelectedValue != null)
                {
                    MessageBoxResult Pregunta = MessageBox.Show("Estas Seguro De Eliminar Esta categoria " + NombreCategoria, "MENSAJE", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (Pregunta == MessageBoxResult.Yes)
                    {
                        string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                        using (SqlConnection conexion = new SqlConnection(ConexionSql))
                        {
                            conexion.Open();
                            string DeleteCategorias = "DELETE FROM ConfCategoriasInve WHERE CategoriasID=@id";
                            SqlCommand cmd = new SqlCommand(DeleteCategorias, conexion);
                            cmd.Parameters.AddWithValue("@id", lstCategorias.SelectedValue);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Categoria Borrada Correctamente", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Debe seleccionar categoria a eliminar", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                muestraCategoriasCreadas();

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message);
            }
        }


        private void GuardaActualiza()
        {
            try
            {
                string CodigoCategoria = txtCodigoCategoria.Text;
                string NombreCategoria = txtNombreCategoria.Text;
                int TipoGravado = Convert.ToInt32(cmbTipoGravado.SelectedValue);
                decimal Porcentaje = Convert.ToDecimal(txtTarifaImpuesto.Text);
                int Id = Convert.ToInt32(lstCategorias.SelectedValue);

                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string ConsultaExiste = "select COUNT(*) from ConfCategoriasInve where CodigoCategoria=@codigo";
                    SqlCommand cmd = new SqlCommand(ConsultaExiste, conexion);
                    cmd.Parameters.AddWithValue("@codigo", CodigoCategoria);
                    int Cantidad = Convert.ToInt32(cmd.ExecuteScalar());
                    if (Cantidad > 0)
                    {
                        MessageBoxResult Pregunta = MessageBox.Show("Codigo Categoria Ya Existe ¿Quieres Actualizar Los Datos?", "MENSAJE", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (Pregunta == MessageBoxResult.Yes)
                        {
                            string Uptdate = @"update ConfCategoriasInve set
                                                CodigoCategoria=@codigo,
                                                NombreCategoria=@nombre,
                                                TipoGravado=@tipogravado,
                                                TarifaImpuesto=@impuesto where 
                                                CategoriasID=@id";
                            SqlCommand cmdUpdtae = new SqlCommand(Uptdate, conexion);
                            cmdUpdtae.Parameters.AddWithValue("@codigo", CodigoCategoria);
                            cmdUpdtae.Parameters.AddWithValue("@nombre", NombreCategoria);
                            cmdUpdtae.Parameters.AddWithValue("@tipogravado", TipoGravado);
                            cmdUpdtae.Parameters.AddWithValue("@impuesto", Porcentaje);
                            cmdUpdtae.Parameters.AddWithValue("@id", Id);
                            cmdUpdtae.ExecuteNonQuery();
                            conexion.Close();
                            muestraCategoriasCreadas();
                            BloquearCampos();

                        }
                        else
                        {
                            MessageBox.Show("Cambia el codigo para crear Una Nueva", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                            return;
                        }
                    }
                    else
                    {
                        CrearCategorias();
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message);
            }
        }

        // ------------------- BOTONES ------------------- //

        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {
            DesbloquearCampos();
            txtCodigoCategoria.Text = "";
            txtNombreCategoria.Text = "";
            txtTarifaImpuesto.Text = "";
            btnGuardar.IsEnabled = true;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            GuardaActualiza();
        }

        private void lstCategorias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MontarInfoCategorias();
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (lstCategorias.SelectedValue != null)
            {
                DesbloquearCampos();
            }
            else
            {
                MessageBox.Show("Debe Seleccionar categoria a modificar", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            EliminarCategorias();
        }

        private void btnConfiguracionContable_Click(object sender, RoutedEventArgs e)
        {
            if (lstCategorias.SelectedValue != null)
            {
                int id = Convert.ToInt32(lstCategorias.SelectedValue);
                string codigo = txtCodigoCategoria.Text;
                string nombre = txtNombreCategoria.Text;

                VentanaCuentasCategorias ventana = new VentanaCuentasCategorias(id, codigo, nombre);
                ventana.ShowDialog();
            }
            else
            {
                MessageBox.Show("Debe seleccionar una categoría", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void Cerrar()
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

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Cerrar();
        }
    }
}
