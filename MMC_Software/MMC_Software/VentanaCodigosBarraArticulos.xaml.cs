using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaCodigosBarraArticulos.xaml
    /// </summary>
    public partial class VentanaCodigosBarraArticulos : Window
    {
        private int _Articuloid;
        private string _Nombre;
        private string _ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
        public static ObservableCollection<CodigosBarras> barras { get; set; }
        public VentanaCodigosBarraArticulos(int ArticuloID, string Nombre)
        {
            InitializeComponent();
            _Articuloid = ArticuloID;
            _Nombre = Nombre;
            TxtNombreArticulo.Text = _Nombre;
            barras = new ObservableCollection<CodigosBarras>();
            dgBarras.ItemsSource = barras;
            CargarDatos();
        }

        public void CargarDatos()
        {
            if (barras != null)
            {
                var RBarras = new RepositorioBarras(_ConexionSql);
                var ListaBD = RBarras.ListadoCodigosBD(_Articuloid);
                dgBarras.ItemsSource = barras;
            }
        }
        public class CodigosBarras
        {
            public string CodigoBarras { get; set; }
            public string NombreBarras { get; set; }
            public decimal Cantidad { get; set; }
            public bool Yaguardado { get; set; }
        }

        private void SoloNumeros_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex SoloNumeros = new Regex("[^0-9]+");
            e.Handled = SoloNumeros.IsMatch(e.Text);
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dgBarras_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true; // Evita que Enter cierre la edición
                DataGrid dataGrid = (DataGrid)sender;
                var cellInfo = dataGrid.CurrentCell;

                // Posición actual
                int currentColumn = dataGrid.Columns.IndexOf(cellInfo.Column);
                int currentRow = dataGrid.Items.IndexOf(cellInfo.Item);

                // Determinar siguiente posición
                int nextColumn = currentColumn + 1;
                int nextRow = currentRow;

                // Si está en la última columna → siguiente fila, primera columna
                if (nextColumn >= dataGrid.Columns.Count)
                {
                    nextColumn = 0;
                    nextRow++;
                }

                // Si no existe esa fila (es la última), crea una nueva (si quieres)
                if (nextRow >= dataGrid.Items.Count)
                {
                    // Solo si se permite agregar filas
                    if (dataGrid.CanUserAddRows)
                    {
                        dataGrid.CommitEdit();
                        dataGrid.Items.Refresh();
                    }
                    nextRow = dataGrid.Items.Count - 1;
                }

                // Mover selección
                if (nextRow >= 0 && nextRow < dataGrid.Items.Count)
                {
                    dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[nextRow], dataGrid.Columns[nextColumn]);
                    dataGrid.BeginEdit();
                }
            }
        }


        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // aqui le hago un cast al observable collection
                var BarrasNuevas = barras.Where(b => !string.IsNullOrWhiteSpace(b.CodigoBarras) && b.Yaguardado == false).ToList();
                var Barras = new RepositorioBarras(_ConexionSql);

                if (BarrasNuevas.Count == 0)
                {
                    MessageBox.Show("No hay códigos de barras válidos para guardar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                bool Crear = Barras.insertBarras(_Articuloid, BarrasNuevas);
                if (Crear == true)
                {
                    foreach (var B in BarrasNuevas)
                    {
                        B.Yaguardado = true;
                    }
                    MessageBox.Show("Barras Guardadas Correctamente", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Ocurrió un error al guardar las barras.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.Message);
            }
        }

        private void dgBarras_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                CodigosBarras Barras = e.Row.Item as CodigosBarras;
                if (Barras != null)
                {
                    if (string.IsNullOrEmpty(Barras.CodigoBarras) &&
                        string.IsNullOrEmpty(Barras.NombreBarras) &&
                        Barras.Cantidad == 0)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            barras.Remove(Barras);

                        }), System.Windows.Threading.DispatcherPriority.Background);
                    }
                }
            }
        }

        private void dgBarras_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                //aqui valido segun el nombre del tiulo de la columna que quiero validar

                if (e.Column.Header.ToString() == "Codigo")
                {
                    //recolecto el dato de la textbox en una variable
                    TextBox Codigo = e.EditingElement as TextBox;
                    if (Codigo != null)
                    {
                        if (string.IsNullOrEmpty(Codigo.Text))
                        {
                            MessageBox.Show("El codigo de barras esta vacio", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
                            e.Cancel = true;
                            return;
                        }
                        var barrasBD = new RepositorioBarras(_ConexionSql);
                        bool Existe = barrasBD.ValidaExisteCodigoBarras(Codigo.Text);
                        if (Existe == true)
                        {

                            string Nombre;
                            DataTable dt = barrasBD.TraerNombreExisteBarras(Codigo.Text);
                            if (dt.Rows.Count > 0)
                            {
                                Nombre = dt.Rows[0]["NombreArticulo"].ToString();
                                MessageBox.Show("El codigo de barras Ya existe sobre el articulo" + Nombre, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);

                                Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    var Items = e.Row.Item as CodigosBarras;
                                    if (Items != null)
                                    {
                                        barras.Remove(Items);
                                    }
                                }), System.Windows.Threading.DispatcherPriority.Background);
                                e.Cancel = true;
                                return;
                            }
                        }
                    }
                }

            }
            catch
            (Exception)
            {
                throw;
            }
        }
    }
}
