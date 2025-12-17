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
    /// <summary>
    /// Lógica de interacción para VentanaCrearDocumentos.xaml
    /// </summary>
    public partial class VentanaCrearDocumentos : UserControl
    {
        public VentanaCrearDocumentos()
        {
            InitializeComponent();

            muestraDocumentos();

            MuestraTipoDocumentos();

            MuestraSubcentros();
        }

        private void muestraDocumentos()
        {
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

            using (SqlConnection conexion = new SqlConnection(ConexionSql))
            {
                conexion.Open();

                string ConsultamuestraAlmacen = @"select CONCAT(CodigoDocumento,'    ',NombreDocumento) AS INFOCOMPLETA, DocumentoID from ConfDocumentos";
                SqlDataAdapter adp = new SqlDataAdapter(ConsultamuestraAlmacen, conexion);
                DataTable dataTable = new DataTable();
                adp.Fill(dataTable);
                lstDocumentos.DisplayMemberPath = "INFOCOMPLETA";
                lstDocumentos.SelectedValuePath = "DocumentoID";
                lstDocumentos.ItemsSource = dataTable.DefaultView;
            }
        }

        private void MuestraTipoDocumentos()
        {
            var tipo = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("1","Entradas"),
                new KeyValuePair<string, string>("2","Salidas"),
                new KeyValuePair<string, string>("3","CarteraCliente"),
                new KeyValuePair<string, string>("4","CarteraProveedor"),
                new KeyValuePair<string, string>("5","Comprobantes"),
                new KeyValuePair<string, string>("6","Devolucion Entradas"),
                new KeyValuePair<string, string>("7","Devolucion Salidas")
            };

            cmbTipoDocumento.ItemsSource = tipo;
            cmbTipoDocumento.DisplayMemberPath = "Value";
            cmbTipoDocumento.SelectedValuePath = "Key";

        }

        private void MuestraSubcentros()
        {
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

            using (SqlConnection conexion = new SqlConnection(ConexionSql))
            {
                conexion.Open();

                string consultaSubcentros = @"select CONCAT(CC.CodigoCosto,'-',SCC.CodigoSubcentro,'    ',SCC.NombreSubCentroCosto) AS INFOCOMPLETA , SCC.SubCentroCostoID,CC.IdCentroCosto
                                            from ConfSubCentroCosto SCC, ConfCentroCostos CC
                                            WHERE CC.IdCentroCosto=SCC.IdCentroCosto";
                SqlDataAdapter adp = new SqlDataAdapter(consultaSubcentros, conexion);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                cmbSubCentroCosto.DisplayMemberPath = "INFOCOMPLETA";
                cmbSubCentroCosto.SelectedValuePath = "SubCentroCostoID";
                cmbSubCentroCosto.ItemsSource = dt.DefaultView;


            }
        }

        private void bloqueacampos()
        {
            txtCodigoDocumento.IsEnabled = false;
            txtConsecutivo.IsEnabled = false;
            txtFormatoImpresion.IsEnabled = false;
            txtNombreDocumento.IsEnabled = false;
            cmbTipoDocumento.IsEnabled = false;
            cmbSubCentroCosto.IsEnabled = false;

        }

        private void desbloquearCampos()
        {
            txtCodigoDocumento.IsEnabled = true;
            txtConsecutivo.IsEnabled = true;
            txtFormatoImpresion.IsEnabled = true;
            txtNombreDocumento.IsEnabled = true;
            cmbTipoDocumento.IsEnabled = true;
            cmbSubCentroCosto.IsEnabled = true;
            btnGuardar.IsEnabled = true;

        }

        private void MontarInfoDocuentos()
        {
            try
            {
                string Iddocumentos = lstDocumentos.SelectedValue.ToString();

                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    string ConsultaMontarInfoDocumentos = @"select NombreDocumento, CodigoDocumento, ConsecutivoActual,FormatoImpresion ,TipoDocumento,SubCentroCostoID
                                                        from ConfDocumentos where DocumentoID=@id";
                    SqlCommand cmd = new SqlCommand(ConsultaMontarInfoDocumentos, conexion);
                    cmd.Parameters.AddWithValue("@id", Iddocumentos);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        txtCodigoDocumento.Text = dt.Rows[0]["CodigoDocumento"].ToString();
                        txtNombreDocumento.Text = dt.Rows[0]["NombreDocumento"].ToString();
                        txtConsecutivo.Text = dt.Rows[0]["ConsecutivoActual"].ToString();
                        txtFormatoImpresion.Text = dt.Rows[0]["FormatoImpresion"].ToString();
                        int Tipodocumento = Convert.ToInt32(dt.Rows[0]["TipoDocumento"]);
                        int Subcentro = Convert.ToInt32(dt.Rows[0]["SubCentroCostoID"]);
                        MuestraTipoDocumentos();
                        cmbTipoDocumento.SelectedValue = Tipodocumento;
                        MuestraSubcentros();
                        cmbSubCentroCosto.SelectedValue = Subcentro;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void CrearDocumentos()
        {
            try
            {
                string NombreDocumento = txtNombreDocumento.Text;
                String CodigoDocumento = txtCodigoDocumento.Text.Trim();
                string FormatoImpresion = txtFormatoImpresion.Text;
                int Consecutivo = Convert.ToInt32(txtConsecutivo.Text);
                int Tipodocumento = Convert.ToInt32(cmbTipoDocumento.SelectedValue);
                int SubcentroCosto = Convert.ToInt32(cmbSubCentroCosto.SelectedValue);

                if (string.IsNullOrEmpty(NombreDocumento) || string.IsNullOrEmpty(CodigoDocumento) || string.IsNullOrEmpty(FormatoImpresion))
                {
                    MessageBox.Show("hay campos en blancos que son obligatorios", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                    using (SqlConnection conexion = new SqlConnection(ConexionSql))
                    {
                        conexion.Open();

                        string ConsultaCodigo = "select COUNT(*) from ConfDocumentos where CodigoDocumento=@codigo";
                        SqlCommand cmdCodigo = new SqlCommand(ConsultaCodigo, conexion);
                        cmdCodigo.Parameters.AddWithValue("codigo", CodigoDocumento);
                        int Cantidad = Convert.ToInt32(cmdCodigo.ExecuteScalar());
                        if (Cantidad < 0)
                        {
                            string InsertDocumentos = @"insert into ConfDocumentos(SubCentroCostoID,TipoDocumento,NombreDocumento,CodigoDocumento,ConsecutivoActual,FormatoImpresion) 
                                                values(@scc,@tipodoc,@nombre,@cBodigo,@consecutivo,@formato)";
                            SqlCommand cmd = new SqlCommand(InsertDocumentos, conexion);
                            cmd.Parameters.AddWithValue("scc", SubcentroCosto);
                            cmd.Parameters.AddWithValue("tipodoc", Tipodocumento);
                            cmd.Parameters.AddWithValue("nombre", NombreDocumento);
                            cmd.Parameters.AddWithValue("codigo", CodigoDocumento);
                            cmd.Parameters.AddWithValue("consecutivo", Consecutivo);
                            cmd.Parameters.AddWithValue("formato", Consecutivo);
                            cmd.ExecuteNonQuery();
                            muestraDocumentos();
                            bloqueacampos();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void GuardaActualiza()
        {
            if (lstDocumentos.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un documento para actualizar.", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int Id = Convert.ToInt32(lstDocumentos.SelectedValue);
                string NombreDocumento = txtNombreDocumento.Text;
                string CodigoDocumento = txtCodigoDocumento.Text.Trim();
                string FormatoImpresion = txtFormatoImpresion.Text;
                int Consecutivo = Convert.ToInt32(txtConsecutivo.Text);
                int Tipodocumento = Convert.ToInt32(cmbTipoDocumento.SelectedValue);
                int SubcentroCosto = Convert.ToInt32(cmbSubCentroCosto.SelectedValue);

                if (string.IsNullOrWhiteSpace(NombreDocumento) ||
                    string.IsNullOrWhiteSpace(CodigoDocumento) ||
                    string.IsNullOrWhiteSpace(FormatoImpresion))
                {
                    MessageBox.Show("Debe completar todos los campos obligatorios.", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();

                    string consultaExiste = "Select COUNT(*) from ConfDocumentos where CodigoDocumento=@codigo";
                    SqlCommand cmdExiste = new SqlCommand(consultaExiste, conexion);
                    cmdExiste.Parameters.AddWithValue("@codigo", CodigoDocumento);
                    int cantidad = Convert.ToInt32(cmdExiste.ExecuteScalar());
                    if (cantidad > 0)
                    {
                        string updateQuery = @"UPDATE ConfDocumentos 
                                   SET NombreDocumento = @nombre,
                                       CodigoDocumento = @codigo,
                                       FormatoImpresion = @formato,
                                       ConsecutivoActual = @consecutivo,
                                       TipoDocumento = @tipo,
                                       SubCentroCostoID = @scc
                                   WHERE DocumentoID = @id";

                        SqlCommand cmd = new SqlCommand(updateQuery, conexion);
                        cmd.Parameters.AddWithValue("@id", Id);
                        cmd.Parameters.AddWithValue("@nombre", NombreDocumento);
                        cmd.Parameters.AddWithValue("@codigo", CodigoDocumento);
                        cmd.Parameters.AddWithValue("@formato", FormatoImpresion);
                        cmd.Parameters.AddWithValue("@consecutivo", Consecutivo);
                        cmd.Parameters.AddWithValue("@tipo", Tipodocumento);
                        cmd.Parameters.AddWithValue("@scc", SubcentroCosto);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Documento actualizado correctamente.", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
                            muestraDocumentos();
                            bloqueacampos();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo actualizar el documento.", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar: " + ex.Message);
            }
        }


        private void EliminarDocumentos()
        {
            int DocumentoID = Convert.ToInt32(lstDocumentos.SelectedValue);
            MontarInfoDocuentos();
            string NombreDocumento = txtNombreDocumento.Text;
            try
            {
                if (lstDocumentos.SelectedValue != null)
                {
                    MessageBoxResult Consulta = MessageBox.Show("Estas Seguro de Eliminar" + " " + NombreDocumento, "MENSAJE",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (Consulta == MessageBoxResult.Yes)
                    {
                        string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                        using (SqlConnection conexion = new SqlConnection(ConexionSql))
                        {
                            conexion.Open();
                            string DeleteDocumentos = @"delete from ConfDocumentos where DocumentoID=@id";
                            SqlCommand cdm = new SqlCommand(DeleteDocumentos, conexion);
                            cdm.Parameters.AddWithValue("id", DocumentoID);
                            cdm.ExecuteNonQuery();
                            muestraDocumentos();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Proceso Cancelado Por El Usuario", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                else
                {
                    MessageBox.Show("Debe seleccionar documento a eliminar", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // -----------------------------------------------botones----------------------------------------------------//


        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {

            desbloquearCampos();
            txtCodigoDocumento.Text = "";
            txtNombreDocumento.Text = "";
            txtFormatoImpresion.Text = "";
            txtConsecutivo.Text = "";


        }

        private void txtNombreDocumento_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox TextoMayus = sender as TextBox;
            if (TextoMayus != null)
            {
                int Careindex = TextoMayus.CaretIndex;
                string OriginalText = TextoMayus.Text;
                string uppertext = OriginalText.ToUpper();
                if (OriginalText != uppertext)
                {
                    TextoMayus.Text = uppertext;
                    TextoMayus.CaretIndex = Careindex;
                }
            }
        }


        private void txtConsecutivo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !EsNumero(e.Text);
        }

        private bool EsNumero(string texto)
        {
            return texto.All(char.IsDigit);
        }

        private void lstDocumentos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MontarInfoDocuentos();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {

            GuardaActualiza();
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (lstDocumentos.SelectedValue != null)
            {
                desbloquearCampos();

            }
            else
            {
                MessageBox.Show("Debe seleccionar documento a modificar", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            EliminarDocumentos();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            var tabAbierto = this.Parent as TabItem;
            if (tabAbierto != null)
            {
                var Item = tabAbierto.Parent as TabControl;
                if (Item != null)
                {
                    Item.Items.Remove(tabAbierto);
                }
            }
        }
    }
}

