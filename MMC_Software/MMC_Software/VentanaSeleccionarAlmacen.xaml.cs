using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaSeleccionarAlmacen.xaml
    /// </summary>
    public partial class VentanaSeleccionarAlmacen : Window
    {
        string _ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
        private int _ArticuloID;
        public VentanaSeleccionarAlmacen(int ArticuloID)
        {
            InitializeComponent();
            _ArticuloID = ArticuloID;
            LlenarAlmacen();
        }

        private void LlenarAlmacen()
        {
            var RAlmacen = new AlmacenRepositorio(_ConexionSql);
            DataTable dt = RAlmacen.DevolverAlmacenes(2, _ArticuloID);

            if (dt.Rows.Count == 0)
                return;

            // Convertimos el DataTable a una lista de ListadoAlmacen
            List<ListadoAlmacen> listaAlmacenes = new List<ListadoAlmacen>();

            foreach (DataRow row in dt.Rows)
            {
                listaAlmacenes.Add(new ListadoAlmacen
                {
                    AlmacenID = Convert.ToInt32(row["IdAlmacen"]),
                    CodigoAlmacen = row["CodigoAlmacen"].ToString(),
                    DatoAlmacen = row["Almacen"].ToString(),
                    SaldoActual = row.Table.Columns.Contains("SaldoActual") ? Convert.ToDecimal(row["SaldoActual"]) : 0
                });
            }

            // 🔹 Aquí es donde enlazas los datos al ListBox
            LstAlmacen.ItemsSource = listaAlmacenes;
        }


        public class ListadoAlmacen
        {
            public int AlmacenID { get; set; }
            public string CodigoAlmacen { get; set; }
            public string DatoAlmacen { get; set; }
            public decimal SaldoActual { get; set; }
        }
        public ListadoAlmacen AlmacenSeleccionado { get; set; }

        private void LstAlmacen_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && LstAlmacen.SelectedItem != null)
            {
                ConfirmarSeleccion();
                e.Handled = true;
            }
        }

        private void LstAlmacen_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ConfirmarSeleccion();
        }

        private void ConfirmarSeleccion()
        {
            var drv = LstAlmacen.SelectedItem as ListadoAlmacen;
            if(drv!= null)
            {
                AlmacenSeleccionado = new ListadoAlmacen
                {
                    AlmacenID = drv.AlmacenID,
                    CodigoAlmacen = drv.CodigoAlmacen,
                    DatoAlmacen = drv.DatoAlmacen,
                    SaldoActual = drv.SaldoActual
                };
                DialogResult = true;
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(LstAlmacen.Items.Count>0)
            {
                LstAlmacen.SelectedIndex = 0;
                LstAlmacen.Focus();
            }
        }
    }
}
