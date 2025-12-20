using MMC_Software.Repositorys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO.Packaging;
using System.Linq;
using System.Printing;
using System.Runtime.InteropServices;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
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
            CmdInputNameArticle = new RelayCommand(InputName);
        }

        ///<summary>
        /// Icommand View
        /// </summary>

        public ICommand CmdInputNameArticle { get;}

        /// <summary>
        ///   METODOS
        /// </summary>


        public void InputName(object obj)
        {

        }

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

        private string _Namearticle;
        public string Namearticle
        {
            get => _Namearticle;
            set
            {
                if(_Namearticle != value)
                {
                    _Namearticle = value;
                    OnPropertyChanged(nameof(Namearticle));
                }
            }
        }

        private string _Referencia;
        public string Referencia
        {
            get => _Referencia;
            set
            {
                if(_Referencia != value)
                {
                    _Referencia = value;
                    OnPropertyChanged(nameof(Referencia));
                }
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
        private decimal _CostoSinIva;
        public decimal CostoSinIva
        {
            get => _CostoSinIva;
            set
            {
                if(_CostoSinIva != value)
                {
                    _CostoSinIva= value;
                    OnPropertyChanged(nameof(CostoSinIva));
                }
            }
        }

        private decimal _CostoConIva;
        public decimal CostoConIva
        {
            get => _CostoConIva;
            set
            {
                if (_CostoConIva != value)
                {
                    _CostoConIva= value;
                    OnPropertyChanged(nameof(CostoConIva));
                }
            }
        }

        private decimal _Margen;
        public decimal Margen
        {
            get => _Margen;
            set
            {
                if(_Margen != value)
                {
                    _Margen= value;
                    OnPropertyChanged(nameof(Margen));
                }
            }
        }

        private decimal _Incremento;
        public decimal Incremento
        {
            get => _Incremento;
            set
            {
                if (_Incremento != value)
                {
                    _Incremento = value;
                    OnPropertyChanged(nameof(Margen));
                }
            }
        }

        private decimal _PrecioVenta;
        public decimal PrecioVenta
        {
            get => _PrecioVenta;
            set
            {
                if(_PrecioVenta!= value)
                {
                    _PrecioVenta = value;
                    OnPropertyChanged(nameof(PrecioVenta));
                }
            }
        }

        private decimal _PrecioMinimo;
        public decimal PrecioMinimo
        {
            get => _PrecioMinimo;
            set
            {
                if(_PrecioMinimo != value)
                {
                    _PrecioMinimo= value;
                    OnPropertyChanged(nameof(PrecioMinimo));
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
