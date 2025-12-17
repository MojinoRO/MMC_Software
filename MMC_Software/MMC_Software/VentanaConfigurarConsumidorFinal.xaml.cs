using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaConfigurarConsumidorFinal.xaml
    /// </summary>
    public partial class VentanaConfigurarConsumidorFinal : UserControl
    {
        public VentanaConfigurarConsumidorFinal()
        {
            InitializeComponent();
        }
        public static class NitConsumidorfinal
        {
            public static int IDconsumidor { get; set; }
        }
        private void txtNitConsumidor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    string Nit = txtNitConsumidor.Text;
                    string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                    using (SqlConnection conexion = new SqlConnection(ConexionSql))
                    {
                        conexion.Open();
                        string Consulta = "select TercerosIdentificacion,TerceroRazonSocial , TercerosID from InveTerceros where TercerosIdentificacion=@filtro";
                        using (SqlCommand cmd = new SqlCommand(Consulta, conexion))
                        {
                            cmd.Parameters.AddWithValue("@filtro", Nit);
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                txtNitConsumidor.Text = dt.Rows[0]["TercerosIdentificacion"].ToString();
                                txtNombreConsumidor.Text = dt.Rows[0]["TerceroRazonSocial"].ToString();
                                NitConsumidorfinal.IDconsumidor = Convert.ToInt32(dt.Rows[0]["TercerosID"]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Insert = "insert into InveNitConsumidor(TercerosID) VALUES(@terceroid)";
                    using (SqlCommand cmdInsert = new SqlCommand(Insert, conexion))
                    {
                        cmdInsert.Parameters.AddWithValue("@terceroid", NitConsumidorfinal.IDconsumidor);
                        cmdInsert.ExecuteNonQuery();
                        conexion.Close();
                        txtNitConsumidor.IsEnabled = false;
                        txtNombreConsumidor.IsEnabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            var FormularioAbierto = this.Parent as TabItem;
            if (FormularioAbierto != null)
            {
                var Tabcontrol = FormularioAbierto.Parent as TabControl;
                if (Tabcontrol != null)
                {
                    Tabcontrol.Items.Remove(FormularioAbierto);
                }
            }
        }
    }
}
