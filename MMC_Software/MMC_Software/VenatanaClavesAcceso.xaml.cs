using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VenatanaClavesAcceso.xaml
    /// </summary>
    public partial class VentanaClavesAcceso : UserControl
    {
        public string _ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
        private string _ClavePos;
        private string _ClaveUsuarios;
        private string _ClaveClaves;
        public VentanaClavesAcceso()
        {
            InitializeComponent();
            DesbloquearCampos();
            LlenarClaves();
        }
        private void BloquearCampos()
        {
            pwdClaveCaja.IsEnabled = false;
            pwdClaveSistema.IsEnabled = false;
            pwdClaveUsuarios.IsEnabled = false;
        }

        private void DesbloquearCampos()
        {
            pwdClaveCaja.IsEnabled = true;
            pwdClaveSistema.IsEnabled = true;
            pwdClaveUsuarios.IsEnabled = true;
        }

        private void LlenarClaves()
        {
            try
            {
                var Claves = new RepositorioClavesAcceso(_ConexionSql);
                DataTable dt = Claves.LlenarClaves();
                if (dt.Rows.Count > 0)
                {
                    _ClavePos = dt.Rows[0]["ClavePost"].ToString();
                    _ClaveUsuarios = dt.Rows[0]["ClaveUsuarios"].ToString();
                    _ClaveClaves = dt.Rows[0]["ClaveSistemaClaves"].ToString();

                    // VISUAL DE 5 PARA VALIDAR CON UN IF
                    pwdClaveCaja.Password = "*****";
                    pwdClaveUsuarios.Password = "*****";
                    pwdClaveSistema.Password = "*****";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message, "Mensaje", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ClavePos;
                string ClaveUsuarios;
                string ClaveClaves;

                if (pwdClaveCaja.Password == "*****")
                {
                    ClavePos = _ClavePos;
                }
                else
                {
                    ClavePos = RespositorioBCryp.HashPassword(pwdClaveCaja.Password);
                }

                if (pwdClaveUsuarios.Password == "*****")
                {
                    ClaveUsuarios = _ClaveUsuarios;
                }
                else
                {
                    ClaveUsuarios = RespositorioBCryp.HashPassword(pwdClaveUsuarios.Password);
                }

                if (pwdClaveSistema.Password == "*****")
                {
                    ClaveClaves = _ClaveClaves;
                }
                else
                {
                    ClaveClaves = RespositorioBCryp.HashPassword(pwdClaveSistema.Password);
                }

                var claves = new RepositorioClavesAcceso(_ConexionSql);
                bool Actualizo = claves.UpdateClaves(ClavePos, ClaveUsuarios, ClaveClaves);
                if (Actualizo == true)
                {
                    MessageBox.Show("Registro Actualizado Con Exito", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Warning);
                    BloquearCampos();
                    CerrarVentana();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message, "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            CerrarVentana();
        }

        private void CerrarVentana()
        {
            var TabAbierto = this.Parent as TabItem;
            if (TabAbierto != null)
            {
                var Items = TabAbierto.Parent as TabControl;
                if (Items != null)
                {
                    Items.Items.Remove(TabAbierto);
                }
            }
        }
    }
}
