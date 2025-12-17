using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaCrearFormasPago.xaml
    /// </summary>
    public partial class VentanaCrearFormasPago : UserControl
    {
        public VentanaCrearFormasPago()
        {
            InitializeComponent();
            MuestraMedios();
            MuestraTipos();
            listaFormaspago();
        }

        private void txtCuentaPucF1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                VentanaBuscarCuentas ventana = new VentanaBuscarCuentas();
                bool? Resultado = ventana.ShowDialog();

                if (Resultado == true)
                {
                    txtCuentaPuc.Text = ventana.CuentaSeleccionada;
                    txtCuentaPuc.IsEnabled = false;
                    txtNombreCuenta.Text = ventana.NombreCuentaSeleccionada;
                    txtNombreCuenta.IsEnabled = false;
                    txtCuentaPuc.Tag = ventana.Idcuenta;
                }
            }
        }

        private void MuestraTipos()
        {
            var tipo = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("1","Cliente"),
                new KeyValuePair<string, string>("2","Proveedor"),
                new KeyValuePair<string, string>("3","Empleado")
            };
            cmbTipo.ItemsSource = tipo;
            cmbTipo.DisplayMemberPath = "Value";
            cmbTipo.SelectedValuePath = "Key";
        }

        private void MuestraMedios()
        {
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

            using (SqlConnection conexion = new SqlConnection(ConexionSql))
            {
                string consultaMedios = @"select CONCAT(CodigoMedioPago,'  ',NombreMedioPago)as INFOCOMPLETA , MedioPagoId  from ConfMediosPago";
                SqlDataAdapter adp = new SqlDataAdapter(consultaMedios, conexion);
                DataTable dataTable = new DataTable();
                adp.Fill(dataTable);
                cmbMedioPago.DisplayMemberPath = "INFOCOMPLETA";
                cmbMedioPago.SelectedValuePath = "MedioPagoId";
                cmbMedioPago.ItemsSource = dataTable.DefaultView;
            }
        }

        private void listaFormaspago()
        {
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

            using (SqlConnection conexion = new SqlConnection(ConexionSql))
            {
                conexion.Open();
                string consultaMontaFormaPago = @"select CONCAT(CodigoFormaPago,'  ',NombreFormaPago) AS INFOCOMPLETA , FormaPagoID from ConfFormasPago";
                SqlDataAdapter adp = new SqlDataAdapter(consultaMontaFormaPago, conexion);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                lstFormasPago.DisplayMemberPath = "INFOCOMPLETA";
                lstFormasPago.SelectedValuePath = "FormaPagoID";
                lstFormasPago.ItemsSource = dt.DefaultView;
            }
        }

        private void CrearFormasPago()
        {
            string Codigo = txtCodigoFormaPago.Text;
            string NombreFormaPago = txtNombreFormaPago.Text;
            int tipoforma = Convert.ToInt32(cmbTipo.SelectedValue);
            int manejacartera = chkCuentaXPagar.IsChecked == true ? 1 : 0;
            int mediopago = Convert.ToInt32(cmbMedioPago.SelectedValue);
            int idCuenta = Convert.ToInt32(txtCuentaPuc.Tag);
            string Naturaleza = txtNaturaleza.Text;
            int manejaCambio = chkPideCambio.IsChecked == true ? 1 : 0;

            if (string.IsNullOrEmpty(Codigo) || string.IsNullOrEmpty(NombreFormaPago) || txtCuentaPuc.Tag == null || string.IsNullOrEmpty(Naturaleza)
               || mediopago.ToString() == null)
            {
                MessageBox.Show("El codigo de la forma de pago o nombre o Cuenta Puc no pueden Estar vacios");
            }
            else
            {
                try
                {
                    string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                    using (SqlConnection conexion = new SqlConnection(ConexionSql))
                    {
                        conexion.Open();
                        string insertFormasPago = @"INSERT INTO ConfFormasPago(CodigoFormaPago,NombreFormaPago,FormaPagoTipo,CuentasPucID,MedioPagoID,ManejaCartera,Naturaleza,ManejaCambio) 
                                                  VALUES(@codigo,@nombre,@tipo,@cuentaid,@medioid,@manejacartera,@naturaleza,@cambio)";
                        SqlCommand cmd = new SqlCommand(insertFormasPago, conexion);

                        cmd.Parameters.AddWithValue("@codigo", Codigo);
                        cmd.Parameters.AddWithValue("@nombre", NombreFormaPago);
                        cmd.Parameters.AddWithValue("@tipo", Convert.ToInt32(cmbTipo.SelectedValue));
                        cmd.Parameters.AddWithValue("@cuentaid", idCuenta);
                        cmd.Parameters.AddWithValue("@medioid", mediopago);
                        cmd.Parameters.AddWithValue("@manejacartera", manejacartera);
                        cmd.Parameters.AddWithValue("@naturaleza", Naturaleza);
                        cmd.Parameters.AddWithValue("@cambio", manejaCambio);
                        cmd.ExecuteNonQuery();
                        txtCodigoFormaPago.IsEnabled = false;
                        txtCuentaPuc.IsEnabled = false;
                        txtNombreCuenta.IsEnabled = false;
                        txtNombreFormaPago.IsEnabled = false;
                        chkCuentaXPagar.IsEnabled = false;
                        cmbMedioPago.IsEnabled = false;
                        MessageBox.Show("Forma de pago creada correctamente", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
                        listaFormaspago();
                        txtCodigoFormaPago.IsEnabled = false;
                        txtNombreFormaPago.IsEnabled = false;
                        txtCuentaPuc.IsEnabled = false;
                        txtNaturaleza.IsEnabled = false;
                        txtNombreCuenta.IsEnabled = false;
                        cmbTipo.IsEnabled = false;
                        chkCuentaXPagar.IsEnabled = false;
                        cmbMedioPago.IsEnabled = false;
                        txtCodigoFormaPago.Text = "";
                        txtNombreCuenta.Text = "";
                        txtCuentaPuc.Text = "";
                        txtNaturaleza.Text = "";
                        txtNombreFormaPago.Text = "";

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error" + ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void LlenainfoFormaPagos()
        {
            try
            {
                string CodigoFormaPago = txtCodigoFormaPago.Text;

                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Consultamontainfo = @"select CodigoFormaPago,NombreFormaPago,FormaPagoTipo,CuentasPucID,Naturaleza,MedioPagoID,ManejaCambio
                                                from ConfFormasPago where FormaPagoID=@id";
                    SqlCommand cmd = new SqlCommand(Consultamontainfo, conexion);
                    cmd.Parameters.AddWithValue("@id", lstFormasPago.SelectedValue);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        txtCodigoFormaPago.Text = dt.Rows[0]["CodigoFormaPago"].ToString();
                        txtNombreFormaPago.Text = dt.Rows[0]["NombreFormaPago"].ToString();
                        txtNaturaleza.Text = dt.Rows[0]["Naturaleza"].ToString();
                        int Idmediopago = Convert.ToInt32(dt.Rows[0]["MedioPagoID"]);
                        int TipoFormapago = Convert.ToInt32(dt.Rows[0]["FormaPagoTipo"]);
                        int PideCambio = Convert.ToInt32(dt.Rows[0]["ManejaCambio"]);
                        int cuentaId = Convert.ToInt32(dt.Rows[0]["CuentasPucID"]);
                        txtCuentaPuc.Tag = cuentaId;
                        MuestraMedios();
                        cmbMedioPago.SelectedValue = Idmediopago;
                        MuestraTipos();
                        chkPideCambio.IsChecked = PideCambio == 1;
                        cmbTipo.SelectedValue = TipoFormapago;
                        string ConsultaCuenta = @"select CuentasPucCodigo, CuentasPucNombre from ConfCuentasPuc where CuentasPucID=@id";
                        SqlCommand cmdcuenta = new SqlCommand(ConsultaCuenta, conexion);
                        cmdcuenta.Parameters.AddWithValue("@id", cuentaId);
                        SqlDataAdapter adpCuenta = new SqlDataAdapter(cmdcuenta);
                        DataTable dtcuenta = new DataTable();
                        adpCuenta.Fill(dtcuenta);
                        if (dtcuenta.Rows.Count > 0)
                        {
                            txtCuentaPuc.Text = dtcuenta.Rows[0]["CuentasPucCodigo"].ToString();
                            txtNombreCuenta.Text = dtcuenta.Rows[0]["CuentasPucNombre"].ToString();
                            txtCodigoFormaPago.IsEnabled = false;
                            txtNombreFormaPago.IsEnabled = false;
                            txtCuentaPuc.IsEnabled = false;
                            txtNaturaleza.IsEnabled = false;
                            txtNombreCuenta.IsEnabled = false;
                            cmbTipo.IsEnabled = false;
                            chkCuentaXPagar.IsEnabled = false;
                            cmbMedioPago.IsEnabled = false;
                            chkPideCambio.IsEnabled = false;
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void EliminarFormaPago()
        {
            MessageBoxResult Pregunta = MessageBox.Show("¿Estas seguro de eliminar forma de pago?", "Mensaje", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (Pregunta == MessageBoxResult.Yes)
            {
                if (lstFormasPago.SelectedValue != null)
                {
                    string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                    using (SqlConnection conexion = new SqlConnection(ConexionSql))
                    {
                        conexion.Open();

                        string DeleteFormaPago = @"delete from ConfFormasPago where FormaPagoID=@id";

                        SqlCommand cmd = new SqlCommand(DeleteFormaPago, conexion);

                        cmd.Parameters.AddWithValue("@id", lstFormasPago.SelectedValue);

                        cmd.ExecuteNonQuery();

                        conexion.Close();
                    }
                }

                else
                {
                    MessageBox.Show("Proceso cancelado por el usuario", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }

        }

        private void GuardaActualiza()
        {
            string Codigo = txtCodigoFormaPago.Text;
            string NombreFormaPago = txtNombreFormaPago.Text;
            int tipoforma = Convert.ToInt32(cmbTipo.SelectedValue);
            int manejacartera = chkCuentaXPagar.IsChecked == true ? 1 : 0;
            int mediopago = Convert.ToInt32(cmbMedioPago.SelectedValue);
            int idCuenta = Convert.ToInt32(txtCuentaPuc.Tag);
            string Naturaleza = txtNaturaleza.Text;
            int PideCambio = chkPideCambio.IsChecked == true ? 1 : 0;

            //if (string.IsNullOrEmpty(Codigo) ||
            //    string.IsNullOrEmpty(NombreFormaPago) ||
            //    cmbTipo.SelectedValue == null ||
            //    txtCuentaPuc.Tag == null)
            //{
            //    MessageBox.Show("Faltan datos obligatorios para guardar la forma de pago.");
            //    return;
            //}
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();

                    string consultaExiste = @"select COUNT(*)  from ConfFormasPago where CodigoFormaPago=@codigo";
                    SqlCommand cmd = new SqlCommand(consultaExiste, conexion);
                    cmd.Parameters.AddWithValue("@codigo", Codigo);
                    int cantidad = Convert.ToInt32(cmd.ExecuteScalar());

                    if (cantidad > 0)
                    {

                        MessageBoxResult pregunta = MessageBox.Show(@"¿Desea Actualizar la informacion de la forma de pago?", "Mensaje",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (pregunta == MessageBoxResult.Yes)
                        {
                            string ConsultaNombre = @"select NombreFormaPago from ConfFormasPago where CodigoFormaPago=@codigo";
                            SqlCommand cmdnombre = new SqlCommand(ConsultaNombre, conexion);
                            cmdnombre.Parameters.AddWithValue("@codigo", Codigo);
                            string NombreBD = cmdnombre.ExecuteScalar().ToString();

                            if (NombreBD != NombreFormaPago)
                            {
                                string updatenombre = "update ConfFormasPago set NombreFormaPago=@nombre  where CodigoFormaPago=@codigo";
                                SqlCommand cmdupdatenombre = new SqlCommand(updatenombre, conexion);
                                cmdupdatenombre.Parameters.AddWithValue("@nombre", NombreFormaPago);
                                cmdupdatenombre.Parameters.AddWithValue("@codigo", Codigo);
                                cmdupdatenombre.ExecuteNonQuery();

                            }

                            string ConsltaCuentaPuc = @"SELECT CuentasPucID FROM ConfFormasPago WHERE CodigoFormaPago=@codigo";
                            SqlCommand cmdidcuenta = new SqlCommand(ConsltaCuentaPuc, conexion);
                            cmdidcuenta.Parameters.AddWithValue("@codigo", Codigo);
                            int cuentapuc = Convert.ToInt32(cmdidcuenta.ExecuteScalar());

                            if (cuentapuc != idCuenta)
                            {
                                string updateCuenta = @"UPDATE ConfFormasPago SET CuentasPucID=@cuenta WHERE CodigoFormaPago=@codigo";
                                SqlCommand cmdupdatecuenta = new SqlCommand(updateCuenta, conexion);
                                cmdupdatecuenta.Parameters.AddWithValue("@cuenta", idCuenta);
                                cmdupdatecuenta.Parameters.AddWithValue("@codigo", Codigo);
                                cmdupdatecuenta.ExecuteNonQuery();
                                MessageBox.Show("Cuenta Actualizada correctamente", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);

                            }
                            string ConsultaTipo = @"select FormaPagoTipo from ConfFormasPago where CodigoFormaPago=@codigo";
                            SqlCommand cmdtipo = new SqlCommand(ConsultaTipo, conexion);
                            cmdtipo.Parameters.AddWithValue("@codigo", Codigo);
                            int tipoBD = Convert.ToInt32(cmdtipo.ExecuteScalar());
                            if (tipoBD != tipoforma)
                            {
                                String updatetipo = @"update ConfFormasPago set FormaPagoTipo=@tipo where CodigoFormaPago=@codigo";
                                SqlCommand cmdupdatetipo = new SqlCommand(updatetipo, conexion);
                                cmdupdatetipo.Parameters.AddWithValue("@tipo", tipoforma);
                                cmdupdatetipo.Parameters.AddWithValue("@codigo", Codigo);
                                cmdupdatetipo.ExecuteNonQuery();
                            }

                            string Consultanaturaleza = @"select  Naturaleza from ConfFormasPago where CodigoFormaPago=@codigo";
                            SqlCommand cmdnaturaleza = new SqlCommand(Consultanaturaleza, conexion);
                            cmdnaturaleza.Parameters.AddWithValue("@codigo", Codigo);
                            string NaturalezaBD = cmdnaturaleza.ExecuteScalar().ToString();

                            if (NaturalezaBD != Naturaleza)
                            {
                                string updatenaturaleza = @"UPDATE ConfFormasPago SET Naturaleza=@naturaleza where CodigoFormaPago=@codigo";
                                SqlCommand cmdupdatenaturaleza = new SqlCommand(updatenaturaleza, conexion);
                                cmdupdatenaturaleza.Parameters.AddWithValue("@naturaleza", Naturaleza);
                                cmdupdatenaturaleza.Parameters.AddWithValue("@codigo", Codigo);
                                cmdupdatenaturaleza.ExecuteNonQuery();
                            }

                            string consultaMedioPago = @"select MedioPagoID from ConfFormasPago where CodigoFormaPago=@codigo";
                            SqlCommand cmdmediopago = new SqlCommand(consultaMedioPago, conexion);
                            cmdmediopago.Parameters.AddWithValue("@codigo", Codigo);
                            int MedioBD = Convert.ToInt32(cmdmediopago.ExecuteScalar());

                            if (MedioBD != mediopago)
                            {
                                string updatemedio = "update ConfFormasPago set MedioPagoID=@medio where CodigoFormaPago=@codigo";
                                SqlCommand cmdupdatemedio = new SqlCommand(updatemedio, conexion);
                                cmdupdatemedio.Parameters.AddWithValue("@medio", mediopago);
                                cmdupdatemedio.Parameters.AddWithValue("@codigo", Codigo);
                                cmdupdatemedio.ExecuteNonQuery();
                            }
                            string consultaPideCambio = @"select ManejaCambio from ConfFormasPago where CodigoFormaPago=@filtro";
                            using (SqlCommand cmdPidecambio = new SqlCommand(consultaPideCambio, conexion))
                            {
                                cmdPidecambio.Parameters.AddWithValue("@filtro", Codigo);
                                int Maneja = Convert.ToInt32(cmdPidecambio.ExecuteScalar());
                                if (Maneja != PideCambio)
                                {
                                    string update = @"update ConfFormasPago set ManejaCambio=@filtro where CodigoFormaPago=@filtro2";
                                    using (SqlCommand cmdUpdate = new SqlCommand(update, conexion))
                                    {
                                        cmdUpdate.Parameters.AddWithValue("@filtro", PideCambio);
                                        cmdUpdate.Parameters.AddWithValue("@filtro2", Codigo);
                                        cmdUpdate.ExecuteNonQuery();
                                    }
                                }
                            }

                            listaFormaspago();
                        }
                    }

                    else
                    {
                        CrearFormasPago();
                        listaFormaspago();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        //-----------------------------------------------------------------------ZONA BOTONES--------------------------------------------------//
        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {

            txtCodigoFormaPago.IsEnabled = true;
            txtNombreFormaPago.IsEnabled = true;
            txtCuentaPuc.IsEnabled = true;
            txtNaturaleza.IsEnabled = true;
            txtNombreCuenta.IsEnabled = true;
            lstFormasPago.IsEnabled = true;
            cmbTipo.IsEnabled = true;
            chkCuentaXPagar.IsEnabled = true;
            cmbMedioPago.IsEnabled = true;
            txtCodigoFormaPago.Text = "";
            txtNombreCuenta.Text = "";
            txtCuentaPuc.Text = "";
            txtNaturaleza.Text = "";
            txtNombreFormaPago.Text = "";
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            GuardaActualiza();
        }

        private void lstFormasPago_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LlenainfoFormaPagos();
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (lstFormasPago.SelectedValue != null)
            {
                txtCodigoFormaPago.IsEnabled = true;
                txtNombreFormaPago.IsEnabled = true;
                txtCuentaPuc.IsEnabled = true;
                txtNaturaleza.IsEnabled = true;
                txtNombreCuenta.IsEnabled = true;
                lstFormasPago.IsEnabled = true;
                cmbTipo.IsEnabled = true;
                chkCuentaXPagar.IsEnabled = true;
                cmbMedioPago.IsEnabled = true;
                chkPideCambio.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Debe seleccionar cuenta a modificar", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            EliminarFormaPago();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VentanaBuscarCuentas ventana = new VentanaBuscarCuentas();
            bool? Resultado = ventana.ShowDialog();

            if (Resultado == true)
            {
                txtCuentaPuc.Text = ventana.CuentaSeleccionada;
                txtCuentaPuc.IsEnabled = false;
                txtNombreCuenta.Text = ventana.NombreCuentaSeleccionada;
                txtNombreCuenta.IsEnabled = false;
                txtCuentaPuc.Tag = ventana.Idcuenta;
            }
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            // PRIMERO LE HAGO UN CAST  
            var TabAbierto = this.Parent as TabItem;
            if (TabAbierto != null)
            {
                //otro cast para el tabcontrol
                var Tabcontrol = TabAbierto.Parent as TabControl;
                if (Tabcontrol != null)
                {
                    Tabcontrol.Items.Remove(TabAbierto); //aqui quito la pestaña 
                }
            }
        }
    }

}