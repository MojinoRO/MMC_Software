using MMC_Software.Repositorys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Navigation;

namespace MMC_Software.ViewModel
{
    public class ArticulosViewModel : INotifyPropertyChanged
    {
        public int ArticulosID { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ArticulosViewModel(int articulosID)
        {
            ArticulosID = articulosID;
        }
        public ArticulosViewModel()
        {

            //LoadedCategories();
        }

        private bool _CodigoArticulo = false;
        public bool CodigoArticulo
        {
            get { return _CodigoArticulo; }
            set
            {
                _CodigoArticulo = value;
            }
        }

        private DataTable _categories;
        public DataTable Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }
        private int _categoriaSeleccionadaID;
        public int CategoriaSeleccionadaID
        {
            get => _categoriaSeleccionadaID;
            set
            {
                _categoriaSeleccionadaID = value;
                OnPropertyChanged(nameof(CategoriaSeleccionadaID));
            }
        }
        public void LoadedCategories()
        {
            DataTable dt = new DataTable();
            if (Categories != null && Categories.Rows.Count > 0)
            {
                return;
            }
        }
    }

}
