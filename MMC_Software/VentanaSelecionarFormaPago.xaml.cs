using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaSelecionarFormaPago.xaml
    /// </summary>
    public partial class VentanaSelecionarFormaPago : Window
    {
        private readonly int TipoDocumentoSeleccionado;
        private readonly decimal ValorTotalAPagar;
        private ListadoFormasPago UltimaSeleccion = null;

        public decimal _Entregado;
        public decimal _Cambio;
        public VentanaSelecionarFormaPago(int TipoDocumento, decimal valorTotalAPagar)
        {
            InitializeComponent();
            TipoDocumentoSeleccionado = TipoDocumento;
            MostrarFormasPago();
            ValorTotalAPagar = valorTotalAPagar;
            LlenarTotalAPagar();

        }
        private void LlenarTotalAPagar()
        {
            try
            {
                txtTotal.Text = ValorTotalAPagar.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se Puede facturar Registros en 0 " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }

        // clase donde se llena el dgformapgos 
        public class ListadoFormasPago
        {
            public int ID { get; set; }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public decimal Valor { get; set; }
        }

        ObservableCollection<ListadoFormasPago> FormasPagos = new ObservableCollection<ListadoFormasPago>();

        public  ListadoFormasPago FormaPagoSeleccionada { get; set; }   


        private void MostrarFormasPago()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    string Consulta = "";
                    conexion.Open();
                    if (TipoDocumentoSeleccionado == 1)
                    {
                        Consulta = "select FormaPagoID,CodigoFormaPago,NombreFormaPago from ConfFormasPago where FormaPagoTipo=2";

                    }
                    else if (TipoDocumentoSeleccionado == 2)
                    {
                        Consulta = "select FormaPagoID,CodigoFormaPago,NombreFormaPago from ConfFormasPago where FormaPagoTipo=1";
                    }
                    using (SqlCommand cmd = new SqlCommand(Consulta, conexion))
                    {
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            FormasPagos.Clear();
                            foreach (DataRow row in dt.Rows)
                            {
                                ListadoFormasPago formasPago = new ListadoFormasPago
                                {
                                    ID = Convert.ToInt32(row["FormaPagoID"]),
                                    Codigo = row["CodigoFormaPago"].ToString(),
                                    Nombre = row["NombreFormaPago"].ToString()

                                };

                                FormasPagos.Add(formasPago);
                            }

                            dgFormasPago.ItemsSource = FormasPagos;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }


        private void ConfirmarSeleccion()
        {
            try
            {
                if (dgFormasPago.SelectedItem is ListadoFormasPago seleccionado)
                {
                    // Limpia el valor del anterior seleccionado
                    if (UltimaSeleccion != null && UltimaSeleccion != seleccionado)
                    {
                        UltimaSeleccion.Valor = 0;
                    }

                    // Convertir double a decimal directamente
                    decimal total = Convert.ToDecimal(ValorTotalAPagar);

                    seleccionado.Valor = total;
                    dgFormasPago.Items.Refresh();

                    // Actualiza el último seleccionado
                    UltimaSeleccion = seleccionado;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public int FormaPagoIDFinal;
        private void ButtonAceptar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgFormasPago.SelectedItem is ListadoFormasPago Selecccion)
                {
                    FormaPagoIDFinal = Selecccion.ID;

                    string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                    using (SqlConnection conexion = new SqlConnection(ConexionSql))
                    {
                        conexion.Open();
                        string Consulta = "select ManejaCambio from ConfFormasPago where FormaPagoID=@filtro";
                        using (SqlCommand cmd = new SqlCommand(Consulta, conexion))
                        {
                            cmd.Parameters.AddWithValue("@filtro", FormaPagoIDFinal);
                            int ManejaCambio = Convert.ToInt32(cmd.ExecuteScalar());
                            if (ManejaCambio == 1)
                            {
                                VentanaDevueltasVentas ventana = new VentanaDevueltasVentas(ValorTotalAPagar);
                                bool? resultado = ventana.ShowDialog();
                                if (resultado != null)
                                {
                                    _Entregado = ventana._ValorRecibido;
                                    _Cambio = ventana._ValorDevueltas;
                                }
                            }
                        }
                    }
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Debe seleccionar una forma de pago.", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.ToString(), "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void dgFormasPago_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
                ConfirmarSeleccion();
                e.Handled = true;
            }
        }

        private void dgFormasPago_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ConfirmarSeleccion();
        }
    }
}
