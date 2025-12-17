using CrystalDecisions.CrystalReports;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.ReportAppServer;
using CrystalDecisions.ReportSource;
using CrystalDecisions.Shared;
using CrystalDecisions.Windows.Forms;
using SAPBusinessObjects.WPF.Viewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para ImpresionDocumentos.xaml
    /// </summary>
    public partial class ImpresionDocumentos : Window
    {
        /// <summary>
        /// EL Tipo de reporte le voy a pasar por parametros que se va a imprimir, para saber que metodo va usar
        /// 1=compras
        /// 2=ventas
        /// 3=dv compras  etc
        /// </summary>
        

        private int _TipoReporte;
        private int _MovimientoID;

        public ImpresionDocumentos(int TipoReporte, int MovimientoID)
        {
            InitializeComponent();
            _TipoReporte = TipoReporte;
            _MovimientoID=MovimientoID;
            Loaded += (s, e) => CargarReporte();

        }

        private void CargarReporte()
        {
            try
            {
             switch (_TipoReporte)
                {
                    case 1:
                        //Obtener Cadena base de datos
                        string _ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

                        // CARGAR reporte 

                        ReportDocument Rpt = new ReportDocument();
                        Rpt.Load(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reportes", "Inventarios", "RepComprobanteCompras.rpt"));

                        //aplicamos conexion real al rpt 

                        CrystalHelper.AplicarConexion(Rpt, _ConexionSql);

                        int pCompraID = _MovimientoID;
                        Rpt.SetParameterValue("CompraID", pCompraID);
                        //mostrar en el visor 

                        // 5️⃣ Mostrarlo en el visor Crystal Reports de WPF
                        CrystalReportViewer visor = new CrystalReportViewer
                        {
                            Dock = System.Windows.Forms.DockStyle.Fill,
                            ReportSource = Rpt,
                            ToolPanelView = ToolPanelViewType.None,
                            DisplayStatusBar = false,
                            BorderStyle = System.Windows.Forms.BorderStyle.None
                        };

                        //  Asignar el visor al WindowsFormsHost del XAML
                        winFormsHost.Child= visor;
                        break;

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error al cargar el reporte: " + ex.ToString());
            }
        }
    }
}
