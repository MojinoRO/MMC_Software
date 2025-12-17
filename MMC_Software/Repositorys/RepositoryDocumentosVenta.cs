using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Hosting;

namespace MMC_Software
{
    public class DocumentosVentaRespositorio
    {
        private readonly string _Conexion;

        public DocumentosVentaRespositorio(string Conexion)
        {
            _Conexion = Conexion;
        }

        public DataTable ListarDocumentoVentas()
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select CONCAT(CodigoDocumento,' ',NombreDocumento) AS INFODOCUMENTO , DocumentoID ,CodigoDocumento,NombreDocumento
                                    from ConfDocumentos where TipoDocumento=2";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    return dt;
                }
            }
        }
        public DataTable DevolverDoctoVentaXUsuario(int UsuarioID)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select CONCAT(doc.CodigoDocumento,'  ',doc.NombreDocumento) as INFODOCUMENTO , doc.DocumentoID FROM ConfDocumentos doc
                                    inner join ConfUsuarios usu on doc.DocumentoID=usu.DocumentoID and usu.UsuariosID=@filtro";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@filtro", UsuarioID);
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public DataTable ListarDocumentosCompras()
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select CONCAT(CodigoDocumento,' ',NombreDocumento) AS INFODOCUMENTO , DocumentoID ,CodigoDocumento,NombreDocumento
                                    from ConfDocumentos where TipoDocumento=1";
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

        public DataTable BuscarDocumentoXCodigoVentas(string Codigo)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select DocumentoID,CodigoDocumento,NombreDocumento from ConfDocumentos where TipoDocumento=2 and CodigoDocumento=@codigo";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@codigo", SqlDbType.NVarChar, 4)).Value = Codigo;
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public DataTable BuscarDoctoComprasXcodigo(string Codigo,int tipoDocumento)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select DocumentoID,concat( CodigoDocumento,'  ',NombreDocumento) AS InfoDocto from ConfDocumentos
                                    where TipoDocumento=@tipodoc and CodigoDocumento like @codigo + '%'";
                try
                {
                    using (SqlCommand  cmd = new SqlCommand(Consulta, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@tipodoc",SqlDbType.Int)).Value= tipoDocumento;
                        cmd.Parameters.Add(new SqlParameter("@codigo",SqlDbType.NVarChar,4)).Value= Codigo;
                        using (SqlDataAdapter ado = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            ado.Fill(dt);
                            return dt;
                        }
                    }
                }catch (Exception ex)
                {
                    throw new Exception("Error " + ex.Message);
                }
            }
        }

        public DataTable BuscarDocumentoXCodigoCompras(string Codigo)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select DocumentoID,CodigoDocumento,NombreDocumento from ConfDocumentos where TipoDocumento=1 and CodigoDocumento=@codigo";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@codigo", SqlDbType.NVarChar, 4)).Value = Codigo;
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
