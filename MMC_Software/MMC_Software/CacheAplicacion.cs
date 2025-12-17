using System.Data;
using System.Data.SqlClient;

namespace MMC_Software
{
    public class CacheAplicacion
    {
        private readonly string _Conexion;

        public CacheAplicacion(string conexion)
        {
            _Conexion = conexion;
        }

        // ===========================================================================
        //                         LLENAR CACHE GLOBAL
        // ===========================================================================
        public static class CacheGlobal
        {
            public static DataTable vendedores { get; set; }
            public static DataTable vendedoresXUsuario { get; set; }
            public static DataTable Documento { get; set; }
            public static DataTable DocumentoXUsuarios { get; set; }
            public static DataTable Almacenes { get; set; }
            public static DataTable AlmacenXUsuario { get; set; }
            public static DataTable DatosEmpresa { get; set; }

        }

        public void LlenarCacheAplicacion(int UsuarioID)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                string ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
                var Vendedores = new VendedoresRepositorio(ConexionSql);
                CacheGlobal.vendedores = Vendedores.GenerarVendedores();
                CacheGlobal.vendedoresXUsuario = Vendedores.GenerarVendedoresXUsuario(UsuarioID);
                var Documento = new DocumentosVentaRespositorio(ConexionSql);
                CacheGlobal.Documento = Documento.ListarDocumentoVentas();
                CacheGlobal.DocumentoXUsuarios = Documento.DevolverDoctoVentaXUsuario(UsuarioID);
                var almacen = new AlmacenRepositorio(ConexionSql);
                CacheGlobal.Almacenes = almacen.DevolverAlmacenes(1, 0);
                CacheGlobal.AlmacenXUsuario = almacen.DevolverAlmacenXUsuario(UsuarioID);
                var Ingreso = new RepositorioIngresoApp(ConexionSql);
                CacheGlobal.DatosEmpresa = Ingreso.LlenarDatosEmpresa();
            }
        }
    }
}
