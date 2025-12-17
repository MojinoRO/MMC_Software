using System;
using System.Windows;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaDevueltasVentas.xaml
    /// </summary>
    public partial class VentanaDevueltasVentas : Window
    {

        public double _ValorAPagar;

        public double _ValorRecibido = 0;

        public double _ValorDevueltas = 0;


        public VentanaDevueltasVentas(double ValorAPagar)
        {
            InitializeComponent();
            _ValorAPagar = ValorAPagar;
            LlenarValorAPagar();
            TxtValorRecibido.Focus();
        }

        private void LlenarValorAPagar()
        {
            try
            {
                TxtValorApagar.Text = _ValorAPagar.ToString("N2", System.Globalization.CultureInfo.CurrentCulture);

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Button2k_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double ValorActual = _ValorRecibido;
                _ValorRecibido = ValorActual + 2000;
                LLenarEntregado();
                MostrarValorDevuletas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void LLenarEntregado()
        {
            string Signo = "$";
            try
            {
                TxtValorRecibido.Text = Signo + " " +
                    _ValorRecibido.ToString("N2", System.Globalization.CultureInfo.CurrentCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.ToString(), "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MostrarValorDevuletas()
        {
            try
            {
                string Signo = "$";
                double Recibido = _ValorRecibido;
                double ValorAPagar = _ValorAPagar;

                double Devueltas = Recibido - ValorAPagar;

                TxtVueltas.Text = Signo + " " +
                    Devueltas.ToString("N2", System.Globalization.CultureInfo.CurrentCulture);
                _ValorDevueltas = Devueltas;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Button5K_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _ValorRecibido = _ValorRecibido + 5000;
                LLenarEntregado();
                MostrarValorDevuletas();

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Button10K_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _ValorRecibido = _ValorRecibido + 10000;
                LLenarEntregado();
                MostrarValorDevuletas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Button20K_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _ValorRecibido = _ValorRecibido + 20000;
                LLenarEntregado();
                MostrarValorDevuletas();

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Button50K_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _ValorRecibido = _ValorRecibido + 50000;
                LLenarEntregado();
                MostrarValorDevuletas();

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Button100K_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _ValorRecibido = _ValorRecibido + 100000;
                LLenarEntregado();
                MostrarValorDevuletas();

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonM1K_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _ValorRecibido = _ValorRecibido + 1000;
                LLenarEntregado();
                MostrarValorDevuletas();

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonM500K_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _ValorRecibido = _ValorRecibido + 500;
                LLenarEntregado();
                MostrarValorDevuletas();

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void ButtonM200K_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _ValorRecibido = _ValorRecibido + 200;
                LLenarEntregado();
                MostrarValorDevuletas();

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonM100K_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _ValorRecibido = _ValorRecibido + 100;
                LLenarEntregado();
                MostrarValorDevuletas();

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonM50K_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _ValorRecibido = _ValorRecibido + 50;
                LLenarEntregado();
                MostrarValorDevuletas();

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            GuardarCambios();
        }

        private void GuardarCambios()
        {
            try
            {
                if (_ValorRecibido < _ValorAPagar)
                {
                    MessageBox.Show("El Valor A Recibido Es Menor Al Valor De La Venta", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                else
                {
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_ValorRecibido != 0)
                {
                    _ValorRecibido = 0;
                    LLenarEntregado();
                    MostrarValorDevuletas();
                    return;

                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TxtValorRecibido_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void TxtValorRecibido_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                double ValorDigitado = Convert.ToDouble(TxtValorRecibido.Text);
                string Signo = "$";
                TxtValorRecibido.Text = Signo + ValorDigitado.ToString("N2", System.Globalization.CultureInfo.CurrentCulture);
                _ValorRecibido = ValorDigitado;
                LLenarEntregado();
                MostrarValorDevuletas();
                BtnAceptar.Focus();
            }
        }

    }
}
