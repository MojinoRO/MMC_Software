using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MMC_Software
{
    /// <summary>
    /// </summary>
    public partial class VentanaRegistroVentas : UserControl
    {

        private readonly IDialogService _dialogServices = new DialogService();
        public VentanaRegistroVentas(int? VendedorID, int? AlmacenID)
        {
            InitializeComponent();
            BloquarCampos();
            this.DataContext = new VentanaRegistroVentaViewModel();
            if(this.DataContext is VentanaRegistroVentaViewModel vm)
            {
                vm.SolicitudFoco += vm_SolicitudFoco;
                vm.SolicitarFoco += Vm_SolicitarFocoCelda;
            }
            CargarVendedores();
        }

        private void vm_SolicitudFoco(string NombreControl)
        {
            switch (NombreControl)
            {
                case "dpfecha":
                    dpfecha.Focus();
                    break;
                case "TxtClienteVenta":
                    TxtClienteVenta.Focus();
                    break;
                case "TxtObservacionesVenta":
                    TxtObservacionesVenta.Focus();
                    break;
                case "btnAgregarProductoVenta":
                    btnAgregarProductoVenta.Focus();
                    break;

            }
        }


        private void BloquarCampos()
        {
            btnBuscarClienteVenta.IsEnabled=false;
            txtBase19Venta.IsEnabled=false;
            txtBase5Venta.IsEnabled=false;
            txtDescuentoVenta.IsEnabled=false;
            txtExentoVenta.IsEnabled = false;
            txtIVA19Venta.IsEnabled = false;
            txtIVA5Venta.IsEnabled=false;
            txtSubTotalVenta.IsEnabled=false;
            txtTotalIVAVenta.IsEnabled=false;
            txtTotalVenta.IsEnabled=false;
            TxtDocumentoVenta.IsEnabled = false;
            TxtNombreDocumentoVenta.IsEnabled = false;
            cbxVendedor.IsEnabled = false;
            TxtObservacionesVenta.IsEnabled = false;
            dpfecha.IsEnabled = false;
        }

        private void DesbloquearCampos()
        {
            btnBuscarClienteVenta.IsEnabled = true;
            txtBase19Venta.IsEnabled = true;
            txtBase5Venta.IsEnabled = true;
            txtDescuentoVenta.IsEnabled = true;
            txtExentoVenta.IsEnabled = true;
            txtIVA19Venta.IsEnabled = true;
            txtIVA5Venta.IsEnabled = true;
            txtSubTotalVenta.IsEnabled = true;
            txtTotalIVAVenta.IsEnabled = true;
            txtTotalVenta.IsEnabled = true;
            TxtDocumentoVenta.IsEnabled = true;
            TxtNombreDocumentoVenta.IsEnabled = true;
            cbxVendedor.IsEnabled = true;
            TxtObservacionesVenta.IsEnabled = true;
            dpfecha.IsEnabled = true;
            if (this.DataContext is VentanaRegistroVentaViewModel vm)
            {
                vm.BtnCrearVenta = true;
            }
        }

        private void btnNuevaVenta_Click(object sender, RoutedEventArgs e)
        {
            if(this.DataContext is VentanaRegistroVentaViewModel vm)
            {
                int VentaID = vm.VentaID;
                bool Modificando = vm.Modificando;
                if(VentaID==0 && Modificando == false)
                {
                    DesbloquearCampos();
                    TxtDocumentoVenta.Focus();
                }
            }
        }

        private void btnCerrarVenta_Click(object sender, RoutedEventArgs e)
        {
            if(this.DataContext is VentanaRegistroVentaViewModel vm)
            {
                int VentaID = vm.VentaID;
                bool Modificando = vm.Modificando;
                if (VentaID == 0 && Modificando == false)
                {
                    var TabAbierto = this.Parent as TabItem;
                    if (TabAbierto!=null)
                    {
                        var Items = TabAbierto.Parent as TabControl;
                        if(Items != null)
                        {
                            Items.Items.Remove(TabAbierto);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No se puede cerrar Venta Sin Finalizar", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void CargarVendedores()
        {
            var Vendedores = CacheAplicacion.CacheGlobal.vendedoresXUsuario;
            if(Vendedores == null)
            {
                Vendedores = CacheAplicacion.CacheGlobal.vendedores;
            }
            cbxVendedor.DisplayMemberPath = "INFOVENDEDOR";
            cbxVendedor.SelectedValuePath = "VendedorID";
            cbxVendedor.ItemsSource = Vendedores.DefaultView;


            // Seleccionar vemdedor
            if(cbxVendedor.Items.Count > 0)
            {
                cbxVendedor.SelectedIndex = 0;
                int VendedorID =Convert.ToInt32(cbxVendedor.SelectedValue);
               if(this.DataContext is VentanaRegistroVentaViewModel vm)
                {
                    vm.VendedorID= VendedorID;
                }
            }
        }

        private void TxtObservacionesVenta_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DataContext is VentanaRegistroVentaViewModel vm)
            {
                vm.DetalleVenta = TxtObservacionesVenta.Text;
            }
        }


        private void dgProductos_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // ERROR corregido: la condición estaba invertida
            if (dgProductos.CurrentCell == null || dgProductos.CurrentCell.Column == null)
                return;

            var columna = dgProductos.CurrentCell.Column as DataGridBoundColumn;
            if (columna == null) return;

            var binding = columna.Binding as Binding;
            string NombreBinding = binding?.Path?.Path;
            if (string.IsNullOrEmpty(NombreBinding)) return;

            if (this.DataContext is VentanaRegistroVentaViewModel vm)
            {
                switch (e.Key)
                {
                    case Key.F1:
                        if (NombreBinding == "CodigoArticulo")
                        {
                            
                            var filaActual = dgProductos.CurrentItem as VentanaRegistroVentaViewModel.ArticulosVenta;
                            if (filaActual == null) return;

                            var articulo = _dialogServices.BuscarArticulo();
                            if (articulo != null)
                            {
                                filaActual.ArticuloID = articulo.ArticuloID;
                                filaActual.CodigoArticulo = articulo.Codigo;
                                filaActual.NombreArticulo = articulo.Nombre;
                                filaActual.IvaArticulo=articulo.TarifaIVA;
                                filaActual.ValorUnitario= articulo.ValorVenta;

                                dgProductos.CurrentCell = new DataGridCellInfo(filaActual, columna);
                                dgProductos.BeginEdit();
                            }

                            e.Handled = true; // IMPORTANTE
                        }
                        break;
                    case Key.Enter:
                        if (NombreBinding == "CodigoAlmacen")
                        {
                            var filaActual = dgProductos.CurrentItem as VentanaRegistroVentaViewModel.ArticulosVenta;
                            if (filaActual == null) return;
                            var Almacen = _dialogServices.BuscadorAlmacen(filaActual.ArticuloID);
                            if(Almacen != null)
                            {
                                filaActual.AlmacenID = Almacen.AlmacenID;
                                filaActual.CodigoAlmacen= Almacen.CodigoAlmacen;
                            }
                            e.Handled= true;
                        }

                        if(NombreBinding == "CantidadVenta")
                        {
                            var FilaActual = dgProductos.CurrentItem as VentanaRegistroVentaViewModel.ArticulosVenta;
                            if(FilaActual == null) return;
                            try
                            {
                                vm.ProcesingCantDigit(FilaActual);
                            }
                            catch(Exception ex)
                            {
                                MessageBox.Show("Error al procesar cantidad: "+ ex.Message);
                            }
                        }


                        if (dgProductos.CurrentCell != null)
                        {
                            var grid = (DataGrid)sender;

                            int currentColumn = grid.Columns.IndexOf(grid.CurrentCell.Column);
                            int currentRow = grid.Items.IndexOf(grid.CurrentItem);

                            // Primero commit seguro
                            grid.CommitEdit(DataGridEditingUnit.Cell, true);

                            // Cambiar a la siguiente columna
                            if (currentColumn < grid.Columns.Count - 1)
                            {
                                grid.CurrentCell = new DataGridCellInfo(
                                    grid.Items[currentRow],       
                                    grid.Columns[currentColumn + 1]
                                );
                            }
                            else
                            {
                                // Última columna → pasar a la siguiente fila
                                if (currentRow == grid.Items.Count - 1)
                                {
                                    // Crear nueva fila
                                    vm.ListadoArticulos.Add(new VentanaRegistroVentaViewModel.ArticulosVenta
                                    {
                                        Item = vm.ListadoArticulos.Count + 1
                                    });
                                }

                                grid.CurrentCell = new DataGridCellInfo(
                                    grid.Items[Math.Min(currentRow + 1, grid.Items.Count - 1)],
                                    grid.Columns[0]
                                );
                            }

                            grid.BeginEdit();  // muy importante

                            e.Handled = true;
                        }
                        break;

                }
            }
        }

        private void Vm_SolicitarFocoCelda(int fila, int columna)
        {
            try
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (dgProductos.Items.Count == 0) return;
                    if (fila < 0 || columna < 0) return;

                    // Obtener item y columna
                    var item = dgProductos.Items[fila];
                    var col = dgProductos.Columns[columna];

                    // Seleccionar directamente la celda (válido con SelectionUnit="Cell")
                    dgProductos.SelectedCells.Clear();
                    dgProductos.SelectedCells.Add(new DataGridCellInfo(item, col));

                    dgProductos.CurrentCell = new DataGridCellInfo(item, col);

                    dgProductos.ScrollIntoView(item, col);

                    FocusCellNoEdit(dgProductos, fila, columna);

                }), DispatcherPriority.Background);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message);
            }
        }


        private void FocusCellNoEdit(DataGrid dataGrid, int row, int column)
        {
            dataGrid.UpdateLayout();
            dataGrid.ScrollIntoView(dataGrid.Items[row]);

            DataGridRow fila = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(row);
            if (fila == null) return;

            DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(fila);
            DataGridCell celda = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);

            if (celda != null)
            {
                celda.Focus();     // ← SOLO FOCO, NO EDITA
                Keyboard.Focus(celda);
            }
        }


        private T GetVisualChild<T>(Visual parent) where T : Visual
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = (Visual)VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                    return typedChild;

                var result = GetVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

    }
}
