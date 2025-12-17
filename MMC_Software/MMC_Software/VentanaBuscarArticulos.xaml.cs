using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaBuscarArticulos.xaml
    /// </summary>
    public partial class VentanaBuscarArticulos : Window
    {

        public VentanaBuscarArticulos()
        {
            InitializeComponent();
            MontarArticulos();
            txtBuscar.Focus();
        }

        public List<articulos> ListaArticulos = new List<articulos>();

        public articulos Articulosseleccionado { get; set; }

        public class articulos
        {
            public int ArticuloID { get; set; }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public string Marca { get; set; }
            public decimal TarifaIVA { get; set; }
            public decimal ValorVenta { get; set; }
            public decimal CostoSinIva { get; set; }
            public decimal Margen { get; set; }
            public decimal Incremento { get; set; }
            public decimal Saldo { get; set; }
        }

        private void MontarArticulos()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Consulta = @"SELECT TOP 100  AR.ArticulosID, AR.CodigoArticulo,AR.NombreArticulo,MAR.NombreMarca,
                                        CAT.TarifaImpuesto,AR.ArticulosVenta, AR.CostoArticuloSinIva,AR.ArticulosMargen , AR.ArticulosIncremento,
                                        ISNULL(SUM(SAL.SaldoActual),0) AS Saldo
                                        FROM InveArticulos AR
                                        LEFT JOIN ConfMarcas MAR ON AR.MarcasID=MAR.MarcasID
                                        LEFT JOIN ConfCategoriasInve CAT ON AR.CategoriasID=CAT.CategoriasID
                                        LEFT JOIN InveSaldoInvetarios SAL ON AR.ArticulosID=SAL.ArticulosID
                                        GROUP BY 
	                                        AR.ArticulosID,
	                                        AR.CodigoArticulo,
	                                        AR.NombreArticulo,
	                                        MAR.NombreMarca,
	                                        CAT.TarifaImpuesto,
	                                        AR.ArticulosVenta,
	                                        AR.CostoArticuloSinIva,
	                                        AR.ArticulosMargen,
	                                        AR.ArticulosIncremento";
                    using (SqlCommand command = new SqlCommand(Consulta, conexion))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        ListaArticulos.Clear();
                        while (reader.Read())
                        {
                            articulos Art = new articulos
                            {
                                ArticuloID = Convert.ToInt32(reader["ArticulosID"]),
                                Codigo = reader["CodigoArticulo"].ToString(),
                                Nombre = reader["NombreArticulo"].ToString(),
                                Marca = reader["NombreMarca"].ToString(),
                                TarifaIVA = Convert.ToDecimal(reader["TarifaImpuesto"]),
                                ValorVenta = Convert.ToDecimal(reader["ArticulosVenta"]),
                                CostoSinIva = Convert.ToDecimal(reader["CostoArticuloSinIva"]),
                                Margen = Convert.ToDecimal(reader["ArticulosMargen"]),
                                Incremento = Convert.ToDecimal(reader["ArticulosIncremento"]),
                                Saldo = Convert.ToDecimal(reader["Saldo"])
                            };
                            ListaArticulos.Add(Art);
                            dgArticulos.ItemsSource = ListaArticulos;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);

            }

        }


        private void Buscador(string filtro)
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();

                    string ConsultaBuscar = @"SELECT TOP 100  AR.ArticulosID, AR.CodigoArticulo,AR.NombreArticulo,MAR.NombreMarca,
                                            CAT.TarifaImpuesto,AR.ArticulosVenta, AR.CostoArticuloSinIva,AR.ArticulosMargen , AR.ArticulosIncremento,
                                            ISNULL(SUM(SAL.SaldoActual),0) AS Saldo
                                            FROM InveArticulos AR
                                            LEFT JOIN ConfMarcas MAR ON AR.MarcasID=MAR.MarcasID
                                            LEFT JOIN ConfCategoriasInve CAT ON AR.CategoriasID=CAT.CategoriasID
                                            LEFT JOIN InveSaldoInvetarios SAL ON AR.ArticulosID=SAL.ArticulosID 
                                            WHERE (AR.CodigoArticulo likE @filtro or AR.NombreArticulo like @filtro)
                                            GROUP BY 
	                                            AR.ArticulosID,
	                                            AR.CodigoArticulo,
	                                            AR.NombreArticulo,
	                                            MAR.NombreMarca,
	                                            CAT.TarifaImpuesto,
	                                            AR.ArticulosVenta,
	                                            AR.CostoArticuloSinIva,
	                                            AR.ArticulosMargen,
	                                            AR.ArticulosIncremento";
                    using (SqlCommand cmd = new SqlCommand(ConsultaBuscar, conexion))
                    {
                        cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");
                        SqlDataReader reader = cmd.ExecuteReader();
                        ListaArticulos.Clear();
                        while (reader.Read())
                        {
                            articulos art = new articulos()
                            {
                                ArticuloID = Convert.ToInt32(reader["ArticulosID"]),
                                Codigo = reader["CodigoArticulo"].ToString(),
                                Nombre = reader["NombreArticulo"].ToString(),
                                Marca = reader["NombreMarca"].ToString(),
                                TarifaIVA = Convert.ToDecimal(reader["TarifaImpuesto"]),
                                ValorVenta = Convert.ToDecimal(reader["ArticulosVenta"]),
                                CostoSinIva = Convert.ToDecimal(reader["CostoArticuloSinIva"]),
                                Margen = Convert.ToDecimal(reader["ArticulosMargen"]),
                                Incremento = Convert.ToDecimal(reader["ArticulosIncremento"]),
                                Saldo = Convert.ToDecimal(reader["Saldo"])
                            };
                            ListaArticulos.Add(art);
                        }
                        // la tabla se llena por fuera del bucle while
                        dgArticulos.ItemsSource = null;
                        dgArticulos.ItemsSource = ListaArticulos;

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void dgArticulos_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && dgArticulos.SelectedItem!=null)
            {
                ConfirmarSeleccion();
                e.Handled = true;
            }
        }

        private void dgArticulos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ConfirmarSeleccion();
        }

        private void ConfirmarSeleccion()
        {
            Articulosseleccionado = (articulos)dgArticulos.SelectedItem;
            this.DialogResult = true;
            this.Close();
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                string text = txtBuscar.Text;
                Buscador(text);
                if (dgArticulos.Items.Count > 0)
                {
                    dgArticulos.SelectedIndex = 0;
                    dgArticulos.Focus();
                }
            }
        }
    }
}
