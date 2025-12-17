using MMC_Software;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static MMC_Software.VentanaConfigurarConsumidorFinal;

namespace MMC_v1
{

    public partial class MainWindow : Window
    {
        private SqlConnection miConexionSql;
        private DispatcherTimer timer;
        private int progreso = 0;
        private int _UsuarioID = 0;

        public MainWindow()
        {
            InitializeComponent();

            // Obtener cadena de conexión desde clase de configuración
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
            miConexionSql = new SqlConnection(ConexionSql);

            // Mostrar datos iniciales
            MuestraUsuario();
            MuestraEmpresa();

            // Configurar barra de progreso
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(30);
            timer.Tick += Timer_Tick;


            //LLENAR CONSUMIDOR 
            llenarConsumidor();
        }

        private void Button_Click_Salir(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MuestraUsuario()
        {
            string consultaMuestraUsuario = "SELECT * FROM ConfUsuarios order by NombreUsuario ASC";
            SqlDataAdapter AdaptadorUsuarios = new SqlDataAdapter(consultaMuestraUsuario, miConexionSql);

            using (AdaptadorUsuarios)
            {
                DataTable UsuariosExistentes = new DataTable();
                AdaptadorUsuarios.Fill(UsuariosExistentes);

                ListadoUsuario.DisplayMemberPath = "NombreUsuario";
                ListadoUsuario.SelectedValuePath = "UsuariosID";
                ListadoUsuario.ItemsSource = UsuariosExistentes.DefaultView;
                miConexionSql.Close();
            }
        }

        private void MuestraEmpresa()
        {
            string ConsultaEmpresa = "SELECT EmpresaNombre FROM ConfEmpresa";
            SqlDataAdapter adaptadorMuestraEmpresa = new SqlDataAdapter(ConsultaEmpresa, miConexionSql);

            using (adaptadorMuestraEmpresa)
            {
                DataTable nombreEmpresa = new DataTable();
                adaptadorMuestraEmpresa.Fill(nombreEmpresa);

                if (nombreEmpresa.Rows.Count > 0)
                    TxtEmpresa.Text = nombreEmpresa.Rows[0]["EmpresaNombre"].ToString();
            }
        }

        private void ListadoUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView itemSeleccionado = ListadoUsuario.SelectedItem as DataRowView;

            if (itemSeleccionado != null)
            {
                txtNombreSeleccionado.Text = itemSeleccionado["NombreUsuario"].ToString();
                ListadoUsuario.Visibility = Visibility.Collapsed;
            }
        }

        private void ToggleListaUsuarios(object sender, RoutedEventArgs e)
        {
            ListadoUsuario.Visibility = ListadoUsuario.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void Button_Click_Entrar(object sender, RoutedEventArgs e)
        {
            string usuario = txtNombreSeleccionado.Text.Trim();
            string contraseña = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contraseña))
            {
                MessageBox.Show("Por favor seleccione un usuario e ingrese una contraseña.");
                return;
            }

            try
            {
                miConexionSql.Open();

                string consultaIngreso = "SELECT COUNT(*) FROM ConfUsuarios WHERE NombreUsuario = @usuario AND UsuarioClave = @clave";
                SqlCommand comando = new SqlCommand(consultaIngreso, miConexionSql);
                comando.Parameters.AddWithValue("@usuario", usuario);
                comando.Parameters.AddWithValue("@clave", contraseña);
                int count = (int)comando.ExecuteScalar();

                if (count > 0)
                {
                    // Iniciar carga visual
                    progreso = 0;
                    progressBarLogin.Value = 0;
                    progressBarLogin.Visibility = Visibility.Visible;
                    timer.Start();
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.");
                    txtPassword.SelectAll();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al validar el usuario: " + ex.ToString());
            }
            finally
            {
                miConexionSql.Close();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            progreso += 2;
            progressBarLogin.Value = progreso;

            if (progreso >= 100)
            {
                timer.Stop();
                progressBarLogin.Visibility = Visibility.Collapsed;

                string Usuario = txtNombreSeleccionado.Text.Trim();
                string Clave = txtPassword.Password;
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                var ingresousuariosrepositorio = new RepositorioIngresoApp(ConexionSql);
                _UsuarioID = ingresousuariosrepositorio.BuscarUsuarioID(Usuario, Clave);
                var CacheAplicacion = new CacheAplicacion(ConexionSql);
                CacheAplicacion.LlenarCacheAplicacion(_UsuarioID);
                int TipoUsuario = ingresousuariosrepositorio.LeerTipoUsuario(_UsuarioID);
                if (TipoUsuario == 2)
                {
                    var UsuarioID = RepositorioIngresoApp.UsuariosID.UsuarioID;
                    VentanaAbrePos ventana = new VentanaAbrePos(UsuarioID);
                    ventana.ShowDialog();
                }
                else
                {
                    Dashboard dashboard = new Dashboard();
                    dashboard.Show();
                }

                this.Close();
            }
        }

        private void llenarConsumidor()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Consulta = @"select TercerosID from InveNitConsumidor";
                    using (SqlCommand cmd = new SqlCommand(Consulta, conexion))
                    {
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataTable dataTable = new DataTable();
                        adp.Fill(dataTable);
                        if (dataTable.Rows.Count > 0)
                        {
                            NitConsumidorfinal.IDconsumidor = Convert.ToInt32(dataTable.Rows[0]["TercerosID"]);
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
}
