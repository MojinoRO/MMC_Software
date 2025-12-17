using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaCrearArticulos.xaml
    /// </summary>
    public partial class VentanaCrearArticulos : Window
    {
        private int ArticulosID;
        private int TarifaIva;
        private int Margen;
        private int Incremeto;
        private decimal CostoConIva;
        private decimal VentaPublico;

        public VentanaCrearArticulos(int ArticulosID)
        {
            InitializeComponent();
            this.ArticulosID = ArticulosID;
            mostrarDatos();
            bloquearCampos();
        }

        public VentanaCrearArticulos()
        {
            InitializeComponent();
            MuestraCategorias();
            MuestraMarcas();
            MuestraSubCategorias();
            bloquearCampos();

        }

        private void desbloquearCampos()
        {
            txtCodigo.IsEnabled = true;
            txtNombre.IsEnabled = true;
            txtCostoConIva.IsEnabled = true;
            txtCostoSinIva.IsEnabled = true;
            txtMargen.IsEnabled = true;
            txtPrecioVenta.IsEnabled = true;
            TxtBarras.IsEnabled = true;
            txtReferencia.IsEnabled = true;
            cbMarca.IsEnabled = true;
            cbSubCategoria.IsEnabled = true;
            cbCategoria.IsEnabled = true;
            txtIncremento.IsEnabled = true;
        }

        private void bloquearCampos()
        {
            txtCodigo.IsEnabled = false;
            txtNombre.IsEnabled = false;
            txtCostoConIva.IsEnabled = false;
            txtCostoSinIva.IsEnabled = false;
            txtMargen.IsEnabled = false;
            txtPrecioVenta.IsEnabled = false;
            TxtBarras.IsEnabled = false;
            txtReferencia.IsEnabled = false;
            cbMarca.IsEnabled = false;
            cbSubCategoria.IsEnabled = false;
            cbCategoria.IsEnabled = false;
            txtTarifaIva.IsEnabled = false;
            txtIncremento.IsEnabled = false;
        }

        private void MuestraCategorias()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string ConsultaCategorias = "select CONCAT(CodigoCategoria,'  ',NombreCategoria) as InfoCompleta ,CategoriasID from ConfCategoriasInve";
                    SqlDataAdapter adp = new SqlDataAdapter(ConsultaCategorias, conexion);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    cbCategoria.DisplayMemberPath = "InfoCompleta";
                    cbCategoria.SelectedValuePath = "CategoriasID";
                    cbCategoria.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message);
            }
        }

        private void MuestraSubCategorias()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string ConsultaSubcentro = @"select CONCAT(CodigoSubCategoria,'  ',NombreSubCategoria) as InfoCompleta ,SubCategoriaID from ConfSubCategorias
