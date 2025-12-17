using System.Data;
using System.Data.SqlClient;


namespace MMC_Software
{
    public class RepositorioClavesAcceso
    {
        private string _Conexion;

        public RepositorioClavesAcceso(string conexion)
        {
            _Conexion = conexion;
        }

        public bool UpdateClaves(string ClavePos, string ClaveUsuarios, string ClaveClaves,string ClaveInventarios)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Update = @"UPDATE ConfSistemaClaves SET
                                ClavePost=@pos,
                                ClaveUsuarios=@usuarios,
                                ClaveSistemaClaves=@claves,
								ClaveInventarios=@inventarios";
                using (SqlCommand cmd = new SqlCommand(Update, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@pos", SqlDbType.NVarChar)).Value = ClavePos;
                    cmd.Parameters.Add(new SqlParameter("@usuarios", SqlDbType.NVarChar)).Value = ClaveUsuarios;
                    cmd.Parameters.Add(new SqlParameter("@claves", SqlDbType.NVarChar)).Value = ClaveClaves;
                    cmd.Parameters.Add(new SqlParameter("@inventarios",SqlDbType.NVarChar)).Value=ClaveInventarios;
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }

        public DataTable LlenarClaves()
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select * from ConfSistemaClaves";
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

        public bool ValidaClavePos(string Clave)
        {
            string ClaveBD = null;
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select  ClavePost from ConfSistemaClaves";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClaveBD = reader.GetString(0);
                        }
                    }
                }
            }
            if (ClaveBD == null) return false;
            bool Ok = RespositorioBCryp.VerifyPassword(Clave, ClaveBD);
            return Ok;
        }

        public bool ValidaClaveUsuarios(string Clave)
        {
            string ClaveBD = null;
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select  ClaveUsuarios from ConfSistemaClaves";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClaveBD = reader.GetString(0);
                        }
                    }
                }
            }
            if (ClaveBD == null) return false;
            bool Ok = RespositorioBCryp.VerifyPassword(Clave, ClaveBD);
            return Ok;
        }

        public bool ValidaClaveSistemaClaves(string Clave)
        {
            string ClaveBD = null;
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select  ClaveSistemaClaves from ConfSistemaClaves";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClaveBD = reader.GetString(0);
                        }
                    }
                }
            }
            if (ClaveBD == null) return false;
            bool Ok = RespositorioBCryp.VerifyPassword(Clave, ClaveBD);
            return Ok;
        }

        public bool ValidaClaveInventarios(string Clave)
        {
            string ClaveBD= null;
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open ();
                string Query = "SELECT ClaveInventarios FROM ConfSistemaClaves";
                using (SqlCommand cmd = new SqlCommand(Query, conn))
                {
                    using (SqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            ClaveBD= Reader.GetString(0);
                        }
                        if(ClaveBD == null)return false;
                        bool Ok = RespositorioBCryp.VerifyPassword(Clave, ClaveBD);
                        return Ok;
                    }
                }
            }
        }
    }
}
