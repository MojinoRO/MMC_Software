using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaBuscarCuentas.xaml
    /// </summary>
    public partial class VentanaBuscarCuentas : Window
    {
        public VentanaBuscarCuentas()
        {
            InitializeComponent();
            MuestracuentasPuc();
        }

        private void MuestracuentasPuc()
        {
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

            using (SqlConnection conexion = new SqlConnection(ConexionSql))
            {
                conexion.Open();

                string consultaCuentas = "SELECT  top(100) CuentasPucCodigo As Codigo,CuentasPucNombre AS Nombre,CuentasPucID from ConfCuentasPuc order BY CuentasPucCodigo ASC";
                SqlDataAdapter adp = new SqlDataAdapter(consultaCuentas, conexion);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                dgCuentas.ItemsSource = dt.DefaultView;
                dgCuentas.SelectedValuePath = "CuentasPucID";
            }

        }

        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string Text = txtBuscar.Text;

            Buscador(Text);
        }

        private void Buscador(string filtro)
        {
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

            using (SqlConnection conexion = new SqlConnection(ConexionSql))
            {
                string ConsutlaCuentas = @"select top (100)  CuentasPucCodigo As Codigo,CuentasPucNombre AS Nombre,CuentasPucID 
                                        from ConfCuentasPuc
                                        where CuentasPucNombre like @filtro OR CuentasPucCodigo LIKE @filtro   order BY CuentasPucCodigo ASC";
                SqlCommand cmd = new SqlCommand(ConsutlaCuentas, conexion);
                cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                dgCuentas.ItemsSource = dt.DefaultView;
                dgCuentas.SelectedValuePath = "CuentasPucID";
            }
        }

        public string CuentaSeleccionada { get; set; }
        public string NombreCuentaSeleccionada { set; get; }

        public int Idcuenta { get; set; }

        private void btnSeleccionar_Click(object sender, RoutedEventArgs e)
        {

            if (dgCuentas.SelectedItem != null)
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string ConsultaPermiteMov = @"select  CuentaPermiteMov FROM ConfCuentasPuc WHERE CuentasPucID=@id";
                    SqlCommand cmd = new SqlCommand(ConsultaPermiteMov, conexion);
                    cmd.Parameters.AddWithValue("@id", dgCuentas.SelectedValue);
                    SqlDataAdapter adp1 = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    int resultado = Convert.ToInt32(cmd.ExecuteScalar());
                    if (resultado == 1)
                    {
                        DataRowView fila = (DataRowView)dgCuentas.SelectedItem;

                        if (fila != null)
                        {
                            CuentaSeleccionada = fila.Row["Codigo"].ToString();
                            NombreCuentaSeleccionada = fila.Row["Nombre"].ToString();
                            Idcuenta = Convert.ToInt32(fila.Row["CuentasPucID"]);
                            this.DialogResult = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cuenta No permite Movimiento", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }

            }

            else
            {
                MessageBox.Show("Debe seleccionar alguna cuenta", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
