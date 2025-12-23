using MMC_Software.Helpers;
using MMC_Software.Repositorys;
using MMC_Software.Services;
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
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Navigation;

namespace MMC_Software.ViewModel
{
    public class ArticulosViewModel : INotifyPropertyChanged
    {
        public static readonly serviceTransactionCreationArticle ServiceArticleCreation = new serviceTransactionCreationArticle();
        public static readonly RepositorioOperacionesMatematicas RepoMate = new RepositorioOperacionesMatematicas();
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
            CalCostPlusTax = new RelayCommand(CalculatorCostPlusTax);
            CalCostMenTax = new RelayCommand(CalculatorCostMeTax);
            CalculatePriceSale = new RelayCommand(CalculatorPriceSale);
            CalculatePriceMinim = new RelayCommand(CalculatorPriceMinim);
            CreateNewArticle = new RelayCommand(GetCodeArticle);
            SaveArticleBD = new RelayCommand(SaveArticle);
        }

        ///<summary>
        /// Icommand View
        /// </summary>

        public ICommand CalCostPlusTax { get; }
        public ICommand CalCostMenTax { get; }
        public ICommand CalculatePriceSale { get; }
        public ICommand CalculatePriceMinim {  get; }
        public ICommand CreateNewArticle { get; }
        public ICommand SaveArticleBD { get; }

        /// <summary>
        ///   METODOS
        /// </summary>
        /// 


        public bool ValidateCompleteDate()
        {
            if(CodigoArticulo == null) return false;
            else if(Namearticle == null)return false;
            else if(CostoSinIva == 0)return false;
            else if(CostoConIva == 0) return false;
            else if(PrecioVenta<0) return false;
            else if(CategoriaSeleccionadaID <0) return false;
            else if(SubCategoriesSelecctionID<0)return false;
            else if(PrecioMinimo<0) return false;
            else
            {
                return true;
            }
        }

        public void SaveArticle(object obj)
        {
            bool Complete = ValidateCompleteDate();
            if(Complete == false)
            {
                MessageBox.Show("Revisa Datos de Articulos", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                bool Creado= ServiceArticleCreation.ServiceCreateArticle(this);
                if(Creado == false)
                {
                    MessageBox.Show("Mal");
                }
                else
                {
                    MessageBox.Show("Bien");
                }
            }
        }
        public void GetCodeArticle(object obj)
        {
            if(ArticulosID == 0)
            {
                string CodigoNew = ServiceArticleCreation.GenerateCodeArticle();
                CodigoArticulo = CodigoNew;
            }
        }
        public void CalculatorPriceMinim(object obj)
        {
            PrecioMinimo = Math.Round(RepoMate.CalcularValorVentaIncremento(2, CostoSinIva, Margen, IvaCategoria),2);
            if (PrecioMinimo > PrecioVenta)
            {
                PrecioVenta = PrecioMinimo;
            }
        }
        public void CalculatorPriceSale(object obj)
        {
            PrecioVenta = RepoMate.CalcularValorVentaIncremento(2, CostoSinIva,Incremento, IvaCategoria);
            Utilidad = PrecioVenta - CostoConIva;
        }

        public void CalculatorCostPlusTax(object obj)
        {
            CostoConIva = Math.Round(RepoMate.CalcularValorMasIva(CostoSinIva, IvaCategoria),2);
            PrecioVenta = CostoConIva;
            PrecioMinimo = CostoConIva;
        }

        public void CalculatorCostMeTax(object obj)
        {
            CostoSinIva = Math.Round(RepoMate.CalcularValirSinIva(CostoConIva,IvaCategoria),2);
            PrecioVenta = CostoConIva;
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
        public bool _Actualizando = false;

        private string _CodigoArticulo;
        public string  CodigoArticulo
        {
            get { return _CodigoArticulo; }
            set
            {
                _CodigoArticulo = value;
                OnPropertyChanged(nameof(CodigoArticulo));
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
                    OnPropertyChanged(nameof(Incremento));
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

        private decimal _Utilidad;
        public decimal Utilidad
        {
            get => _Utilidad;
            set
            {
                if(_Utilidad != value)
                {
                    _Utilidad = value;
                    OnPropertyChanged(nameof(Utilidad));
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

        private string _codebar;
        public string Codebar
        {
            get => _codebar;
            set
            {
                if(_codebar!= value)
                {
                    _codebar = value;
                    OnPropertyChanged(nameof(Codebar));
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

        ///<summary>
        ///
        /// Botones
        /// </summary>
        /// 
        private bool _BtnCrear= true;  
        public bool BtnCrear
        {
            get =>_BtnCrear;
            set
            {
                if (_BtnCrear!=value)
                {
                    _BtnCrear=value;
                    OnPropertyChanged(nameof(BtnCrear));
                }
            }
        }

        private bool _BtnEditar= true;
        public bool BtnEditar
        {
            get=>_BtnEditar;
            set
            {
                if(_BtnEditar != value)
                {
                    _BtnEditar = value;
                    OnPropertyChanged(nameof(BtnEditar));
                }
            }
        }

        private bool _BtnDelete= true;
        public bool BtnDelete
        {
            get=> _BtnDelete;
            set
            {
                if (_BtnDelete != value)
                {
                    _BtnDelete=value;  
                    OnPropertyChanged(nameof(BtnDelete));
                }
            }
        }

        private bool _BtnGuarde= true;
        public bool BtnGuarde
        {
            get => _BtnGuarde;
            set
            {
               if( _BtnGuarde != value)
                {
                    _BtnGuarde = value;
                    OnPropertyChanged(nameof(BtnGuarde));
                }
            }
        }

        private bool _BtnClose =true;
        public bool BtnClose
        {
            get=>_BtnClose;
            set
            {
                if(_BtnClose != value)
                {
                    _BtnClose=value;
                    OnPropertyChanged(nameof(BtnClose));
                }
            }
        }
    }
}
