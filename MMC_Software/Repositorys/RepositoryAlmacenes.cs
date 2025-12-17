using System.Data;
using System.Data.SqlClient;

namespace MMC_Software
{

    // Esta Clase es para manejar un dividido mi codigo UI/BD/LOGICA
    //Esta sera para la BD

    public class AlmacenRepositorio
    {
        private readonly string _Conexion;

        // Este metodo recibe por parametro la cadena de conexion 

        public AlmacenRepositorio(string Conexion)
        {
            _Conexion = Conexion;
        }

        // Método que devuelve todos los almacenes

        // El parametro tipo consulta me especifica si lo quiero con saldo o sin saldo 
        // El tipo 1 es Sin Saldo
        // El Tipo 2 es Con Saldo
        public DataTable DevolverAlmacenes(int TipoConsulta, int ArticuloID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta1 = @"select  concat(CodigoAlmacen,'  ',NombreAlmacen)AS Almacen , IdAlmacen ,CodigoAlmacen,NombreAlmacen
                                   from ConfAlmacen";

                string Consulta2 = @"select  concat(A.CodigoAlmacen,'  ',A.NombreAlmacen)AS Almacen , A.IdAlmacen ,A.CodigoAlmacen,A.NombreAlmacen
                                    , ISNULL(S.SaldoActual,0) AS SaldoActual
                                    from ConfAlmacen A
                                    LEFT JOIN InveSaldoInvetarios S ON A.IdAlmacen=S.IdAlmacen AND S.ArticulosID=@articuloid";

                string ConsultaFinal = (TipoConsulta == 1) ? Consulta1 : Consulta2;

                using (SqlCommand cmd = new SqlCommand(ConsultaFinal, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@articuloid", SqlDbType.Int)).Value = ArticuloID;
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    {
                        adp.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public DataTable DevolverAlmacenXUsuario(int UsuarioID)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"SELECT CONCAT(Alm.CodigoAlmacen,'  ',Alm.NombreAlmacen) AS Almacen,Alm.IdAlmacen
                                    FROM ConfAlmacen Alm
                                    INNER JOIN ConfUsuarios USU ON Alm.IdAlmacen = USU.IdAlmacen;";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        return dt;
                    }
                }
            }
        }
    }
}
