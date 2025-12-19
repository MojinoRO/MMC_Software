using MMC_Software.Repositorys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO.Packaging;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Navigation;

namespace MMC_Software.ViewModel
{
    public class ArticulosViewModel : INotifyPropertyChanged
    {
        public static readonly RepositoryCreacionArticulos RepoCreacionArticulos = new RepositoryCreacionArticulos(
            ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada));
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
            LoadedCategories();
            LoadedMarcas();
        }


        /// <summary>
        ///   METODOS
        /// </summary>

        public void LoadedMarcas()
        {
            if (ArticulosID == 0)
            {
                Marca = RepoCreacionArticulos.GetMarcas();
            }
        }

        public void LoadedSubCategories()
        {
            if (CategoriaSeleccionadaID > 0)
            {
                SubCategories = RepoCreacionArticulos.GetSubCategories(CategoriaSeleccionadaID);
            }
        }

        public void LoadedCategories()
        {
            if (Categories != null && Categories.Rows.Count > 0) return;
            else
            {
                Categories = RepoCreacionArticulos.GetCategorias();
            }
        }

        /// <summary>
        ///       PROPIEDADES DE VIEW MODEL 
        /// </summary>

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

        private DataTable _SubCategories;
        public DataTable SubCategories
        {
            get => _SubCategories;
            set
            {
                if(_SubCategories != value)
                {
                    _SubCategories = value;
                    OnPropertyChanged(nameof(SubCategories));
                }
            }
        }

        private DataTable _Marca;
        public DataTable Marca
        {
            get => _Marca;
            set
            {
                if(_Marca != value)
                {
                    _Marca= value; 
                    OnPropertyChanged(nameof(Marca));
                }
            }
        }


        private decimal _IvaCategoria;
        public decimal IvaCategoria
        {
            get => _IvaCategoria;
            set
            {
                if(_IvaCategoria != value)
                {
                    _IvaCategoria = value;
                    OnPropertyChanged(nameof(IvaCategoria));
                }
            }
        }

        private DataRowView _categoriesselection;
        public DataRowView Categoriesselection
        {
            get => _categoriesselection;
            set
            {
                _categoriesselection = value;
                OnPropertyChanged(nameof(Categoriesselection));

                if(_categoriesselection != null)
                {
                    CategoriaSeleccionadaID = Convert.ToInt32(_categoriesselection["CategoriasID"]);
                    IvaCategoria = Convert.ToDecimal(_categoriesselection["TarifaImpuesto"]);
                }
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

                //===>Cargo las subcategorias<===//
                LoadedSubCategories();
            }
        }

        private int _SubCategoriesSelecctionID;
        public int SubCategoriesSelecctionID
        {
            get => _SubCategoriesSelecctionID;
            set
            {
                _SubCategoriesSelecctionID= value;
                OnPropertyChanged(nameof(SubCategoriesSelecctionID));
            }
        }

        private int _marcaSelecctionChanged;
        public int MarcaSelecctionChanged
        {
            get => _marcaSelecctionChanged;
            set
            {
                _marcaSelecctionChanged= value;
                OnPropertyChanged(nameof(MarcaSelecctionChanged));
            }
        }
    }

}
