using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaEliminaArticulos.xaml
    /// </summary>
    public partial class VentanaEliminaArticulos : Window
    {
        public VentanaEliminaArticulos()
        {
            InitializeComponent();
            txtDesde.Focus();
        }

        private void txtDesde_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !EsNumero(e.Text);
        }

        private bool EsNumero(string texto)
        {
            return texto.All(char.IsDigit);
        }

        private void txtHasta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnAceptar.Focus();
            }
        }

        private void txtDesde_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtHasta.Text = txtDesde.Text;
                txtHasta.Focus();
                txtHasta.SelectAll();
            }
        }
        public int ArtDesde { get; private set; }
        public int ArtHasta { get; private set; }


        private void GuardaRegistros()
        {
            int Desde, Hasta;
            if (!int.TryParse(txtDesde.Text, out Desde))
            {
                MessageBox.Show("Debe agregar producto desde", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtDesde.Focus();
                return;
            }
            if (!int.TryParse(txtHasta.Text, out Hasta))
            {
                MessageBox.Show("Debe agregar producto hasta", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtHasta.Focus();
                return;
            }
            if (Desde > Hasta)
            {
                MessageBox.Show("El valor desde no puede ser mayor que el hasta", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ArtDesde = Desde;
            ArtHasta = Hasta;
            this.DialogResult = true;
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GuardaRegistros();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
