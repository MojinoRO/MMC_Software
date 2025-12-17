using MMC_v1;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para SeleccionEmpresa.xaml
    /// </summary>
    public partial class SeleccionEmpresa : Window
    {


        public SeleccionEmpresa()
        {
            InitializeComponent();
            muestraEmpresas();
        }

        public class Empresas
        {
            public string NombreBaseDatos { get; set; }
            public string NombreEmpresa { get; set; }
        }
        private void muestraEmpresas()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion("master");

                var Empresas = new List<Empresas>();

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();

                    string Consulta = "SELECT name FROM sys.databases WHERE name LIKE '%mmc%' AND state = 0 Order by name asc";

                    using (SqlCommand cmd = new SqlCommand(Consulta, conexion))
                    using (SqlDataReader adr = cmd.ExecuteReader())
                    {
                        while (adr.Read())
                        {
                            string BaseDatos = adr.GetString(0);

                            try
                            {
                                // Conexión a la base de datos de la empresa
                                string ConexionEmpresa = ConfiguracionConexion.ObtenerCadenaConexion(BaseDatos);

                                using (SqlConnection conexionEmpresa = new SqlConnection(ConexionEmpresa))
                                {
                                    conexionEmpresa.Open();

                                    string ConsultaEmpresa = "SELECT TOP 1 EmpresaNombre FROM ConfEmpresa";
                                    using (SqlCommand cmdEmpresa = new SqlCommand(ConsultaEmpresa, conexionEmpresa))
                                    {
                                        object resultado = cmdEmpresa.ExecuteScalar();
                                        if (resultado != null)
                                        {
                                            Empresas.Add(new Empresas
                                            {
                                                NombreBaseDatos = BaseDatos,
                                                NombreEmpresa = resultado.ToString()
                                            });
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error al consultar {BaseDatos}: {ex.Message}");
                            }
                        }
                    }
                }

                //lleno  el ComboBox una sola vez
                cmbEmpresas.ItemsSource = Empresas;
                cmbEmpresas.DisplayMemberPath = "NombreEmpresa";
                cmbEmpresas.SelectedValuePath = "NombreBaseDatos";
                cmbEmpresas.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cmbEmpresas.SelectedValue != null)
            {
                ConexionEmpresaActual.BaseDeDatosSeleccionada = cmbEmpresas.SelectedValue.ToString();
                MainWindow ventana = new MainWindow();
                ventana.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una empresa.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
