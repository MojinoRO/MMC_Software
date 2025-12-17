using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Windows.Forms;
using System;
using System.Data.SqlClient;
using System.Windows;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaImpresionDocumentos.xaml
    /// </summary>
    public partial class VentanaImpresionDocumentos : Window
    {
        private string _Conexion = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
        private int _CierreID;
        public VentanaImpresionDocumentos(int CiereID)
        {
            InitializeComponent();
            _CierreID = CiereID;
            CargarReporte();
        }

        private void CargarReporte()
        {
            DS_CierreCaja DS = new DS_CierreCaja();

            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string ConsultaCierre = @"Select * from ConfCierreCaja where CierreID=@cierreid";
                using (SqlCommand cmd = new SqlCommand(ConsultaCierre, conn))
                {
                    cmd.Parameters.AddWithValue("@cierreid", _CierreID);
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    {
                        adp.Fill(DS, "CierreCaja");
                    }
                }
                string ConsultaEmpresa = @"select EmpresaNombre as NombreEmpresa ,EmpresaNit as Nit,EmpresaDv as Dv,
                                            EmpresaDireccion as Direccion,EmpresaTelefono as Telefono from ConfEmpresa";
                using (SqlDataAdapter adp = new SqlDataAdapter(ConsultaEmpresa, conn))
                {
                    adp.Fill(DS, "Empresa");
                }

                string ConsultaFcturas = @"select * from ConfCierreFacturas where CierreID=@cierreid";
                using (SqlCommand cmd = new SqlCommand(ConsultaFcturas, conn))
                {
                    cmd.Parameters.AddWithValue("@cierreid", _CierreID);
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    {
                        adp.Fill(DS, "Facturas");
                    }
                }

                string ConsultaFormasPago = @"select * from ConfCierreFormaPagos where CierreID=@cierreid";
                using (SqlCommand cmd = new SqlCommand(ConsultaFormasPago, conn))
                {
                    cmd.Parameters.AddWithValue("@cierreid", _CierreID);
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    {
                        adp.Fill(DS, "FormasPago");
                    }
                }

                string ConsultaProductos = @"select * from ConfCierreProductos where CierreID=@cierreid";
                using (SqlCommand cmd = new SqlCommand(ConsultaProductos, conn))
                {
                    cmd.Parameters.AddWithValue("@cierreid", _CierreID);
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    {
                        adp.Fill(DS, "Productos");
                    }
                }

                string ConsultaImpuestos = @"select * from ConfCierreImpuestos where CierreID=@cierreid";
                using (SqlCommand cmd = new SqlCommand(ConsultaImpuestos, conn))
                {
                    cmd.Parameters.AddWithValue("@cierreid", _CierreID);
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    {
                        adp.Fill(DS, "Impuestos");
                    }
                }
            }

            string RutaReporte = ConfiguracionConexion.ObtenerRutadeReportes("RepImpCierreCaja.rpt");
            ReportDocument Reporte = new ReportDocument();
            Reporte.Load(RutaReporte);
            Reporte.SetDataSource(DS);

            CrystalReportViewer visor = new CrystalReportViewer
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ReportSource = Reporte,
                ToolPanelView = ToolPanelViewType.None,
                DisplayStatusBar = false,
                BorderStyle = System.Windows.Forms.BorderStyle.None
            };

            //  Asignar el visor al WindowsFormsHost del XAML
            VenatanaImpresion.Child = visor;
        }
    }
}
