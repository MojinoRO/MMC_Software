using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaBuscarTerceros.xaml
    /// </summary>
    public partial class VentanaBuscarTerceros : Window
    {
        string _ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
        public VentanaBuscarTerceros()
        {
            InitializeComponent();
            llenaDataGridTerceros();
            txtBuscar.Focus();
        }

        public class DatosTercero
        {
            public int TerceroID { get; set; }
            public int TipoDocumento { get; set; }
            public string TercerosIdentificacion { get; set; }
            public string TerceroNobres { get; set; }
            public string TerceroEmail { get; set; }
        }

        private void llenaDataGridTerceros()
        {
            var RBuscador = new RepositorioBuscarTercero(_ConexionSql);
            dgTerceros.ItemsSource = RBuscador.BuscarTerceros();
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                string Texto = txtBuscar.Text;
                if (e.Key == Key.Enter)
                {
                    var RBuscador = new RepositorioBuscarTercero(_ConexionSql);
                    dgTerceros.ItemsSource = RBuscador.BuscadorTerceros(Texto);
                    if (dgTerceros.Items.Count > 0)
                    {
                        dgTerceros.SelectedIndex= 0;
                        dgTerceros.Focus();
                    }
                }
                if (e.Key == Key.Escape)
                {
                    this.DialogResult = false;
                    this.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DatosTercero TerceroSelecionado { get; set; }

        private void dgTerceros_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ConfirmarSeleccion();
        }


        private void ConfirmarSeleccion()
        {
            var celda = dgTerceros.SelectedItem as DatosTercero;
            if (celda != null)
            {
                TerceroSelecionado = new DatosTercero
                {
                    TerceroID = celda.TerceroID,
                    TipoDocumento = celda.TipoDocumento,
                    TercerosIdentificacion = celda.TercerosIdentificacion,
                    TerceroNobres = celda.TerceroNobres,
                    TerceroEmail = celda.TerceroEmail
                };
            }
            DialogResult = true;
            this.Close();
        }

        private void dgTerceros_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && dgTerceros.SelectedItem != null)
            {
                ConfirmarSeleccion();
                e.Handled= true;    
            }
        }
    }
}
