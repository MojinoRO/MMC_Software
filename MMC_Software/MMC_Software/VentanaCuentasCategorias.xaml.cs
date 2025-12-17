using System.Windows;

namespace MMC_Software
{
    public partial class VentanaCuentasCategorias : Window
    {
        private int categoriaId;

        public VentanaCuentasCategorias(int idCategoria, string codigo, string nombre)
        {
            InitializeComponent();
            categoriaId = idCategoria;
            txtCodigoCategoria.Text = codigo;
            txtNombreCategoria.Text = nombre;

        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
