using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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
    /// Lógica de interacción para VentanaSeleccionarTarfia.xaml
    /// </summary>
    public partial class VentanaSeleccionarTarifa : Window
    {
        private string _ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
        private int _ArticuloID;
        public VentanaSeleccionarTarifa(int ArticuloID)
        {
            InitializeComponent();
            _ArticuloID = ArticuloID;
        }


        public class ListadoPrecios
        {
            public int TarifaID { get; set; }
            public string Descripcion { get; set; }
            public decimal Precio { get; set; }

        }

        public ListadoPrecios PrecioSeleccionado { get; set; }

        }
}
