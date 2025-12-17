using System.Data;
using System.Data.SqlClient;

namespace MMC_Software
{
    public class VendedoresRepositorio
    {
        private readonly string _Conexion;

        public VendedoresRepositorio(string Conexion)
        {
            _Conexion = Conexion;
        }

        public DataTable GenerarVendedores()
        {

            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select  concat(CodigoVendedor,'  ',NombreVendedor)AS INFOVENDEDOR , VendedorID from ConfVendedores";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    return dt;
                }
            }
        }

        public DataTable GenerarVendedoresXUsuario(int UsuarioID)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string consulta = @"select CONCAT(ven.CodigoVendedor,'  ',ven.NombreVendedor) as INFOVENDEDOR , ven.VendedorID FROM ConfVendedores ven
                                    inner join ConfUsuarios usu on ven.VendedorID=usu.VendedorID and usu.UsuariosID=@filtro";
                using (SqlCommand cmd = new SqlCommand(consulta, conn))
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

        public DataTable GenerarInfoVendedorUnitario(int? VendedorID)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select CONCAT(CodigoVendedor,' ',NombreVendedor)AS INFOCOMPLETA , VendedorID
                                    from ConfVendedores where VendedorID=@vendedorid";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@vendedorid", VendedorID);
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
