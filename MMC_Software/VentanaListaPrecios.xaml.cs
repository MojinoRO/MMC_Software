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
    /// Lógica de interacción para VentanaListaPrecios.xaml
    /// </summary>
    public partial class VentanaListaPrecios : UserControl
    {
        public VentanaListaPrecios()
        {
            InitializeComponent();
            BloquearCampos();
            muestraListasprecios();
        }

        private void BloquearCampos()
        {
            txtCodigoLista.IsEnabled = false;
            txtNombreLista.IsEnabled = false;
            btnGuardar.IsEnabled = false;
        }

        private void DesbloquearCampos()
        {
            txtCodigoLista.IsEnabled = true;
            txtNombreLista.IsEnabled = true;
            btnGuardar.IsEnabled = true;
        }

        private void muestraListasprecios()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string ConsultaListas = @"select concat(CodigoListaPrecio,'  ',NombreListaPrecios)As InfoCompleta, ListaPreciosID from ConfListaPrecios";
                    SqlDataAdapter adp = new SqlDataAdapter(ConsultaListas, conexion);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    lstListaPrecios.DisplayMemberPath = "InfoCompleta";
                    lstListaPrecios.SelectedValuePath = "ListaPreciosID";
                    lstListaPrecios.ItemsSource = dt.DefaultView;
                    conexion.Close();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message);
            }
        }

        private void LlenarInfoListas()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    String LlenarInfo = @"select  CodigoListaPrecio, NombreListaPrecios from  ConfListaPrecios where ListaPreciosID=@id";
                    SqlCommand cmd = new SqlCommand(LlenarInfo, conexion);
                    cmd.Parameters.AddWithValue("@id", lstListaPrecios.SelectedValue);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        txtCodigoLista.Text = dt.Rows[0]["CodigoListaPrecio"].ToString();
                        txtNombreLista.Text = dt.Rows[0]["NombreListaPrecios"].ToString();
                        BloquearCampos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message);

            }
        }

        private void CreaActualiza()
        {
            try
            {
                string CodigoLista = txtCodigoLista.Text;
                string NombreLista = txtNombreLista.Text;
                int ID = Convert.ToInt32(lstListaPrecios.SelectedValue);

                if (string.IsNullOrEmpty(CodigoLista) || string.IsNullOrEmpty(NombreLista))
                {
                    MessageBox.Show("Revisa Campos En Blanco", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {

                    if (lstListaPrecios.SelectedValue != null)
                    {

                        conexion.Open();
                        string ConsultaCheck = @"select COUNT(*) from ConfListaPrecios where CodigoListaPrecio=@codigo and ListaPreciosID <>@id";
                        SqlCommand cmd = new SqlCommand(ConsultaCheck, conexion);
                        cmd.Parameters.AddWithValue("@codigo", CodigoLista);
                        cmd.Parameters.AddWithValue("@id", ID);
                        int Existe = Convert.ToInt32(cmd.ExecuteScalar());

                        if (Existe > 0)
                        {
                            MessageBox.Show("Ya existe una lista con este mismo codigo", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        string Updatelista = @"update ConfListaPrecios set 
                                            CodigoListaPrecio=@codigo,
                                            NombreListaPrecios=@nombre
                                            where ListaPreciosID=@id";
                        SqlCommand cmdupdate = new SqlCommand(Updatelista, conexion);
                        cmdupdate.Parameters.AddWithValue("@codigo", CodigoLista);
                        cmdupdate.Parameters.AddWithValue("@nombre", NombreLista);
                        cmdupdate.Parameters.AddWithValue("@id", ID);
                        cmdupdate.ExecuteNonQuery();
                        MessageBox.Show("Lista Actualiada Correctamente", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    else
                    {
                        conexion.Open();
                        string ConsultaCheck = @"select COUNT(*) from ConfListaPrecios where CodigoListaPrecio=@codigo";
                        SqlCommand cmd = new SqlCommand(ConsultaCheck, conexion);
                        cmd.Parameters.AddWithValue("@codigo", CodigoLista);
                        int Existe = Convert.ToInt32(cmd.ExecuteScalar());
                        if (Existe > 0)
                        {
                            MessageBox.Show("Ya existe una lista con este mismo codigo", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        string InsertListas = @"insert into ConfListaPrecios 
                                            (CodigoListaPrecio,NombreListaPrecios) VALUES
                                            (@codigo,@nombre)";
                        SqlCommand cmdInsert = new SqlCommand(InsertListas, conexion);
                        cmdInsert.Parameters.AddWithValue("@codigo", CodigoLista);
                        cmdInsert.Parameters.AddWithValue("@nombre", NombreLista);
                        cmdInsert.ExecuteNonQuery();
                    }

                    muestraListasprecios();
                    BloquearCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message);
            }
        }

        private void EliminarListas()
        {
            try
            {
                string nombre = txtNombreLista.Text;

                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    if (lstListaPrecios.SelectedValue == null)
                    {
                        MessageBox.Show("Debe seleccionar Lista a eliminar", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else
                    {
                        MessageBoxResult Pregunta = MessageBox.Show("Estas Seguro de Eliminar " + nombre, "MENSAJE", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (Pregunta == MessageBoxResult.Yes)
                        {
                            conexion.Open();
                            string deletelista = "DELETE FROM ConfListaPrecios WHERE ListaPreciosID=@id";
                            SqlCommand cmd = new SqlCommand(deletelista, conexion);
                            cmd.Parameters.AddWithValue("@id", lstListaPrecios.SelectedValue);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    muestraListasprecios();
                    BloquearCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message);
            }
        }


        private void lstListaPrecios_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LlenarInfoListas();
        }

        private void txtCodigoLista_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Esnumero(e.Text);
        }

        private bool Esnumero(string Texto)
        {
            return Texto.All(char.IsDigit);
        }

        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {
            DesbloquearCampos();
            txtCodigoLista.Text = "";
            txtNombreLista.Text = "";
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            DesbloquearCampos();
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            EliminarListas();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            CreaActualiza();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
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
