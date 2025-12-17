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
    /// Lógica de interacción para VentanaSubCategorias.xaml
    /// </summary>
    public partial class VentanaSubCategorias : UserControl
    {
        public VentanaSubCategorias()
        {
            InitializeComponent();
            muestraCategorias();
            BloquearCampos();
            MuestraSubCategorias();
        }

        private void BloquearCampos()
        {
            txtCodigo.IsEnabled = false;
            txtNombre.IsEnabled = false;
            cbCategorias.IsEnabled = false;
            btnGuardar.IsEnabled = false;
        }

        private void DesbloquearCampos()
        {
            txtCodigo.IsEnabled = true;
            txtNombre.IsEnabled = true;
            cbCategorias.IsEnabled = true;
            btnGuardar.IsEnabled = true;
        }

        private void MuestraSubCategorias()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string ConsultaSubCategorias = @"select CONCAT(CA.CodigoCategoria,'-',SCA.CodigoSubCategoria,'  ',NombreSubCategoria) as Infocompleta,SCA.SubCategoriaID
                                                    from ConfSubCategorias SCA, ConfCategoriasInve CA
                                                    where CA.CategoriasID=SCA.CategoriasID";
                    SqlDataAdapter adp = new SqlDataAdapter(ConsultaSubCategorias, conexion);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    lstSubCategorias.DisplayMemberPath = "Infocompleta";
                    lstSubCategorias.SelectedValuePath = "SubCategoriaID";
                    lstSubCategorias.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message);
            }
        }

        private void EliminarSubcategorias()
        {
            try
            {
                int SubcategoriaID = Convert.ToInt32(lstSubCategorias.SelectedValue);
                string NombreCategoria = txtNombre.Text;

                if (lstSubCategorias.SelectedValue != null)
                {
                    MessageBoxResult Pregunta = MessageBox.Show("Esta Seguro De Eliminar  " + NombreCategoria, "MENSAJE", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (Pregunta == MessageBoxResult.Yes)
                    {
                        string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                        using (SqlConnection conexion = new SqlConnection(ConexionSql))
                        {
                            conexion.Open();
                            string deleteSubCategoria = "delete from ConfSubCategorias where SubCategoriaID=@id";
                            SqlCommand cmd = new SqlCommand(deleteSubCategoria, conexion);
                            cmd.Parameters.AddWithValue("@id", SubcategoriaID);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("SubCategoria Eliminada Correctamente", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
                            MuestraSubCategorias();
                            BloquearCampos();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Proceso Cancelado", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message);
            }
        }
        private void muestraCategorias()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string ConsultCategorias = "select  concat(CodigoCategoria,'  ',NombreCategoria )AS INFOCOMPLETA , CategoriasID  from ConfCategoriasInve ";
                    SqlDataAdapter adp = new SqlDataAdapter(ConsultCategorias, conexion);
                    DataTable dataTable = new DataTable();
                    adp.Fill(dataTable);
                    cbCategorias.DisplayMemberPath = "INFOCOMPLETA";
                    cbCategorias.SelectedValuePath = "CategoriasID";
                    cbCategorias.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message);
            }
        }

        private void llenarInfoCategorias()
        {
            try
            {
                int Id = Convert.ToInt32(lstSubCategorias.SelectedValue);

                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string ConsultallenarInfo = "Select  CategoriasID, CodigoSubCategoria,NombreSubCategoria from ConfSubCategorias  where SubCategoriaID=@id";
                    SqlCommand cmd = new SqlCommand(ConsultallenarInfo, conexion);
                    cmd.Parameters.AddWithValue("@id", Id);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        txtCodigo.Text = dt.Rows[0]["CodigoSubCategoria"].ToString();
                        txtNombre.Text = dt.Rows[0]["NombreSubCategoria"].ToString();
                        int CategoriaID = Convert.ToInt32(dt.Rows[0]["CategoriasID"]);
                        muestraCategorias();
                        cbCategorias.SelectedValue = CategoriaID;
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message);
            }
        }
        private void txtNombre_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox Texto = sender as TextBox;

            if (Texto != null)
            {
                int Posicion = Texto.CaretIndex;
                string TextOriginal = Texto.Text;
                string TextoMayuscula = TextOriginal.ToUpper();
                if (TextOriginal != TextoMayuscula)
                {
                    Texto.Text = TextoMayuscula;
                    Texto.CaretIndex = Posicion;
                }
            }
        }

        private void txtCodigo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Esnumero(e.Text);
        }

        private bool Esnumero(string texto)
        {
            return texto.All(char.IsDigit);
        }

        private void CrearSubcategorias()
        {
            try
            {
                string Codigo = txtCodigo.Text;
                string Nombre = txtNombre.Text;
                string Categoria = cbCategorias.SelectedValue.ToString();

                if (string.IsNullOrEmpty(Codigo) || string.IsNullOrEmpty(Nombre) || string.IsNullOrEmpty(Categoria))
                {
                    MessageBox.Show("Revisa Campos En blanco", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                    using (SqlConnection conexion = new SqlConnection(ConexionSql))
                    {
                        conexion.Open();

                        string InsertSubCategorias = @"insert into ConfSubCategorias (CodigoSubCategoria,NombreSubCategoria,CategoriasID)
                                                        values(@codigo,@nombre,@categoriaid)";
                        SqlCommand cmd = new SqlCommand(InsertSubCategorias, conexion);
                        cmd.Parameters.AddWithValue("@codigo", Codigo);
                        cmd.Parameters.AddWithValue("@nombre", Nombre);
                        cmd.Parameters.AddWithValue("@categoriaid", Convert.ToInt32(Categoria));
                        cmd.ExecuteNonQuery();
                        MuestraSubCategorias();
                        BloquearCampos();

                    }
                }


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
                string Codigo = txtCodigo.Text;
                string Nombre = txtNombre.Text;
                string Categoria = cbCategorias.SelectedValue.ToString();
                string Subcategoriaid = lstSubCategorias.SelectedValue.ToString();

                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Existe = @"select count (*) from ConfSubCategorias where CodigoSubCategoria=@codigo and CategoriasID=@id";
                    SqlCommand cmd = new SqlCommand(Existe, conexion);
                    cmd.Parameters.AddWithValue("@codigo", Codigo);
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(Categoria));
                    int cantidad = Convert.ToInt32(cmd.ExecuteScalar());
                    if (cantidad > 0)
                    {
                        MessageBoxResult Pregunta = MessageBox.Show("Codigo Ya Existe Con Esta Categoria , Desea Actualizar Datos", "MENSAJE", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (Pregunta == MessageBoxResult.Yes)
                        {
                            string UpdateDatos = @"update ConfSubCategorias set
                                                    NombreSubCategoria=@nombre,
                                                    CategoriasID=@id,
                                                    CodigoSubCategoria=@codigo
                                                    where SubCategoriaID=@idsubcategoria";
                            SqlCommand cmdupdate = new SqlCommand(UpdateDatos, conexion);
                            cmdupdate.Parameters.AddWithValue("@codigo", Codigo);
                            cmdupdate.Parameters.AddWithValue("@nombre", Nombre);
                            cmdupdate.Parameters.AddWithValue("@id", Categoria);
                            cmdupdate.Parameters.AddWithValue("@idsubcategoria", Convert.ToInt32(Subcategoriaid));
                            cmdupdate.ExecuteNonQuery();
                            MuestraSubCategorias();
                            BloquearCampos();

                        }
                        else
                        {
                            MessageBox.Show("Proceso Cancelado Por El Usuario", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        CrearSubcategorias();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message);
            }
        }

        //----------------------------------------bototnes---------------------------------------------//


        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {

            DesbloquearCampos();
            txtCodigo.Text = "";
            txtNombre.Text = "";

        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            GuardaActualiza();
        }

        private void lstSubCategorias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            llenarInfoCategorias();
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            EliminarSubcategorias();
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            DesbloquearCampos();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            var TabAbierto = this.Parent as TabItem;
            if (TabAbierto != null)
            {
                var tabcontrol = TabAbierto.Parent as TabControl;
                if (tabcontrol != null)
                {
                    tabcontrol.Items.Remove(TabAbierto);
                }
            }
        }
    }
}
