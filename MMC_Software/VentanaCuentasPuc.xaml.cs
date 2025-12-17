using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaCuentasPuc.xaml
    /// </summary>
    public partial class VentanaCuentasPuc : UserControl
    {
        public VentanaCuentasPuc()
        {
            InitializeComponent();
            muestracuentasPuc();
        }

        private void muestracuentasPuc()
        {
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

            using (SqlConnection conexion = new SqlConnection(ConexionSql))
            {
                conexion.Open();
                string ConsultaTraeCuentas = "select TOP(100)  CONCAT(CuentasPucCodigo,'   ',CuentasPucNombre)  AS INFOCOMPLETA ,CuentasPucID from ConfCuentasPuc order by CuentasPucCodigo asc";
                SqlDataAdapter adp = new SqlDataAdapter(ConsultaTraeCuentas, conexion);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                listCuentasPuc.DisplayMemberPath = "INFOCOMPLETA";
                listCuentasPuc.SelectedValuePath = "CuentasPucID";
                listCuentasPuc.ItemsSource = dt.DefaultView;
            }
        }

        private void listCuentasPuc_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MontaInfoCuentas();
        }

        private void MontaInfoCuentas()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string MontaInfoCuentas = "select ConceptoDianID , CuentasPucCodigo,CuentasPucNombre,CuentasPucNaturaleza,CuentaPermiteMov,CuentaPermiteTer from ConfCuentasPuc where CuentasPucID=@ID";
                    SqlCommand cmd = new SqlCommand(MontaInfoCuentas, conexion);
                    cmd.Parameters.AddWithValue("@ID", listCuentasPuc.SelectedValue);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        txtCodigo.Text = dt.Rows[0]["CuentasPucCodigo"].ToString();
                        txtNombre.Text = dt.Rows[0]["CuentasPucNombre"].ToString();
                        txtNaturaleza.Text = dt.Rows[0]["CuentasPucNaturaleza"].ToString();
                        int movimiento = Convert.ToInt32(dt.Rows[0]["CuentaPermiteMov"]);
                        chkPermiteMov.IsChecked = movimiento == 1;
                        int tercero = Convert.ToInt32(dt.Rows[0]["CuentaPermiteTer"]);
                        chkPermiteTer.IsChecked = tercero == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            txtCodigo.Text = "";
            txtNaturaleza.Text = "";
            txtNombre.Text = "";
            chkPermiteMov.IsEnabled = true;
            chkPermiteTer.IsEnabled = true;
            txtCodigo.IsEnabled = true;
            txtNombre.IsEnabled = true;
            txtNaturaleza.IsEnabled = true;
            cbConceptoDian.IsEnabled = true;
        }

        private void muestraConceptos()
        {
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion("MMC");

            using (SqlConnection conexion = new SqlConnection(ConexionSql))
            {
                conexion.Open();
                string ConsultaMuestraConceptos = "SELECT ConceptoDianId, CONCAT(ConceptoDianCodigo,'  ',ConceptoDianNombre) AS INFOCOMPLETA FROM ConfConceptosDian";
                SqlDataAdapter ADP = new SqlDataAdapter(ConsultaMuestraConceptos, conexion);
                DataTable dt = new DataTable();
                ADP.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    cbConceptoDian.DisplayMemberPath = "INFOCOMPLETA";
                    cbConceptoDian.SelectedValuePath = "ConceptoDianId";
                    cbConceptoDian.ItemsSource = dt.DefaultView;
                }
            }

        }

        private void cbConceptoDian_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            muestraConceptos();
        }

        private void GuardaActualiza()
        {
            try
            {
                string codigoCuenta = txtCodigo.Text;
                string NombreCuenta = txtNombre.Text;
                // check box , 1 est marcado 0 no esta marcado
                int valorCheck = chkPermiteTer.IsChecked == true ? 1 : 0;
                int ValorChecckt = chkPermiteMov.IsChecked == true ? 1 : 0;

                if (string.IsNullOrEmpty(codigoCuenta))
                {
                    MessageBox.Show("El codigo de la cuenta no puede estar vacio o con espacio", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string ConsultaExiste = "select count (*)  from confcuentaspuc where CuentasPucCodigo=@codigo";
                    SqlCommand cmd = new SqlCommand(ConsultaExiste, conexion);
                    cmd.Parameters.AddWithValue("@codigo", codigoCuenta);
                    int existe = Convert.ToInt32(cmd.ExecuteScalar());
                    if (existe > 0)
                    {
                        string consultaNombre = "select CuentasPucNombre from ConfCuentasPuc where CuentasPucCodigo=@Codigo";
                        SqlCommand cmdnombre = new SqlCommand(consultaNombre, conexion);
                        cmdnombre.Parameters.AddWithValue("@Codigo", codigoCuenta);
                        string nombreviejo = cmdnombre.ExecuteScalar().ToString();
                        if (nombreviejo != NombreCuenta)
                        {
                            string updateNombrecuenta = "update ConfCuentasPuc set CuentasPucNombre=@nombre where CuentasPucCodigo=@codigo";
                            SqlCommand cmdupdate = new SqlCommand(updateNombrecuenta, conexion);
                            cmdupdate.Parameters.AddWithValue("@nombre", NombreCuenta);
                            cmdupdate.Parameters.AddWithValue("@codigo", codigoCuenta);
                            cmdupdate.ExecuteNonQuery();
                        }

                        if (valorCheck == 1)
                        {
                            string updatemovimiento = "UPDATE ConfCuentasPuc SET CuentaPermiteMov=1 where CuentasPucCodigo=@codigo";
                            SqlCommand cmdUpdatemovsi = new SqlCommand(updatemovimiento, conexion);
                            cmdUpdatemovsi.Parameters.AddWithValue("@codigo", codigoCuenta);
                            cmdUpdatemovsi.ExecuteNonQuery();
                            MessageBox.Show("Registro Actualizado Correctamente", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtCodigo.IsEnabled = false;
                            txtNaturaleza.IsEnabled = false;
                            txtNaturaleza.IsEnabled = false;
                            cbConceptoDian.IsEnabled = false;
                            chkPermiteMov.IsEnabled = false;
                            chkPermiteTer.IsEnabled = false;
                            txtNombre.IsEnabled = false;
                        }
                        else
                        {
                            string updatemovimiento = "UPDATE ConfCuentasPuc SET CuentaPermiteMov=0 where CuentasPucCodigo=@codigo";
                            SqlCommand cmdUpdatemovno = new SqlCommand(updatemovimiento, conexion);
                            cmdUpdatemovno.Parameters.AddWithValue("@codigo", codigoCuenta);
                            cmdUpdatemovno.ExecuteNonQuery();
                            MessageBox.Show("Registro Actualizado Correctamente", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtCodigo.IsEnabled = false;
                            txtNaturaleza.IsEnabled = false;
                            txtNaturaleza.IsEnabled = false;
                            cbConceptoDian.IsEnabled = false;
                            chkPermiteMov.IsEnabled = false;
                            chkPermiteTer.IsEnabled = false;
                            txtNombre.IsEnabled = false;
                        }
                        if (ValorChecckt == 1)
                        {
                            string updatetercero = "UPDATE ConfCuentasPuc SET CuentaPermiteTer=1 where CuentasPucCodigo=@codigo";
                            SqlCommand cmdUpdateter = new SqlCommand(updatetercero, conexion);
                            cmdUpdateter.Parameters.AddWithValue("@codigo", codigoCuenta);
                            cmdUpdateter.ExecuteNonQuery();
                            txtCodigo.IsEnabled = false;
                            txtNaturaleza.IsEnabled = false;
                            txtNaturaleza.IsEnabled = false;
                            cbConceptoDian.IsEnabled = false;
                            chkPermiteMov.IsEnabled = false;
                            chkPermiteTer.IsEnabled = false;
                            txtNombre.IsEnabled = false;
                        }
                        else
                        {
                            string updatetercero = "UPDATE ConfCuentasPuc SET CuentaPermiteTer=0 where CuentasPucCodigo=@codigo";
                            SqlCommand cmdUpdateter1 = new SqlCommand(updatetercero, conexion);
                            cmdUpdateter1.Parameters.AddWithValue("@codigo", codigoCuenta);
                            cmdUpdateter1.ExecuteNonQuery();
                            txtCodigo.IsEnabled = false;
                            txtNaturaleza.IsEnabled = false;
                            txtNaturaleza.IsEnabled = false;
                            cbConceptoDian.IsEnabled = false;
                            chkPermiteMov.IsEnabled = false;
                            chkPermiteTer.IsEnabled = false;
                            txtNombre.IsEnabled = false;
                        }
                    }

                    else
                    {
                        CreaCliente();
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void CreaCliente()
        {
            try
            {
                string codigo = txtCodigo.Text;
                string tiponaturaleza = "";
                int tipocuenta = 0;
                int permitemov = chkPermiteMov.IsChecked == true ? 1 : 0;
                int permiteter = chkPermiteTer.IsChecked == true ? 1 : 0;
                int conceptoid = Convert.ToInt32(cbConceptoDian.SelectedValue);

                if (!string.IsNullOrEmpty(codigo) && char.IsDigit(codigo[0]))
                {
                    char primerdigito = codigo[0];

                    switch (primerdigito)
                    {
                        case '1':
                            tiponaturaleza = "D";
                            tipocuenta = 1;
                            break;
                        case '2':
                            tiponaturaleza = "C";
                            tipocuenta = 2;
                            break;
                        case '3':
                            tiponaturaleza = "C";
                            tipocuenta = 3;
                            break;
                        case '4':
                            tiponaturaleza = "C";
                            tipocuenta = 4;
                            break;
                        case '5':
                            tiponaturaleza = "D";
                            tipocuenta = 5;
                            break;
                        case '6':
                            tiponaturaleza = "D";
                            tipocuenta = 6;
                            break;
                        case '7':
                            tiponaturaleza = "D";
                            tipocuenta = 7;
                            break;
                        case '8':
                            tiponaturaleza = "D";
                            tipocuenta = 8;
                            break;
                        case '9':
                            tiponaturaleza = "C";
                            tipocuenta = 9;
                            break;
                    }
                }

                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    SqlTransaction transaccion = conexion.BeginTransaction();

                    try
                    {
                        // INSERT PRINCIPAL
                        string InsertCuentaPuc = @"
                    INSERT INTO ConfCuentasPuc(
                        ConceptoDianID,
                        CuentasPucCodigo,
                        CuentasPucNombre,
                        CuentasPucNaturaleza,
                        CuentaPermiteMov,
                        CuentaPermiteTer,
                        CuentasPucTipo
                    ) 
                    VALUES (
                        @concepto,
                        @cuenta,
                        @nombre,
                        @naturaleza,
                        @movimiento,
                        @tercero,
                        @tipo
                    )";

                        SqlCommand cmd = new SqlCommand(InsertCuentaPuc, conexion, transaccion);
                        cmd.Parameters.AddWithValue("@concepto", conceptoid);
                        cmd.Parameters.AddWithValue("@cuenta", codigo);
                        cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                        cmd.Parameters.AddWithValue("@naturaleza", tiponaturaleza);
                        cmd.Parameters.AddWithValue("@movimiento", permitemov);
                        cmd.Parameters.AddWithValue("@tercero", permiteter);
                        cmd.Parameters.AddWithValue("@tipo", tipocuenta);
                        cmd.ExecuteNonQuery();

                        // ACTUALIZAR CUENTAS PADRE
                        for (int i = codigo.Length - 2; i >= 2; i -= 2)
                        {
                            string padre = codigo.Substring(0, i);

                            string updatePadre = @"
                        UPDATE ConfCuentasPuc
                        SET CuentaPermiteMov = 0, CuentaPermiteTer = 0
                        WHERE CuentasPucCodigo = @codigoPadre";

                            SqlCommand cmdUpdate = new SqlCommand(updatePadre, conexion, transaccion);
                            cmdUpdate.Parameters.AddWithValue("@codigoPadre", padre);
                            cmdUpdate.ExecuteNonQuery();
                        }

                        transaccion.Commit();
                        conexion.Close();

                        MessageBox.Show("Cuenta creada correctamente y mayores actualizados.");

                        muestracuentasPuc();
                        txtCodigo.IsEnabled = false;
                        txtNaturaleza.IsEnabled = false;
                        txtNaturaleza.IsEnabled = false;
                        cbConceptoDian.IsEnabled = false;
                        chkPermiteMov.IsEnabled = false;
                        chkPermiteTer.IsEnabled = false;
                        txtNombre.IsEnabled = false;
                    }
                    catch (Exception ex)
                    {
                        transaccion.Rollback();
                        MessageBox.Show("Error en la transacción: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error general: " + ex.Message);
            }
        }



        private void txtNombre_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox txt = sender as TextBox;
            txt.Text = txt.Text.ToUpper();
        }

        private void txtCodigo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            GuardaActualiza();
        }

        private void eliminaCuenta()
        {
            try
            {
                int Idcuenta = Convert.ToInt32(listCuentasPuc.SelectedValue);

                if (listCuentasPuc.SelectedValue != null)
                {
                    MessageBoxResult resultado = MessageBox.Show("¿Estas seguro de borrar cuenta?", "MENSAJE", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (resultado == MessageBoxResult.Yes)
                    {

                        string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                        using (SqlConnection conexion = new SqlConnection(ConexionSql))
                        {
                            conexion.Open();

                            string EliminaCuenta = "DELETE FROM ConfCuentasPuc where CuentasPucID=@id";
                            SqlCommand cmd = new SqlCommand(EliminaCuenta, conexion);
                            cmd.Parameters.AddWithValue("@id", Idcuenta);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Cuenta Borrada Correctamento", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Debe seleccionar cuenta a eliminar", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            eliminaCuenta();
            muestracuentasPuc();
        }

        private void txtBuscarCuenta_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txtbuscar = txtBuscarCuenta.Text.Trim();
            FiltrarCuenta(txtbuscar);

        }


        private void FiltrarCuenta(string filtro)
        {
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

            using (SqlConnection conexion = new SqlConnection(ConexionSql))
            {
                conexion.Open();
                string ConsultaEnLinea = @" SELECT TOP(100) 
                CONCAT(CuentasPucCodigo,'   ',CuentasPucNombre) AS INFOCOMPLETA, 
                CuentasPucID 
                FROM ConfCuentasPuc 
                WHERE CuentasPucCodigo LIKE @filtro OR CuentasPucNombre LIKE @filtro
                ORDER BY CuentasPucCodigo ASC";

                SqlCommand cmd = new SqlCommand(ConsultaEnLinea, conexion);
                cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                listCuentasPuc.DisplayMemberPath = "INFOCOMPLETA";
                listCuentasPuc.SelectedValuePath = "CuentasPucID";
                listCuentasPuc.ItemsSource = dt.DefaultView;
            }
        }

        private void btnModificar_Click_1(object sender, RoutedEventArgs e)
        {
            txtNombre.IsEnabled = true;
            cbConceptoDian.IsEnabled = true;
            chkPermiteMov.IsEnabled = true;
            chkPermiteTer.IsEnabled = true;

        }

        private void btnCancelar_Click_1(object sender, RoutedEventArgs e)
        {
            txtNombre.IsEnabled = false;
            cbConceptoDian.IsEnabled = false;
            chkPermiteMov.IsEnabled = false;
            chkPermiteTer.IsEnabled = false;
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            var TabAbierto = this.Parent as TabItem;
            if (TabAbierto != null)
            {
                var item = TabAbierto.Parent as TabControl;
                if (item != null)
                {
                    item.Items.Remove(TabAbierto);
                }
            }
        }
    }
}