"; SqlDataAdapter adp = new SqlDataAdapter(ConsultaSubcentro, conexion);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    cbSubCategoria.DisplayMemberPath = "InfoCompleta";
                    cbSubCategoria.SelectedValuePath = "SubCategoriaID";
                    cbSubCategoria.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message);
            }
        }

        private void MuestraMarcas()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string consultaMarca = "select CONCAT(CodigoMarca,'  ',NombreMarca) as InfoCompleta ,MarcasID from ConfMarcas";
                    SqlDataAdapter adapter = new SqlDataAdapter(consultaMarca, conexion);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    cbMarca.DisplayMemberPath = "InfoCompleta";
                    cbMarca.SelectedValuePath = "MarcasID";
                    cbMarca.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message);
            }
        }

        private void mostrarDatos()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Consulta = @"select AR.ArticulosID,AR.CodigoArticulo,AR.NombreArticulo,AR.CostoArticuloMasIva,AR.ArticulosVenta,
                                        AR.CostoArticuloSinIva,AR.ArticulosIncremento,AR.ArticulosMargen,CAT.NombreCategoria,CAT.TarifaImpuesto,SCAT.NombreSubCategoria,
                                        AR.ArticulosReferencias,MAR.NombreMarca,AR.ArticulosBarras,CAT.CategoriasID,SCAT.SubCategoriaID,MAR.MarcasID,AR.ArticulosUtilidad,
										AR.ArticulosVentaMinima
                                        from InveArticulos AR, ConfCategoriasInve CAT, ConfSubCategorias SCAT,ConfMarcas MAR
                                        WHERE AR.CategoriasID=CAT.CategoriasID AND AR.SubCategoriaID=SCAT.SubCategoriaID
                                        AND AR.MarcasID=MAR.MarcasID and AR.ArticulosId=@id";
                    SqlCommand cmd = new SqlCommand(Consulta, conexion);
                    cmd.Parameters.AddWithValue("@id", ArticulosID);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    if (dataTable.Rows.Count > 0)
                    {
                        txtCodigo.Text = dataTable.Rows[0]["CodigoArticulo"].ToString();
                        txtNombre.Text = dataTable.Rows[0]["NombreArticulo"].ToString();
                        txtCostoConIva.Text = dataTable.Rows[0]["CostoArticuloMasIva"].ToString();
                        txtCostoSinIva.Text = dataTable.Rows[0]["CostoArticuloSinIva"].ToString();
                        txtPrecioVenta.Text = dataTable.Rows[0]["ArticulosVenta"].ToString();
                        txtIncremento.Text = dataTable.Rows[0]["ArticulosIncremento"].ToString();
                        txtMargen.Text = dataTable.Rows[0]["ArticulosMargen"].ToString();
                        TxtBarras.Text = dataTable.Rows[0]["ArticulosBarras"].ToString();
                        txtReferencia.Text = dataTable.Rows[0]["ArticulosReferencias"].ToString();
                        int CategoriaID = Convert.ToInt32(dataTable.Rows[0]["CategoriasID"]);
                        int SubCategoria = Convert.ToInt32(dataTable.Rows[0]["SubCategoriaID"]);
                        int marcaId = Convert.ToInt32(dataTable.Rows[0]["MarcasID"]);
                        int PorcentajeIVA = Convert.ToInt32(dataTable.Rows[0]["TarifaImpuesto"]);
                        txtPrecioMinimo.Text = dataTable.Rows[0]["ArticulosVentaMinima"].ToString();
                        txtUtilidad.Text = dataTable.Rows[0]["ArticulosUtilidad"].ToString();
                        txtTarifaIva.Text = PorcentajeIVA.ToString();
                        MuestraSubCategorias();
                        MuestraMarcas();
                        MuestraCategorias();
                        cbCategoria.SelectedValue = CategoriaID;
                        cbSubCategoria.SelectedValue = SubCategoria;
                        cbMarca.SelectedValue = marcaId;
                    }
                    conexion.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message);
            }
        }

        private void CreaActualiza()
        {
            try
            {
                string codigo = txtCodigo.Text;
                string nombre = txtNombre.Text;
                string costomasiva = txtCostoConIva.Text;
                string costosiniva = txtCostoSinIva.Text;
                string Venta = txtPrecioVenta.Text;
                string referencia = txtReferencia.Text;
                string margen = txtMargen.Text;
                string Incremento = txtIncremento.Text;
                string Barras = TxtBarras.Text;
                string Categoria = cbCategoria.SelectedValue.ToString();
                string Subcategoria = cbSubCategoria.SelectedValue.ToString();
                string Marca = cbMarca.SelectedValue.ToString();
                string Utilidad = txtUtilidad.Text;
                string VentaMinima = txtPrecioMinimo.Text;

                if (string.IsNullOrEmpty(codigo) ||
                    string.IsNullOrEmpty(nombre) ||
                    string.IsNullOrEmpty(costomasiva) ||
                    string.IsNullOrEmpty(costosiniva) ||
                    string.IsNullOrEmpty(Venta) ||
                    string.IsNullOrEmpty(referencia) ||
                    string.IsNullOrEmpty(Barras) ||
                    cbCategoria.SelectedValue == null ||
                    cbSubCategoria.SelectedValue == null ||
                    cbMarca.SelectedValue == null)
                {
                    MessageBox.Show("Revisa Campos En blanco revisa formulario", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string ConsultaExiste = "Select COUNT(*) from InveArticulos where CodigoArticulo=@codigo";
                    SqlCommand cmd = new SqlCommand(ConsultaExiste, conexion);
                    cmd.Parameters.AddWithValue("@codigo", codigo);
                    int Conteo = Convert.ToInt32(cmd.ExecuteScalar());
                    if (Conteo > 0)
                    {
                        string UpdateArticulo = @"update InveArticulos set 
                                                CodigoArticulo=@codigo,
                                                NombreArticulo=@nombre,
                                                CostoArticuloSinIva=@costossiniva,
                                                CostoArticuloMasIva=@costomasiva,
                                                ArticulosVenta=@venta,
                                                ArticulosMargen=@margen,
                                                ArticulosIncremento=@incremento,
                                                CategoriasID=@categoria,
                                                SubCategoriaID=@subcategoria,
                                                MarcasID=@marcasid,
                                                ArticulosBarras=@barras,
                                                ArticulosReferencias=@Referencias,
                                                ArticulosUtilidad=@utilidad,
                                                ArticulosVentaMinima=@ventaminima
                                                where ArticulosID=@idarticulo";
                        SqlCommand cmdupdate = new SqlCommand(UpdateArticulo, conexion);
                        cmdupdate.Parameters.AddWithValue("@codigo", codigo);
                        cmdupdate.Parameters.AddWithValue("@nombre", nombre);
                        cmdupdate.Parameters.AddWithValue("@costossiniva", decimal.Parse(costosiniva));
                        cmdupdate.Parameters.AddWithValue("@costomasiva", decimal.Parse(costomasiva));
                        cmdupdate.Parameters.AddWithValue("@venta", decimal.Parse(Venta));
                        cmdupdate.Parameters.AddWithValue("@margen", string.IsNullOrWhiteSpace(margen) ? (object)DBNull.Value : decimal.Parse(margen));
                        cmdupdate.Parameters.AddWithValue("@incremento", string.IsNullOrWhiteSpace(Incremento) ? (object)DBNull.Value : decimal.Parse(Incremento));
                        cmdupdate.Parameters.AddWithValue("@categoria", Convert.ToInt32(cbCategoria.SelectedValue));
                        cmdupdate.Parameters.AddWithValue("@subcategoria", Convert.ToInt32(cbSubCategoria.SelectedValue));
                        cmdupdate.Parameters.AddWithValue("@marcasid", Convert.ToInt32(cbMarca.SelectedValue));
                        cmdupdate.Parameters.AddWithValue("@barras", Barras);
                        cmdupdate.Parameters.AddWithValue("@Referencias", referencia);
                        cmdupdate.Parameters.AddWithValue("@idarticulo", ArticulosID);
                        cmdupdate.Parameters.AddWithValue("@utilidad", decimal.Parse(Utilidad));
                        cmdupdate.Parameters.AddWithValue("@ventaminima", decimal.Parse(VentaMinima));
                        cmdupdate.ExecuteNonQuery();
                        bloquearCampos();
                    }
                    else
                    {
                        String insertArticulos = @"INSERT INTO InveArticulos
                        (
                        CodigoArticulo,
                        NombreArticulo,
                        CostoArticuloSinIva,
                        CostoArticuloMasIva,
                        ArticulosVenta,
                        ArticulosMargen,
                        ArticulosIncremento,
                        CategoriasID,
                        SubCategoriaID,
                        MarcasID,
                        ArticulosBarras,
                        ArticulosReferencias,
                        ArticulosUtilidad,
                        ArticulosVentaMinima
                        ) VALUES
                        (
                        @codigo,
                        @nombre,
                        @costossiniva,
                        @costomasiva,
                        @venta,
                        @margen,
                        @incremento,
                        @categoria,
                        @subcategoria,
                        @marcasid,
                        @barras,
                        @Referencias,
                        @Utilidad,
                        @VentaMinima)";
                        SqlCommand cmdInsert = new SqlCommand(insertArticulos, conexion);
                        cmdInsert.Parameters.AddWithValue("@codigo", codigo);
                        cmdInsert.Parameters.AddWithValue("@nombre", nombre);
                        cmdInsert.Parameters.AddWithValue("@costossiniva", decimal.Parse(costosiniva));
                        cmdInsert.Parameters.AddWithValue("@costomasiva", decimal.Parse(costomasiva));
                        cmdInsert.Parameters.AddWithValue("@venta", decimal.Parse(Venta));
                        cmdInsert.Parameters.AddWithValue("@margen", string.IsNullOrWhiteSpace(margen) ? (object)DBNull.Value : decimal.Parse(margen));
                        cmdInsert.Parameters.AddWithValue("@incremento", string.IsNullOrWhiteSpace(Incremento) ? (object)DBNull.Value : decimal.Parse(Incremento));
                        cmdInsert.Parameters.AddWithValue("@categoria", Convert.ToInt32(cbCategoria.SelectedValue));
                        cmdInsert.Parameters.AddWithValue("@subcategoria", Convert.ToInt32(cbSubCategoria.SelectedValue));
                        cmdInsert.Parameters.AddWithValue("@marcasid", Convert.ToInt32(cbMarca.SelectedValue));
                        cmdInsert.Parameters.AddWithValue("@barras", Barras);
                        cmdInsert.Parameters.AddWithValue("@Referencias", referencia);
                        cmdInsert.Parameters.AddWithValue("@Utilidad", decimal.Parse(Utilidad));
                        cmdInsert.Parameters.AddWithValue("@VentaMinima", decimal.Parse(VentaMinima));
                        cmdInsert.ExecuteNonQuery();
                        bloquearCampos();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.ToString());
            }
        }

        private void EliminarArticulos()
        {
            try
            {

                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string DeleteArticulos = "delete from InveArticulos where ArticulosID=@id";
                    using (SqlCommand cmd = new SqlCommand(DeleteArticulos, conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", ArticulosID);
                        cmd.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message);
            }
        }
        private void GenerarCodigoArticulo()
        {
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

            using (SqlConnection conexion = new SqlConnection(ConexionSql))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("ObtenerCodigoArticulo", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                string CodigoNuevo = cmd.ExecuteScalar()?.ToString();
                txtCodigo.Text = CodigoNuevo;
            }
        }

        private void TraerTarfifaIva()
        {
            int Idcategoria = Convert.ToInt32(cbCategoria.SelectedValue);

            if (cbCategoria.SelectedValue != null)
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();
                    string Consulta = "select TarifaImpuesto from ConfCategoriasInve where CategoriasID=@id";
                    using (SqlCommand cmd = new SqlCommand(Consulta, conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", Idcategoria);
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            txtTarifaIva.Text = dt.Rows[0]["TarifaImpuesto"].ToString();
                            TarifaIva = Convert.ToInt32(dt.Rows[0]["TarifaImpuesto"]);
                        }
                    }
                }
            }
        }

        private decimal CalcularCostoMasIva()
        {
            if (decimal.TryParse(txtCostoSinIva.Text, out decimal CostoSinIva) && TarifaIva > 0)
            {
                decimal PorcentajeIva = TarifaIva / 100m;

                decimal CostoMasIva = CostoSinIva * (1 + PorcentajeIva);

                return CostoMasIva;
            }
            else
            {
                return 0;
            }
        }

        private decimal CalculaPrecioMInimo()
        {
            try
            {
                Margen = Convert.ToInt32(txtMargen.Text);

                if (decimal.TryParse(txtCostoConIva.Text, out decimal CostoMasIva) && Margen > 0)
                {
                    decimal MargenIncremento = Margen / 100m;
                    decimal PrecioMinimo = CostoMasIva * (1 + MargenIncremento);
                    return PrecioMinimo;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR ASIGNA MARGEN AL PRODUCTO" + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                return 0;

            }
        }

        private decimal CalcularVentaPublico()
        {
            try
            {
                Incremeto = Convert.ToInt32(txtIncremento.Text);

                if (decimal.TryParse(txtCostoConIva.Text, out decimal CostoMasIva) && Incremeto > 0)
                {
                    decimal IncrementoVenta = Incremeto / 100m;
                    decimal VentaPublico = CostoMasIva * (1 + IncrementoVenta);
                    return VentaPublico;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR ASIGNA INCREMENTO AL PRODUCTO " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                return 0;
            }

        }


        private decimal CalcularUtilidad()
        {
            try
            {
                CostoConIva = Convert.ToDecimal(txtCostoConIva.Text);
                VentaPublico = Convert.ToDecimal(txtPrecioVenta.Text);
                if (CostoConIva > 0 && VentaPublico > 0)
                {
                    decimal Utilidad = VentaPublico - CostoConIva;
                    return Utilidad;

                }
                else
                {
                    return 0;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR REVISA MARGEN E INCREMENTO DIGITADO" + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                return 0;
            }

        }
        //--------------------------------------------BOTONES-----------------------------------------------//

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            desbloquearCampos();
            txtCodigo.IsEnabled = false;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            CreaActualiza();
        }

        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {
            GenerarCodigoArticulo();
            desbloquearCampos();
        }



        private bool Esnumero(string texto)
        {
            return texto.All(char.IsDigit);
        }

        private void txtMargen_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Esnumero(e.Text);
        }

        private void cbCategoria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TraerTarfifaIva();
        }

        private void txtCostoSinIva_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtCostoConIva.Text = CalcularCostoMasIva().ToString("N2");
            }
        }

        private void txtMargen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtPrecioMinimo.Text = CalculaPrecioMInimo().ToString("N2");
            }
        }

        private void txtIncremento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtPrecioVenta.Text = CalcularVentaPublico().ToString("N2");
                txtUtilidad.Text = CalcularUtilidad().ToString("N2");
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (ArticulosID != 0)
                {
                    EliminarArticulos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message);
            }
        }

        private void BtnBarras_Click(object sender, RoutedEventArgs e)
        {
            string Nombre = txtNombre.Text;
            VentanaCodigosBarraArticulos Ventana = new VentanaCodigosBarraArticulos(ArticulosID, Nombre);
            Ventana.Owner = this;
            Ventana.ShowDialog();
        }
    }
}
