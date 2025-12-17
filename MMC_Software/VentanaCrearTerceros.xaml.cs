using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMC_Software
{
    public partial class VentanaCrearTerceros : Window
    {
        private int TercerosID;

        private List<InfoCiudad> CiudadesBD; // Aqui voy a almacenar las ciudades de la base de datos
        public VentanaCrearTerceros(int TercerosID)

        {
            InitializeComponent();
            this.TercerosID = TercerosID;
            MuestraTipoTerceros();
            MontarDatosTerceros();
            BloquearCampos();
            CargarCiudades();
            MuestraListaPrecios();
        }

        public VentanaCrearTerceros()
        {
            InitializeComponent();
            MuestraTipoTerceros();
            BloquearCampos();
            CargarCiudades();
            MuestraListaPrecios();
        }

        public class InfoCiudad
        {
            public int CiudadID { get; set; }
            public string NombreCiudad { get; set; }
            public int DepartamentoID { get; set; }
            public string NombreDepartamento { get; set; }

        }

        public int CiudadSeleccionada;
        public int DepartamentoSeleccionada;

        private void MuestraListaPrecios()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string consulta = "select CONCAT(CodigoListaPrecio,'  ',NombreListaPrecios) as infocompleta, ListaPreciosID from ConfListaPrecios";
                    SqlDataAdapter adp = new SqlDataAdapter(consulta, conexion);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    cbListaPrecios.DisplayMemberPath = "infocompleta";
                    cbListaPrecios.SelectedValuePath = "ListaPreciosID";
                    cbListaPrecios.ItemsSource = dt.DefaultView;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.ToString());
            }
        }

        private void BloquearCampos()
        {
            cbTipoDocumento.IsEnabled = false;
            txtDepartamento.IsEnabled = false;
            lstCiudad.IsEnabled = false;
            txtIdentificacion.IsEnabled = false;
            txtPrimerNombre.IsEnabled = false;
            txtSegundoNombre.IsEnabled = false;
            txtPrimerApellido.IsEnabled = false;
            txtSegundoApellido.IsEnabled = false;
            txtRazonSocial.IsEnabled = false;
            txtEstablecimiento.IsEnabled = false;
            txtDireccion.IsEnabled = false;
            txtTelefono.IsEnabled = false;
            txtEmail.IsEnabled = false;
            txtDV.IsEnabled = false;
            txtbuscadorCiudad.IsEnabled = false;
            chkCliente.IsEnabled = false;
            chkEmpleado.IsEnabled = false;
            chkProveedor.IsEnabled = false;
            cbListaPrecios.IsEnabled = false;

        }

        private void desbloquearCampos()
        {
            cbTipoDocumento.IsEnabled = true;
            txtDepartamento.IsEnabled = true;
            lstCiudad.IsEnabled = true;
            txtIdentificacion.IsEnabled = true;
            txtPrimerNombre.IsEnabled = true;
            txtSegundoNombre.IsEnabled = true;
            txtPrimerApellido.IsEnabled = true;
            txtSegundoApellido.IsEnabled = true;
            txtRazonSocial.IsEnabled = true;
            txtEstablecimiento.IsEnabled = true;
            txtDireccion.IsEnabled = true;
            txtTelefono.IsEnabled = true;
            txtEmail.IsEnabled = true;
            txtDV.IsEnabled = true;
            txtbuscadorCiudad.IsEnabled = true;
            chkCliente.IsEnabled = true;
            chkEmpleado.IsEnabled = true;
            chkProveedor.IsEnabled = true;
            cbListaPrecios.IsEnabled = true;
        }

        private void MuestraTipoTerceros()
        {
            var TiposTerceros = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(13,"Cédula de Ciudadanía"),
                new KeyValuePair<int, string>(12,"Tarjeta de Identidad"),
                new KeyValuePair<int, string>(31,"NIT"),
                new KeyValuePair<int, string>(21,"Cédula de Extranjería"),
                new KeyValuePair<int, string>(41,"Pasaporte")
            };

            cbTipoDocumento.ItemsSource = TiposTerceros;
            cbTipoDocumento.SelectedValuePath = "Key";
            cbTipoDocumento.DisplayMemberPath = "Value";
        }

        private void MontarDatosTerceros()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string consulta = @"SELECT TER.TercerosTipoDocumento, TER.TercerosIdentificacion, TER.TercerosPrimerNombre, 
                                        TER.TercerosSegundoNombre, TER.TercerosPrimerApellido,TER.TercerosSegundoApellido,
                                        TER.TerceroRazonSocial, TER.TercerosEstablecimiento,TER.TercerosDireccion, 
                                        TER.TercerosTelefono,TER.TercerosEmail,DEP.DepartamentoNombre,CIU.CiudadNombre,TER.TerceroCliente,
										TER.TerceroProveedor,TER.TerceroEmpleado
                                        FROM InveTerceros TER , ConfDepartamento DEP,ConfCiudad CIU
                                        WHERE TER.DepartamentoID=DEP.DepartamentoID AND TER.CiudadID=CIU.CiudadID
										AND TER.TercerosID=@id";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@id", TercerosID);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        txtIdentificacion.Text = dt.Rows[0]["TercerosIdentificacion"].ToString();
                        txtPrimerNombre.Text = dt.Rows[0]["TercerosPrimerNombre"].ToString();
                        txtSegundoNombre.Text = dt.Rows[0]["TercerosSegundoNombre"].ToString();
                        txtPrimerApellido.Text = dt.Rows[0]["TercerosPrimerApellido"].ToString();
                        txtSegundoApellido.Text = dt.Rows[0]["TercerosSegundoApellido"].ToString();
                        txtRazonSocial.Text = dt.Rows[0]["TerceroRazonSocial"].ToString();
                        txtEstablecimiento.Text = dt.Rows[0]["TercerosEstablecimiento"].ToString();
                        txtDireccion.Text = dt.Rows[0]["TercerosDireccion"].ToString();
                        txtTelefono.Text = dt.Rows[0]["TercerosTelefono"].ToString();
                        txtEmail.Text = dt.Rows[0]["TercerosEmail"].ToString();
                        // Calcula DV solo si hay identificación cargada
                        CalcularYMostrarDV();
                        txtDepartamento.Text = dt.Rows[0]["DepartamentoNombre"].ToString();
                        txtbuscadorCiudad.Text = dt.Rows[0]["CiudadNombre"].ToString();
                        MuestraTipoTerceros();
                        int TipoDocumento = Convert.ToInt32(dt.Rows[0]["TercerosTipoDocumento"]);
                        cbTipoDocumento.SelectedValue = TipoDocumento;
                        int Tcliente = Convert.ToInt32(dt.Rows[0]["TerceroCliente"]);
                        int Tproveedores = Convert.ToInt32(dt.Rows[0]["TerceroProveedor"]);
                        int Templeado = Convert.ToInt32(dt.Rows[0]["TerceroEmpleado"]);
                        chkCliente.IsChecked = Tcliente == 1;
                        chkEmpleado.IsChecked = Tproveedores == 1;
                        chkEmpleado.IsChecked = Templeado == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message);
            }
        }

        /// <summary>
        /// Calcula el dígito de verificación siguiendo el método DIAN:
        /// - Secuencia de pesos: 71,67,59,...,3 (15 posiciones)
        /// - Se realiza suma ponderada, resto = suma % 11, DV = (11 - resto) si resto > 1, sino resto.
        /// </summary>
        public static int CalcularDigitoVerificacion(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new ArgumentException("El número no puede estar vacío.");

            // Limpiar (solo dígitos)
            string limpio = Regex.Replace(numero, @"\D", "");
            if (string.IsNullOrEmpty(limpio))
                throw new ArgumentException("El número no contiene dígitos.");

            int[] pesos = new int[] { 71, 67, 59, 53, 47, 43, 41, 37, 29, 23, 19, 17, 13, 7, 3 };

            // Rellenar a 15 dígitos por la izquierda.
            if (limpio.Length < pesos.Length)
            {
                limpio = limpio.PadLeft(pesos.Length, '0');
            }
            else if (limpio.Length > pesos.Length)
            {
                // Si viene más largo, tomamos los últimos 15 dígitos (alineación a la derecha).
                limpio = limpio.Substring(limpio.Length - pesos.Length);
            }

            long suma = 0;
            for (int i = 0; i < pesos.Length; i++)
            {
                suma += (long)(limpio[i] - '0') * pesos[i];
            }

            int resto = (int)(suma % 11);
            return (resto > 1) ? (11 - resto) : resto;
        }

        // Evento al perder foco para el  calcula DV
        private void txtIdentificacion_LostFocus(object sender, RoutedEventArgs e)
        {
            CalcularYMostrarDV();
        }

        // Evento al presionar Enter calcula el  DV
        private void txtIdentificacion_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                CalcularYMostrarDV();
                // opcional: mover foco al siguiente control
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                (sender as UIElement)?.MoveFocus(request);

                //valida si la cedula existe
                ValidaExisteCedula();
            }

        }

        private void CalcularYMostrarDV()
        {
            try
            {
                string id = txtIdentificacion.Text;
                string cleaned = Regex.Replace(id ?? "", @"\D", "");
                if (string.IsNullOrWhiteSpace(cleaned))
                {
                    txtDV.Text = "";
                    return;
                }

                int dv = CalcularDigitoVerificacion(cleaned);
                txtDV.Text = dv.ToString();
            }
            catch
            {
                // Si hay error en el cálculo sólo limpiamos el DV (no interrumpimos al usuario)
                txtDV.Text = "";
            }
        }

        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {
            desbloquearCampos();
            txtIdentificacion.Focus();
            btnCrear.IsEnabled = false;
        }

        private bool Esnumero(string Texto)
        {
            return Texto.All(char.IsDigit);
        }

        private void txtIdentificacion_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Esnumero(e.Text);
        }

        //METODO PARA LEER LA INFO DE LA BASE DE DATOS DE LAS CIUDADES
        private void CargarCiudades()
        {
            CiudadesBD = new List<InfoCiudad>();
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

            using (SqlConnection conexion = new SqlConnection(ConexionSql))
            {
                conexion.Open();
                string ConsultaCiudad = @"select DEP.DepartamentoID, DEP.DepartamentoNombre,CIU.CiudadNombre,CIU.CiudadID
                                        from ConfDepartamento DEP, ConfCiudad CIU
                                        where CIU.DepartamentoID=DEP.DepartamentoID ORDER BY CIU.CiudadNombre";
                using (SqlCommand cmd = new SqlCommand(ConsultaCiudad, conexion))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CiudadesBD.Add(new InfoCiudad
                        {
                            CiudadID = Convert.ToInt32(reader["CiudadID"]),
                            NombreCiudad = reader["CiudadNombre"].ToString(),
                            DepartamentoID = Convert.ToInt32(reader["DepartamentoID"]),
                            NombreDepartamento = reader["DepartamentoNombre"].ToString()
                        });

                    }
                }
            }

        }

        //METODO DE EVENTO TEXTCHANGED
        private void BuscadorCiudad_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CiudadesBD == null || CiudadesBD.Count == 0)
                return;

            string texto = txtbuscadorCiudad.Text?.ToLower() ?? "";

            var filtradas = string.IsNullOrWhiteSpace(texto)
                ? CiudadesBD
                : CiudadesBD.Where(c => c.NombreCiudad.ToLower().Contains(texto)).ToList();

            lstCiudad.ItemsSource = filtradas;
            lstCiudad.DisplayMemberPath = "NombreCiudad"; // Propiedad correcta
            lstCiudad.SelectedValuePath = "CiudadID";
        }

        // METODO DE EVENTO SELECTIONCHANGED - LISTA DE CIUDADES
        private void lstCiudad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstCiudad.SelectedItem is InfoCiudad ciudadSel)
            {
                DepartamentoSeleccionada = ciudadSel.DepartamentoID;
                CiudadSeleccionada = ciudadSel.CiudadID;

                txtbuscadorCiudad.Text = ciudadSel.NombreCiudad; // Nombre real
                txtDepartamento.Text = ciudadSel.NombreDepartamento;

                lstCiudad.ItemsSource = null;
            }
        }


        private void CrearTerceros()
        {
            try
            {
                int ValorCheckCliente = chkCliente.IsChecked == true ? 1 : 0;
                int ValorChechkProveedor = chkProveedor.IsChecked == true ? 1 : 0;
                int ValorCheckEmpleado = chkEmpleado.IsChecked == true ? 1 : 0;
                int TipoDocumento = Convert.ToInt32(cbTipoDocumento.SelectedValue);
                string Identificaion = txtIdentificacion.Text;
                string DV = txtDV.Text;
                string PrimerNombre = txtPrimerNombre.Text;
                string SegundoNombre = txtSegundoNombre.Text;
                string PrimerApellido = txtPrimerApellido.Text;
                string SegundoApellido = txtSegundoApellido.Text;
                String NombreCompleto = PrimerNombre + " " + SegundoNombre + " " + PrimerApellido + " " + SegundoApellido;
                string RazonSocial = txtRazonSocial.Text;
                string Establecimiento = txtEstablecimiento.Text;
                int DepartamentoID = DepartamentoSeleccionada;
                int CiudadID = CiudadSeleccionada;
                string Telefono = txtTelefono.Text;
                string Direccion = txtDireccion.Text;
                string Email = txtEmail.Text;
                int PreciosID = Convert.ToInt32(cbListaPrecios.SelectedValue);

                if (TipoDocumento != 31)
                {
                    if (string.IsNullOrEmpty(Identificaion) ||
                        string.IsNullOrEmpty(PrimerNombre) ||
                        string.IsNullOrEmpty(PrimerApellido) ||
                        string.IsNullOrEmpty(Direccion) ||
                        string.IsNullOrEmpty(Telefono) ||
                        string.IsNullOrEmpty(Email))
                    {
                        MessageBox.Show("Revisa campos Basicos Del Tercero En Blanco", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else if (DepartamentoID == 0 || CiudadID == 0)
                    {
                        MessageBox.Show("Revisa Ciudad Del Tercero En Blanco", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                }
                else if (TipoDocumento == 31)
                {
                    if (string.IsNullOrEmpty(Identificaion) ||
                        string.IsNullOrEmpty(RazonSocial) ||
                        string.IsNullOrEmpty(Establecimiento) ||
                        string.IsNullOrEmpty(Direccion) ||
                        string.IsNullOrEmpty(Telefono) ||
                        string.IsNullOrEmpty(Email))
                    {

                        MessageBox.Show("Revisa campos Basicos Del Tercero En Blanco", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else if (DepartamentoID == 0 || CiudadID == 0)
                    {
                        MessageBox.Show("Revisa Ciudad Del Tercero En Blanco", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                //CONSULTA PARA VALIDAR SI ESTOY MODIFICANDO 

                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Consulta = "SELECT  COUNT(*) FROM InveTerceros WHERE TercerosID=@id";
                    using (SqlCommand cmd = new SqlCommand(Consulta, conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", TercerosID);
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        int NumeroExistencia = Convert.ToInt32(cmd.ExecuteScalar());

                        //valido si exste el terceroid
                        if (NumeroExistencia > 0)
                        {
                            string UpdateCliente = @"update InveTerceros set 
                                                    TercerosTipoDocumento=@tipo,TercerosIdentificacion=@cc,TercerosDV=@dv,
                                                    TercerosPrimerNombre=@pnombre , TercerosSegundoNombre=@snombre,
                                                    TercerosPrimerApellido=@papellido ,TercerosSegundoApellido=@sapellido,
                                                    TercerosNombreCompleto=@nombrecompleto, TerceroRazonSocial=@razonsocial,
                                                    TercerosEstablecimiento=@establecimiento,DepartamentoID=@departamentoid, CiudadID=@ciudadid,
                                                    TercerosDireccion=@direccion,TercerosTelefono=@telefono,TercerosEmail=@email,
                                                    ListaPreciosID=@listaprecios,TerceroCliente=@cliente,TerceroProveedor=@proveedor ,TerceroEmpleado=@empleado
                                                    where TercerosID=@id";
                            using (SqlCommand cmdupdate = new SqlCommand(UpdateCliente, conexion))
                            {
                                cmdupdate.Parameters.AddWithValue("@tipo", TipoDocumento);
                                cmdupdate.Parameters.AddWithValue("@cc", Identificaion);
                                cmdupdate.Parameters.AddWithValue("@dv", DV);
                                cmdupdate.Parameters.AddWithValue("@pnombre", PrimerNombre);
                                cmdupdate.Parameters.AddWithValue("@snombre", SegundoNombre);
                                cmdupdate.Parameters.AddWithValue("@papellido", PrimerApellido);
                                cmdupdate.Parameters.AddWithValue("@sapellido", SegundoApellido);
                                cmdupdate.Parameters.AddWithValue("@nombrecompleto", NombreCompleto);
                                cmdupdate.Parameters.AddWithValue("@razonsocial", RazonSocial);
                                cmdupdate.Parameters.AddWithValue("@establecimiento", Establecimiento);
                                cmdupdate.Parameters.AddWithValue("@departamentoid", DepartamentoID);
                                cmdupdate.Parameters.AddWithValue("@ciudadid", CiudadID);
                                cmdupdate.Parameters.AddWithValue("@direccion", Direccion);
                                cmdupdate.Parameters.AddWithValue("@telefono", Telefono);
                                cmdupdate.Parameters.AddWithValue("@email", Email);
                                cmdupdate.Parameters.AddWithValue("@listaprecios", PreciosID);
                                cmdupdate.Parameters.AddWithValue("@cliente", ValorCheckCliente);
                                cmdupdate.Parameters.AddWithValue("@proveedor", ValorChechkProveedor);
                                cmdupdate.Parameters.AddWithValue("@empleado", ValorCheckEmpleado);
                                cmdupdate.Parameters.AddWithValue("@id", TercerosID);
                                cmdupdate.ExecuteNonQuery();
                                MessageBox.Show("Datos Actualizados Correctamente", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                BloquearCampos();
                            }
                        }
                        // si no lo creo
                        else
                        {
                            string InsertCliente = @" INSERT INTO InveTerceros
                                                    (
                                                    TercerosTipoDocumento,TercerosDV,TercerosIdentificacion,TercerosPrimerNombre,TercerosSegundoNombre,TercerosPrimerApellido,
                                                    TercerosSegundoApellido,TercerosNombreCompleto,TerceroRazonSocial,TercerosEstablecimiento,DepartamentoID,CiudadID,
                                                    TercerosDireccion,TercerosTelefono,TercerosEmail,ListaPreciosID,TerceroCliente,TerceroEmpleado,TerceroProveedor
                                                    ) values
                                                    (@tipodocumento,@dv,@cc,@pnombre,@snombre,@papellido,@sapelldo,@nombrecomp,
                                                    @razonsocial,@establecimiento,@depaid,@ciudadid,@direccion,@telefono,@email,@preciosid,@cliente,@proveedor,@empleado)";
                            using (SqlCommand cmdInsert = new SqlCommand(InsertCliente, conexion))
                            {
                                cmdInsert.Parameters.AddWithValue("@tipodocumento", TipoDocumento);
                                cmdInsert.Parameters.AddWithValue("@cc", Identificaion);
                                cmdInsert.Parameters.AddWithValue("@dv", DV);
                                cmdInsert.Parameters.AddWithValue("@pnombre", PrimerNombre);
                                cmdInsert.Parameters.AddWithValue("@snombre", SegundoNombre);
                                cmdInsert.Parameters.AddWithValue("@papellido", PrimerApellido);
                                cmdInsert.Parameters.AddWithValue("@sapelldo", SegundoApellido);
                                cmdInsert.Parameters.AddWithValue("@nombrecomp", NombreCompleto);
                                cmdInsert.Parameters.AddWithValue("@razonsocial", RazonSocial);
                                cmdInsert.Parameters.AddWithValue("@establecimiento", Establecimiento);
                                cmdInsert.Parameters.AddWithValue("@ciudadid", CiudadID);
                                cmdInsert.Parameters.AddWithValue("@depaid", DepartamentoID);
                                cmdInsert.Parameters.AddWithValue("@direccion", Direccion);
                                cmdInsert.Parameters.AddWithValue("@telefono", Telefono);
                                cmdInsert.Parameters.AddWithValue("@email", Email);
                                cmdInsert.Parameters.AddWithValue("@preciosid", PreciosID);
                                cmdInsert.Parameters.AddWithValue("@cliente", ValorCheckCliente);
                                cmdInsert.Parameters.AddWithValue("@proveedor", ValorChechkProveedor);
                                cmdInsert.Parameters.AddWithValue("@empleado", ValorCheckEmpleado);
                                cmdInsert.ExecuteNonQuery();
                                BloquearCampos();
                            }
                        }

                        btnGuardar.IsEnabled = false;
                    }
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.ToString());
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            CrearTerceros();
        }

        private void cbTipoDocumento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int TipoDocumento = Convert.ToInt32(cbTipoDocumento.SelectedValue);

            if (TipoDocumento == 13)
            {
                txtEstablecimiento.IsEnabled = false;
                txtRazonSocial.IsEnabled = false;
                txtIdentificacion.Focus();
            }
            else if (TipoDocumento == 31)
            {
                txtPrimerNombre.IsEnabled = false;
                txtSegundoNombre.IsEnabled = false;
                txtPrimerApellido.IsEnabled = false;
                txtSegundoApellido.IsEnabled = false;
                txtIdentificacion.Focus();
            }
        }

        private void ValidaExisteCedula()
        {
            try
            {
                string Identificaion = txtIdentificacion.Text;

                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();

                    string Consulta = @"SELECT  COUNT(*) FROM InveTerceros WHERE TercerosIdentificacion=@identificacion";
                    using (SqlCommand cmd = new SqlCommand(Consulta, conexion))
                    {
                        cmd.Parameters.AddWithValue("@identificacion", Identificaion);
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        int CantidadVeces = Convert.ToInt32(cmd.ExecuteScalar());
                        if (CantidadVeces > 0)
                        {
                            MessageBox.Show("La cedula o el nit ya esta registrado", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                            txtIdentificacion.Focus();
                            return;
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message);
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            desbloquearCampos();
            btnCrear.IsEnabled = false;
        }


        private bool EsNumero(string Texto)
        {
            return Texto.All(char.IsDigit);
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}
