using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMC_Software.DTOs
{
    public class ArticulosDTO
    {
        public int ArticulosID { get; set; }
        public string CodigoArticulo { get; set; }
        public string NombreArticulo { get; set; }
        public string Referencia { get; set; }
        public string CodigoBarras { get; set; }

        public int CategoriasID { get; set; }
        public int SubCategoriaID { get; set; }
        public int MarcasID { get; set; }

        public decimal CostoSinIva { get; set; }
        public decimal CostoConIva { get; set; }
        public decimal Iva { get; set; }

        public decimal Incremento { get; set; }
        public decimal Margen { get; set; }
        public decimal Utilidad { get; set; }

        public decimal PrecioVenta { get; set; }
        public decimal PrecioMinimo { get; set; }
    }

}
