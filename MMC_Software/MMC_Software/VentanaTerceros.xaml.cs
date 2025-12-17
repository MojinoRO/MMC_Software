using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaTerceros.xaml
    /// </summary>
    public partial class VentanaTerceros : UserControl
    {
        public VentanaTerceros()
        {
            InitializeComponent();
        }


        private void cargarTerceros()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string ConsultaClientes = @"select TER.TercerosID,TER.TercerosTipoDocumento,TER.TercerosIdentificacion,TER.TercerosNombreCompleto,
                                                TER.TerceroRazonSocial,DEP.DepartamentoNombre,CIU.CiudadNombre,TER.TercerosDireccion,TER.TercerosEmail,
                                                TER.TercerosTelefono,TER.ListaPreciosID
                                                from InveTerceros TER,ConfDepartamento DEP,ConfCiudad CIU
                                                WHERE TER.DepartamentoID=DEP.DepartamentoID AND CIU.CiudadID=TER.CiudadID ORDER BY TER.TercerosID ASC";
                    SqlDataAdapter adapter = new SqlDataAdapter(ConsultaClientes, conexion);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dgTerceros.ItemsSource = dataTable.DefaultView;
                    dgTerceros.SelectedValuePath = "TercerosID";

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            cargarTerceros();
        }

        private void ButtonCerrar_Click(object sender, RoutedEventArgs e)
        {
            var TabItems = this.Parent as TabItem;
            if (TabItems != null)
            {
                var tabcontrol = TabItems.Parent as TabControl;
                if (tabcontrol != null)
                {
                    tabcontrol.Items.Remove(TabItems);
                }
            }
        }

        private void dgTerceros_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgTerceros.SelectedValue != null)
            {
                int TerceroID = Convert.ToInt32(dgTerceros.SelectedValue);
                VentanaCrearTerceros ventana = new VentanaCrearTerceros(TerceroID);
                ventana.ShowDialog();
            }
        }

        private void btnCrearTercero_Click(object sender, RoutedEventArgs e)
        {
            VentanaCrearTerceros ventana = new VentanaCrearTerceros();
            ventana.ShowDialog();
        }
    }
}
