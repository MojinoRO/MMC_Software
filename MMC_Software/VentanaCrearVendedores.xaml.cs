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
    /// Lógica de interacción para VentanaCrearVendedores.xaml
    /// </summary>
    public partial class VentanaCrearVendedores : UserControl
    {
        public VentanaCrearVendedores()
        {
            InitializeComponent();
            MuestraEstados();
            MuestraVendedores();
        }

        private void BloqueaCampos()
        {
            txtCedula.IsEnabled = false;
            txtCodigo.IsEnabled = false;
            txtNombre.IsEnabled = false;
            cmbEstado.IsEnabled = false;
        }

        private void DesbloqueaCampos()
        {
            txtCedula.IsEnabled = true;
            txtCodigo.IsEnabled = true;
            txtNombre.IsEnabled = true;
            cmbEstado.IsEnabled = true;
            btnGuardar.IsEnabled = true;
        }

        private void MuestraVendedores()
        {
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

            using (SqlConnection conexion = new SqlConnection(ConexionSql))
            {
                conexion.Open();
                string muestraVendedores = @"select CONCAT(CodigoVendedor,'  ',NombreVendedor)AS INFOCOMPLETA, VendedorID from ConfVendedores";
                SqlDataAdapter adp = new SqlDataAdapter(muestraVendedores, conexion);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                lstVendedores.DisplayMemberPath = "INFOCOMPLETA";
                lstVendedores.SelectedValuePath = "VendedorID";
                lstVendedores.ItemsSource = dt.DefaultView;
            }
        }

        private void MuestraEstados()
        {
            var tipo = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(0,"Activo"),
                new KeyValuePair<int,string>(1,"Inactivo")
            };
            cmbEstado.ItemsSource = tipo;
            cmbEstado.DisplayMemberPath = "Value";
            cmbEstado.SelectedValuePath = "Key";
        }

        private void CrearVendedores()
        {
            try
            {
                string CodigoVendedor = txtCodigo.Text;
                string NombreVendedor = txtNombre.Text;
                string CedulaVendedor = txtCedula.Text;
                int Estado = Convert.ToInt32(cmbEstado.SelectedValue);
                if (string.IsNullOrEmpty(CodigoVendedor) ||
                   string.IsNullOrEmpty(NombreVendedor) ||
                   string.IsNullOrEmpty(CedulaVendedor))
                {
                    MessageBox.Show("Todos los campos son obligatorios para crear el vendedor", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                    using (SqlConnection conexion = new SqlConnection(ConexionSql))
                    {
                        conexion.Open();
                        string ConsultaExiste = @"select COUNT(*) from ConfVendedores where CodigoVendedor=@codigo";
                        SqlCommand cmdConsulta = new SqlCommand(ConsultaExiste, conexion);
                        cmdConsulta.Parameters.AddWithValue("@codigo", CodigoVendedor);
                        int cantidad = Convert.ToInt32(cmdConsulta.ExecuteScalar());
                        if (cantidad > 0)
                        {
                            MessageBox.Show("El codigo del vendedor ya existe, cambie el codigo y reintente nuevamente", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        else
                        {
                            string InserVendedores = @"insert into ConfVendedores(CodigoVendedor,NombreVendedor,CedulaVendedor,Estado)
                                                  values(@codigo,@nombre,@cedula,@estado)";
                            SqlCommand cmd = new SqlCommand(InserVendedores, conexion);
                            cmd.Parameters.AddWithValue("@codigo", CodigoVendedor);
                            cmd.Parameters.AddWithValue("@nombre", NombreVendedor);
                            cmd.Parameters.AddWithValue("@cedula", CedulaVendedor);
                            cmd.Parameters.AddWithValue("@estado", Convert.ToInt32(Estado));
                            cmd.ExecuteNonQuery();
                            conexion.Close();
                            MuestraVendedores();
                            BloqueaCampos();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.Message);
            }
        }

        private void montainfoVendedor()
        {
            if (lstVendedores.SelectedValue != null)
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string ConsultaVendedores = @"select CodigoVendedor,NombreVendedor,CedulaVendedor,Estado from ConfVendedores 
                                            where VendedorID=@id";
                    SqlCommand cmd = new SqlCommand(ConsultaVendedores, conexion);
                    cmd.Parameters.AddWithValue("id", lstVendedores.SelectedValue);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        txtCodigo.Text = dt.Rows[0]["CodigoVendedor"].ToString();
                        txtNombre.Text = dt.Rows[0]["NombreVendedor"].ToString();
                        txtCedula.Text = dt.Rows[0]["CedulaVendedor"].ToString();
                        int Estado = Convert.ToInt32(dt.Rows[0]["Estado"]);
                        cmbEstado.SelectedValue = Estado;
                    }
                }
            }

        }

        private void GuardaActualiza()
        {
            try
            {
                string CodigoVendedor = txtCodigo.Text;
                string NombreVendedor = txtNombre.Text;
                string CedulaVendedor = txtCedula.Text;
                int Estado = Convert.ToInt32(cmbEstado.SelectedValue);

                if (string.IsNullOrEmpty(CodigoVendedor) ||
                    string.IsNullOrEmpty(CedulaVendedor) ||
                    string.IsNullOrEmpty(NombreVendedor))
                {
                    MessageBox.Show("Revisa Los Datos Hay Campos En Blanco", "MESANJE", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string ConsultaExiste = @"select COUNT(*) from ConfVendedores where CodigoVendedor=@codigo";
                    SqlCommand cmd = new SqlCommand(ConsultaExiste, conexion);
                    cmd.Parameters.AddWithValue("@codigo", CodigoVendedor);
                    int cantidad = Convert.ToInt32(cmd.ExecuteScalar());
                    if (cantidad > 0)
                    {
                        string UpdateDatos = @"update ConfVendedores set 
                                                CodigoVendedor=@codigonew,
                                                NombreVendedor=@nombrenew,
                                                CedulaVendedor=@cedulanew,
                                                Estado=@estadonew
                                                where VendedorID=@id";
                        SqlCommand cmdupdate = new SqlCommand(UpdateDatos, conexion);
                        cmdupdate.Parameters.AddWithValue("@codigonew", CodigoVendedor);
                        cmdupdate.Parameters.AddWithValue("@nombrenew", NombreVendedor);
                        cmdupdate.Parameters.AddWithValue("@cedulanew", CedulaVendedor);
                        cmdupdate.Parameters.AddWithValue("@estadonew", Estado);
                        cmdupdate.Parameters.AddWithValue("@id", lstVendedores.SelectedValue);
                        cmdupdate.ExecuteNonQuery();
                    }
                    else
                    {
                        CrearVendedores();
                    }

                    MuestraVendedores();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.Message);
            }
        }

        //-----------------------------------------------BOTONES---------------------------------------//
        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {
            DesbloqueaCampos();
            txtCedula.Text = "";
            txtCodigo.Text = "";
            txtNombre.Text = "";
            btnGuardar.IsEnabled = true;
        }

        private void txtNombre_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox Texto = sender as TextBox;
            if (Texto != null)
            {
                int Posicion = Texto.CaretIndex;
                string TextoOriginal = Texto.Text;
                string Mayuscula = TextoOriginal.ToUpper();
                if (TextoOriginal != Mayuscula)
                {
                    Texto.Text = Mayuscula;
                    Texto.CaretIndex = Posicion;
                }
            }

        }
        private void ElimianarVendedores()
        {
            if (lstVendedores.SelectedValue != null)
            {
                MessageBoxResult Pregunta = MessageBox.Show("Estas Seguro De Eliminar Vendedor?", "MENSAJE", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (Pregunta == MessageBoxResult.Yes)
                {
                    string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                    using (SqlConnection conexion = new SqlConnection(ConexionSql))
                    {
                        conexion.Open();
                        string DeleteVendedor = "delete from ConfVendedores where VendedorID=@id";
                        SqlCommand cmd = new SqlCommand(DeleteVendedor, conexion);
                        cmd.Parameters.AddWithValue("@id", lstVendedores.SelectedValue);
                        cmd.ExecuteNonQuery();
                        MuestraVendedores();
                    }
                }
            }
            else
            {
                MessageBox.Show("Debe Seleccionar Vendedor a Eliminar", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void txtCedula_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Numeros(e.Text);
        }

        private bool Numeros(string Texto)
        {
            return Texto.All(char.IsDigit);
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            GuardaActualiza();
        }

        private void lstVendedores_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            montainfoVendedor();
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            ElimianarVendedores();
            BloqueaCampos();
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (lstVendedores.SelectedValue != null)
            {
                DesbloqueaCampos();
            }
            else
            {
                MessageBox.Show("Debe Seleccionar Vendedor A Modificar", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnOpciones_Click(object sender, RoutedEventArgs e)
        {
            int VendedorID = Convert.ToInt32(lstVendedores.SelectedValue);
            if (VendedorID == 0)
            {
                MessageBox.Show("Debe Seleccionar Vendedor Para Parametrizar", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            else
            {
                VentanaOpcionesPos ventana = new VentanaOpcionesPos(VendedorID);
                ventana.ShowDialog();
            }
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
