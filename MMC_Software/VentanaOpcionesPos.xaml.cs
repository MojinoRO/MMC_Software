using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaOpcionesPos.xaml
    /// </summary>
    public partial class VentanaOpcionesPos : Window
    {
        private readonly int _VendedorID;
        // ESTADO 1 MODIFICANDO  0 CREANDO
        private int _Estado = 0;


        public VentanaOpcionesPos(int VendedorID)
        {
            InitializeComponent();
            _VendedorID = VendedorID;
            LllenarDatos();
        }

        private void LllenarDatos()
        {
            string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
            var OpcionesPos = new RespositorioOpcionesPOS(ConexionSql);
            DataTable dt = OpcionesPos.LlenarDatos(_VendedorID);
            if (dt.Rows.Count > 0)
            {
                string Signo = "$";
                int Base = Convert.ToInt32(dt.Rows[0]["BaseCaja"]);
                txtBaseCaja.Text = Signo + Base.ToString("N2");
                txtClavePos.Password = dt.Rows[0]["ClaveCajero"].ToString();
                _Estado = 1;
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                string Clave = txtClavePos.Password;
                string Hash = RespositorioBCryp.HashPassword(Clave);
                int Base = Convert.ToInt32(txtBaseCaja.Text);
                var Opciones = new RespositorioOpcionesPOS(ConexionSql);
                if (_Estado == 0)
                {
                    bool Crear = Opciones.InsertOpcionesPos(_VendedorID, Base, Hash);
                    if (Crear == true)
                    {
                        MessageBox.Show("Registro Creado Correctamente", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                else
                {
                    bool Actualiza = Opciones.UpdateOpcionesPos(_VendedorID, Base, Hash);
                    if (Actualiza == true)
                    {
                        MessageBox.Show("Registro Actualizado Correctamente", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.ToString());
            }
        }

        private void txtBaseCaja_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, "[0-9]+$");
        }

        private void txtBaseCaja_TextChanged(object sender, TextChangedEventArgs e)
        {
            string Base = txtBaseCaja.Text;
            if (string.IsNullOrEmpty(Base))
            {
                txtBaseCaja.Text = "0";
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
