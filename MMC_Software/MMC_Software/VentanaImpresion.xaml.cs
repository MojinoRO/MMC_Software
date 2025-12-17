using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaImpresion.xaml
    /// </summary>
    public partial class VentanaImpresion : Window
    {
        private int VentaID;
        private ReportDocument reporte;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer viewer;

        public VentanaImpresion(int ventaID)
        {
            InitializeComponent();
            VentaID = ventaID;
            LLenarFormulasTotales();
            CargarReporte();
        }

        public class FormulasTotales
        {
            public string VendedorFac { get; set; }
            public decimal Gravado19 { get; set; }
            public decimal Gravado5 { get; set; }
            public decimal Iva19 { get; set; }
            public decimal Iva5 { get; set; }
            public decimal Exento { get; set; }
            public decimal RteFuente { get; set; }
            public decimal Descuentos { get; set; }
            public decimal TotalVenta { get; set; }
            public decimal Entregado { get; set; }
            public decimal Cambio { get; set; }
            public string FormaPafgo { get; set; }
        }

        public List<FormulasTotales> DatosVenta = new List<FormulasTotales>();

        private void LLenarFormulasTotales()
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                using (SqlConnection conexion = new SqlConnection(ConexionSql))
                {
                    conexion.Open();

                    string consulta = @"select VENT.Gravado19, VENT.Gravado5,VENT.Iva19,VENT.Iva5,VENT.Exento,VENT.RteFuente,
                                        VENT.Descuentos,VENT.TotalVenta,VENT.Entregado,VENT.Cambio,
                                        CONCAT(VENDEDOR.CodigoVendedor,' ',VENDEDOR.NombreVendedor) AS Vendedor
                                        from InveVentas VEN,InveVentasTotales VENT,ConfVendedores VENDEDOR
                                        where 
	                                        VEN.VentaID= VENT.VentaID  AND VEN.VendedorID=VENDEDOR.VendedorID AND VEN.VentaID=@VentaID";
                    using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                    {
                        cmd.Parameters.AddWithValue("@VentaID", VentaID);
                        using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adp.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                FormulasTotales Datos = new FormulasTotales()
                                {
                                    VendedorFac = dt.Rows[0]["Vendedor"].ToString(),
                                    Gravado19 = Convert.ToDecimal(dt.Rows[0]["Gravado19"]),
                                    Gravado5 = Convert.ToDecimal(dt.Rows[0]["Gravado5"]),
                                    Iva19 = Convert.ToDecimal(dt.Rows[0]["Iva19"]),
                                    Iva5 = Convert.ToDecimal(dt.Rows[0]["Iva5"]),
                                    Exento = Convert.ToDecimal(dt.Rows[0]["Exento"]),
                                    RteFuente = Convert.ToDecimal(dt.Rows[0]["RteFuente"]),
                                    Descuentos = Convert.ToDecimal(dt.Rows[0]["Descuentos"]),
                                    TotalVenta = Convert.ToDecimal(dt.Rows[0]["TotalVenta"]),
                                    Entregado = Convert.ToDecimal(dt.Rows[0]["Entregado"]),
                                    Cambio = Convert.ToDecimal(dt.Rows[0]["Cambio"])
                                };

                                DatosVenta.Clear();
                                DatosVenta.Add(Datos);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.ToString(), "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LllenarFormaPago()
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void CargarReporte()
        {
            string cadenaConexion = ConfiguracionConexion.ObtenerCadenaConexion(
                                        ConexionEmpresaActual.BaseDeDatosSeleccionada);

            reporte = new ReportDocument();
            string ruta = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reportes", "Inventarios", "RepImpTQ.rpt");
            reporte.Load(ruta);

            // Aplicar conexión (normaliza DB en tiempo de ejecución)
            CrystalHelper.AplicarConexion(reporte, cadenaConexion);

            // Pasar el parámetro
            reporte.SetParameterValue("VentaID", VentaID);


            // Campo fórmula
            reporte.DataDefinition.FormulaFields["Vendedor"].Text = "'" + DatosVenta[0].VendedorFac + "'";
            reporte.DataDefinition.FormulaFields["Gravado19"].Text = DatosVenta[0].Gravado19.ToString(System.Globalization.CultureInfo.InvariantCulture);
            reporte.DataDefinition.FormulaFields["Gravado5"].Text = DatosVenta[0].Gravado5.ToString(System.Globalization.CultureInfo.InvariantCulture);
            reporte.DataDefinition.FormulaFields["Iva19"].Text = DatosVenta[0].Iva19.ToString(System.Globalization.CultureInfo.InvariantCulture);
            reporte.DataDefinition.FormulaFields["Iva5"].Text = DatosVenta[0].Iva5.ToString(System.Globalization.CultureInfo.InvariantCulture);
            reporte.DataDefinition.FormulaFields["Exento"].Text = DatosVenta[0].Exento.ToString(System.Globalization.CultureInfo.InvariantCulture);
            reporte.DataDefinition.FormulaFields["ReteFuente"].Text = DatosVenta[0].RteFuente.ToString(System.Globalization.CultureInfo.InvariantCulture);
            reporte.DataDefinition.FormulaFields["Descuento"].Text = DatosVenta[0].Descuentos.ToString(System.Globalization.CultureInfo.InvariantCulture);
            reporte.DataDefinition.FormulaFields["TotalVenta"].Text = DatosVenta[0].TotalVenta.ToString(System.Globalization.CultureInfo.InvariantCulture);
            reporte.DataDefinition.FormulaFields["Recibi"].Text = DatosVenta[0].Entregado.ToString(System.Globalization.CultureInfo.InvariantCulture);
            reporte.DataDefinition.FormulaFields["Cambio"].Text = DatosVenta[0].Cambio.ToString(System.Globalization.CultureInfo.InvariantCulture);

            // Crear viewer WinForms dentro del host WPF
            viewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            viewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            viewer.Dock = System.Windows.Forms.DockStyle.Fill;

            winFormsHost.Child = viewer; // inserta en el WindowsFormsHost definido en XAML

            // Asignar fuente y mostrar
            viewer.ReportSource = reporte;
            viewer.Refresh();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            try
            {
                if (viewer != null)
                {
                    viewer.ReportSource = null;
                    winFormsHost.Child = null; // Libera el control del host WPF
                    viewer.Dispose();
                    viewer = null;
                }

                if (reporte != null)
                {
                    reporte.Close();
                    reporte.Dispose();
                    reporte = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error liberando recursos: " + ex.Message,
                                "Crystal Reports", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }

}

