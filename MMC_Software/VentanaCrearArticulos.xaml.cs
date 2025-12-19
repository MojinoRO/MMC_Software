using MMC_Software.ViewModel;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaCrearArticulos.xaml
    /// </summary>
    public partial class VentanaCrearArticulos : Window
    {
        private int ArticulosID;

        public VentanaCrearArticulos(int ArticulosID)
        {
            InitializeComponent();
            this.ArticulosID = ArticulosID;
            this.DataContext = new ArticulosViewModel();
        }
        public VentanaCrearArticulos()
        {
            InitializeComponent();
            this.DataContext = new ArticulosViewModel();
        }

    }
}
