using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace MMC_Software
{
    public partial class DatosEmpresaControl : UserControl
    {

        SqlConnection miConexionSql;
        public DatosEmpresaControl()
        {
            InitializeComponent();

            MuestraDatosEmpresa();

        }


        private void MuestraDatosEmpresa()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                miConexionSql = new SqlConnection(ConexionSql);

                miConexionSql.Open();

                String ConsultaEmpresa = "select EmpresaNit, EmpresaDv, EmpresaNombre,EmpresaEstablecimiento" +
                   ",\r\nEmpresaRepresentanteLegal,EmpresaDireccion,EmpresaTelefono,CorreoElectronico from confEmpresa ";

                using (SqlCommand Cmd = new SqlCommand(ConsultaEmpresa, miConexionSql))
                {
                    using (SqlDataReader reader = Cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TxtEmpresaNit.Text = reader["EmpresaNit"].ToString();
                            TxtEmpresaDv.Text = reader["EmpresaDv"].ToString();
                            TxtNombreEmpresa.Text = reader["EmpresaNombre"].ToString();
                            TxtEstablecimiento.Text = reader["EmpresaEstablecimiento"].ToString();
                            TxtRepresentanteLegal.Text = reader["EmpresaRepresentanteLegal"].ToString();
                            TxtDireccion.Text = reader["EmpresaDireccion"].ToString();
                            TxtTelefono.Text = reader["EmpresaTelefono"].ToString();
                            TxtCorreoElectronico.Text = reader["CorreoElectronico"].ToString();
                        }
                    }
                }

                miConexionSql.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private void ActualizarEmpresa()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection miConexionSql = new SqlConnection(ConexionSql))
                {
                    string ActualizarEmpresa = @"
                UPDATE ConfEmpresa 
                SET 
                    EmpresaNit = @nit,
                    EmpresaDv = @dv,
                    EmpresaNombre = @Nombre,
                    EmpresaEstablecimiento = @Establecimiento,
                    EmpresaRepresentanteLegal = @representantelegal,
                    EmpresaDireccion = @direccion,
                    EmpresaTelefono = @telefono,
                    CorreoElectronico = @Correo";

                    using (SqlCommand cmd = new SqlCommand(ActualizarEmpresa, miConexionSql))
                    {
                        cmd.Parameters.AddWithValue("@nit", TxtEmpresaNit.Text);
                        cmd.Parameters.AddWithValue("@dv", TxtEmpresaDv.Text);
                        cmd.Parameters.AddWithValue("@Nombre", TxtNombreEmpresa.Text);
                        cmd.Parameters.AddWithValue("@Establecimiento", TxtEstablecimiento.Text);
                        cmd.Parameters.AddWithValue("@representantelegal", TxtRepresentanteLegal.Text);
                        cmd.Parameters.AddWithValue("@direccion", TxtDireccion.Text);
                        cmd.Parameters.AddWithValue("@telefono", TxtTelefono.Text);
                        cmd.Parameters.AddWithValue("@Correo", TxtCorreoElectronico.Text);

                        miConexionSql.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                //MessageBox.Show("Datos actualizados correctamente.", "Confirmación", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar los datos:\n\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ActualizarEmpresa();

            BaseFormulario.IsEnabled = false;


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BaseFormulario.IsEnabled = true;
        }



        private void ButtonSalir_Click(object sender, RoutedEventArgs e)
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
