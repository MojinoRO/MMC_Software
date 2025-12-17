using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media.Animation;

namespace MMC_Software
{
    public interface IDialogService
    {
        VentanaBuscarTerceros.DatosTercero BuscarTercero();
        VentanaBuscadorDocumentos.Documento BuscarDocumento();
        VentanaBuscarArticulos.articulos BuscarArticulo();
        VentanaSeleccionarAlmacen.ListadoAlmacen BuscadorAlmacen(int ArticuloID);
        VentanaSelecionarFormaPago.ListadoFormasPago BuscarFormasPagos(int TipoFormaPago, decimal Valor);
    }


    public class DialogService : IDialogService
    {
        public VentanaBuscarTerceros.DatosTercero BuscarTercero()
        {
            var Ventana = new VentanaBuscarTerceros();
            bool? showDialog = Ventana.ShowDialog() ?? false;
            if (showDialog == true && Ventana.TerceroSelecionado != null)
            {
                return Ventana.TerceroSelecionado;
            }
            return null;
        }

        public VentanaBuscadorDocumentos.Documento BuscarDocumento()
        {
            var Ven = new VentanaBuscadorDocumentos(1);
            bool? ShowDialog = Ven.ShowDialog() ?? false;
            if (ShowDialog == true && Ven.DocumentoSeleccionado != null)
            {
                return Ven.DocumentoSeleccionado;
            }
            return null;
        }

        public VentanaBuscarArticulos.articulos BuscarArticulo()
        {
            var ven = new VentanaBuscarArticulos();
            bool? Showdialog = ven.ShowDialog();
            if(Showdialog == true && ven.Articulosseleccionado != null)
            {
                return ven.Articulosseleccionado;
            }
            return null;
        }

        public VentanaSeleccionarAlmacen.ListadoAlmacen BuscadorAlmacen(int ArticuloID)
        {
            var ven = new VentanaSeleccionarAlmacen(ArticuloID);
            bool? Showdialog = ven.ShowDialog();
            if (Showdialog == true && ven.AlmacenSeleccionado != null)
            {
                return ven.AlmacenSeleccionado;
            }
            return null;
        }

        public VentanaSelecionarFormaPago.ListadoFormasPago BuscarFormasPagos(int TipoFormaPago, decimal Total)
        {
            var ven = new VentanaSelecionarFormaPago(TipoFormaPago, Total);
            bool? Showdialog = ven.ShowDialog();
            if (Showdialog == true && ven.FormaPagoSeleccionada != null)
            {
                return ven.FormaPagoSeleccionada;
            }
            return null;
        }
    }
}
