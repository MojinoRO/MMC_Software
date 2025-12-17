using System;
using System.Data;
using System.Data.SqlClient;

namespace MMC_Software
{
    public class RespositorioOpcionesPOS
    {
        private readonly string _Conexion;

        public RespositorioOpcionesPOS(string Conexion)
        {
            _Conexion = Conexion;
        }

        public bool InsertOpcionesPos(int VendedorID, int BaseCaja, string Clave)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"Insert into ConfOpcionesVendedores (VendedorID,BaseCaja , ClaveCajero) values
                                    (@vendedorid,@base,@clave)";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@vendedorid", SqlDbType.Int)).Value = VendedorID;
                    cmd.Parameters.Add(new SqlParameter("@base", SqlDbType.Decimal)).Value = BaseCaja;
                    cmd.Parameters.Add(new SqlParameter("@clave", SqlDbType.NVarChar)).Value = Clave;
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }

        public bool UpdateOpcionesPos(int VendedorID, int BaseCaja, string Clave)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string consulta = @"update ConfOpcionesVendedores set
                                    BaseCaja=@basenew,
                                    ClaveCajero=@clavenew
                                    where VendedorID=@vendedorid";
                using (SqlCommand cmd = new SqlCommand(consulta, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@basenew", SqlDbType.Int)).Value = BaseCaja;
                    cmd.Parameters.Add(new SqlParameter("@clavenew", SqlDbType.NVarChar)).Value = Clave;
                    cmd.Parameters.Add(new SqlParameter("@vendedorid", SqlDbType.Int)).Value = VendedorID;
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }

        public DataTable LlenarDatos(int VendedorID)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select COUNT(*) from ConfOpcionesVendedores where VendedorID=@vendedorid";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@vendedorid", VendedorID);
                    int Cantidad = Convert.ToInt32(cmd.ExecuteScalar());
                    if (Cantidad > 0)
                    {
                        string ConsultaDatos = @"select BaseCaja, ClaveCajero from ConfOpcionesVendedores 
                                                where VendedorID=@vendedorid";
                        using (SqlCommand cmd2 = new SqlCommand(ConsultaDatos, conn))
                        {
                            cmd2.Parameters.AddWithValue("@vendedorid", VendedorID);
                            using (SqlDataAdapter adp = new SqlDataAdapter(cmd2))
                            {
                                DataTable dt = new DataTable();
                                adp.Fill(dt);
                                return dt;
                            }
                        }
                    }
                    else
                    {
                        return new DataTable();
                    }
                }
            }
        }

        public bool ValidaClaveVendedor(int VendedorID, string Clave)
        {
            string ClaveGuardada = null;

            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select ClaveCajero from ConfOpcionesVendedores where VendedorID=@vendedorid";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@vendedorid", SqlDbType.Int)).Value = VendedorID;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClaveGuardada = reader.GetString(0);
                        }
                    }
                }

                if (ClaveGuardada == null) return false;
                bool ok = RespositorioBCryp.VerifyPassword(Clave, ClaveGuardada);
                return ok;
            }
        }
    }
}
