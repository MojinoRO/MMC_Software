using System.Windows;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaClave.xaml
    /// </summary>
    public partial class VentanaClave : Window
    {

        //NOTA EL TIPO PERMISO ES SI ES CLAVE DEL SISTEMA DE CLAVES O SI ES CLAVE DE VENDEDORES
        // SE MANEJA EL 1 PARA CLAVES DE SISTEMA DE CLAVES
        // SE MANEJA EL 2 PARA CLAVES DE VENDEDORES

        //LA SEGUNDA VARIBALE ES PARA EL VENDEDORID SI EL PARAMETRO DE TIPO DE PERMISO ES 0

        // LA TERCERO VARIABLE ES PARA EL PERMISO QUE VA EJECUTAR

        private int _TipoPermisoID;
        private int _VendedorID;
        private int _PermisoID;

        private string _ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
        public VentanaClave(int TipoPermiso, int ParametroID, int PermisoID)
        {
            InitializeComponent();
            _TipoPermisoID = TipoPermiso;
            _VendedorID = ParametroID;
            _PermisoID = PermisoID;
            TxtClave.Focus();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            string ClaveDigitada = TxtClave.Password;
            if (string.IsNullOrEmpty(ClaveDigitada))
            {
                MessageBox.Show("Clave Incorrecta", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_TipoPermisoID == 2 && _VendedorID != 0 && _PermisoID == 0)
            {
                ValidaContraseñaVendedor(ClaveDigitada);
            }
            else
            {
                ValidaContraseñas(_PermisoID, ClaveDigitada);
            }
        }
        private void ValidaContraseñaVendedor(string Clave)
        {

            var Vendedor = new RespositorioOpcionesPOS(_ConexionSql);
            bool Contraseña = Vendedor.ValidaClaveVendedor(_VendedorID, Clave);
            if (Contraseña == true)
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MostrarMensajeClaveIncorrecta();
            }
        }

        private void ValidaContraseñas(int PermisoID, string Clave)
        {
            //Aqui se va manejar Un numero por cada indice de la contraseña empezando con el 0
            // 1 es la clave del pos
            // 2 es la clave de usuarios
            // 3 es la clave de claves de acceso 
            // 4 es la clave de Inventatios
            int Permiso = PermisoID;
            var Claves = new RepositorioClavesAcceso(_ConexionSql);
            bool esvalida = false;
            switch (Permiso)
            {
                case 1:
                    esvalida = Claves.ValidaClavePos(Clave);
                    break;
                case 2:
                    esvalida = Claves.ValidaClaveUsuarios(Clave);
                    break;
                case 3:
                    esvalida = Claves.ValidaClaveSistemaClaves(Clave);
                    break;
                case 4:
                    esvalida = Claves.ValidaClaveInventarios(Clave);
                    break;
                default:
                    MessageBox.Show("No Se Encontro Clave Especificada", "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
            }
            if (esvalida == true)
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MostrarMensajeClaveIncorrecta();
            }

        }

        private void MostrarMensajeClaveIncorrecta()
        {
            MessageBox.Show("Clave incorrecta.", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtClave.Clear();
            TxtClave.Focus();
        }

        private void TxtClave_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnAceptar.Focus();
            }
        }
    }
}
