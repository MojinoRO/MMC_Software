using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMC_Software
{
    /// <summary>
    /// Lógica de interacción para VentanaBuscadorDocumentos.xaml
    /// </summary>
    public partial class VentanaBuscadorDocumentos : Window
    {
        string _ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);

        // Parametros de Busqueda

        // el tipo de documento va tener un numero identificador para los tipos de documentos
        // 1 ES PARA VENTAS
        // 2 ES PARA COMPRAS

        int _TipoDocumentoBuscado = 0;


        public VentanaBuscadorDocumentos(int TipoDocto)
        {
            InitializeComponent();
            _TipoDocumentoBuscado = TipoDocto;
            LlenarListadoDocumentos();
        }

        private void LlenarListadoDocumentos()
        {
            DataTable dt = new DataTable();
            int TipoDocumento = _TipoDocumentoBuscado;
            switch (TipoDocumento)
            {
                case 1:
                    dt = DocumentosVentas();
                    break;
                case 2:
                    dt = DocumentosCompras();
                    break;
                default:
                    MessageBox.Show("No se Encontro Tipo Documento", "Mensaje", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
            }
            LstDocumentos.DisplayMemberPath = "INFODOCUMENTO";
            LstDocumentos.SelectedValuePath = "DocumentoID";
            LstDocumentos.ItemsSource = dt.DefaultView;
            Documento Docto = new Documento();
            Docto.DocumentoID = Convert.ToInt32(dt.Rows[0]["DocumentoID"]);
            Docto.DocumentoCodigo = dt.Rows[0]["CodigoDocumento"].ToString();
            Docto.DocumentoNombre = dt.Rows[0]["NombreDocumento"].ToString();
        }

        private DataTable DocumentosVentas()
        {
            var RDocumentos = new DocumentosVentaRespositorio(_ConexionSql);
            DataTable dt = RDocumentos.ListarDocumentoVentas();
            return dt;

        }

        private DataTable DocumentosCompras()
        {
            var RDocumentos = new DocumentosVentaRespositorio(_ConexionSql);
            DataTable dt = RDocumentos.ListarDocumentosCompras();
            return dt;
        }


        public class Documento
        {
            public int DocumentoID { get; set; }
            public string DocumentoNombre { get; set; }
            public string DocumentoCodigo { get; set; }
        }

        public Documento DocumentoSeleccionado { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (LstDocumentos.Items.Count > 0)
            {
                LstDocumentos.SelectedIndex = 0;
                LstDocumentos.Focus();
            }
        }

        private void LstDocumentos_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key== Key.Enter && LstDocumentos.SelectedItem != null)
            {
                ConfirmarSeleccion();
                e.Handled= true;
            }
        }



        private void ConfirmarSeleccion()
        {
            DataRowView dvr = (DataRowView)LstDocumentos.SelectedItem;
            if(dvr!= null)
            {
                DocumentoSeleccionado = new Documento
                {
                    DocumentoID = Convert.ToInt32(dvr["DocumentoID"]),
                    DocumentoNombre = dvr["NombreDocumento"].ToString(),
                    DocumentoCodigo = dvr["CodigoDocumento"].ToString()
                };

                this.DialogResult = true;
                this.Close();
            }
        }

        private void LstDocumentos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(LstDocumentos.SelectedItem!= null)
            {
                ConfirmarSeleccion();
            }
        }
    }
}
