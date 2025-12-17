using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaArticulos.xaml
    /// </summary>
    public partial class VentanaArticulos : UserControl
    {
        public VentanaArticulos()
        {
            InitializeComponent();
        }


        private void MuestraArticulos()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Consulta = @"select top(200) AR.ArticulosID,AR.CodigoArticulo,AR.NombreArticulo,AR.CostoArticuloMasIva,AR.ArticulosVenta,
                                        AR.CostoArticuloSinIva,AR.ArticulosIncremento,AR.ArticulosMargen,CAT.NombreCategoria,SCAT.NombreSubCategoria,
                                        AR.ArticulosReferencias,MAR.NombreMarca,AR.ArticulosBarras
                                        from InveArticulos AR, ConfCategoriasInve CAT, ConfSubCategorias SCAT,ConfMarcas MAR
                                        WHERE AR.CategoriasID=CAT.CategoriasID AND AR.SubCategoriaID=SCAT.SubCategoriaID
                                        AND AR.MarcasID=MAR.MarcasID ORDER BY AR.ArticulosID ASC";
                    SqlDataAdapter adapter = new SqlDataAdapter(Consulta, conexion);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgArticulos.ItemsSource = dt.DefaultView;
                    dgArticulos.SelectedValuePath = "ArticulosID";
                    conexion.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MuestraArticulos();
        }

        private void dgArticulos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int ArticuloID = Convert.ToInt32(dgArticulos.SelectedValue);
            VentanaCrearArticulos ventana = new VentanaCrearArticulos(ArticuloID);
            ventana.ShowDialog();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            // Buscar el TabItem que contiene este UserControl
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

        private void btnCrearArticulo_Click(object sender, RoutedEventArgs e)
        {
            VentanaCrearArticulos ventana = new VentanaCrearArticulos();
            ventana.ShowDialog();
        }

        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txtBuscado = txtBuscar.Text.Trim();
            FiltrarArticulo(txtBuscado);
        }

        private void FiltrarArticulo(string filtro)
        {
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

            using (SqlConnection conexion = new SqlConnection(ConexionSql))
            {
                conexion.Open();
                string consulta = @"SELECT AR.ArticulosID,
                   AR.CodigoArticulo,
                   AR.NombreArticulo,
                   AR.CostoArticuloMasIva,
                   AR.ArticulosVenta,
                   AR.CostoArticuloSinIva,
                   AR.ArticulosIncremento,
                   AR.ArticulosMargen,
                   CAT.NombreCategoria,
                   SCAT.NombreSubCategoria,
                   AR.ArticulosReferencias,
                   MAR.NombreMarca,
                   AR.ArticulosBarras
                   FROM InveArticulos AR
                  JOIN ConfCategoriasInve CAT ON AR.CategoriasID = CAT.CategoriasID
                  JOIN ConfSubCategorias SCAT ON AR.SubCategoriaID = SCAT.SubCategoriaID
                  JOIN ConfMarcas MAR ON AR.MarcasID = MAR.MarcasID
                  WHERE (AR.NombreArticulo LIKE @filtro OR AR.CodigoArticulo LIKE @filtro)
                 ORDER BY AR.CodigoArticulo ASC";
                SqlCommand cmd = new SqlCommand(consulta, conexion);
                cmd.Parameters.AddWithValue("filtro", "%" + filtro + "%");
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                dgArticulos.ItemsSource = dt.DefaultView;
                dgArticulos.SelectedValuePath = "ArticulosID";
            }
        }
    }
}
