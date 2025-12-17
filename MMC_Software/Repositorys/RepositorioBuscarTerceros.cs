using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls.WebParts;
using static MMC_Software.VentanaBuscarTerceros;

namespace MMC_Software
{
    public class RepositorioBuscarTercero
    {
        public string _ConexionSql;

        public RepositorioBuscarTercero(string ConexionSql)
        {
            _ConexionSql = ConexionSql;
        }

        public ObservableCollection<DatosTercero> BuscarTerceros()
        {
            var Datos = new ObservableCollection<DatosTercero>();
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                string Consulta = @"SELECT TOP 100 TercerosID,TercerosTipoDocumento,TercerosIdentificacion,
                                    TRIM(CONCAT(TercerosNombreCompleto,TerceroRazonSocial))As Nombre ,TercerosEmail FROM InveTerceros";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Datos.Add(new DatosTercero
                            {
                                TerceroID = reader.GetInt32(reader.GetOrdinal("TercerosID")),
                                TipoDocumento = reader.GetInt32(reader.GetOrdinal("TercerosTipoDocumento")),
                                TercerosIdentificacion = reader.GetString(reader.GetOrdinal("TercerosIdentificacion")),
                                TerceroNobres = reader.GetString(reader.GetOrdinal("Nombre")),
                                TerceroEmail = reader.GetString(reader.GetOrdinal("TercerosEmail"))
                            });
                        }
                    }
                }
            }
            return Datos;
        }


        public ObservableCollection<DatosTercero> BuscadorTerceros(string Filtro)
        {
            var Datos = new ObservableCollection<DatosTercero>();
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                string Consulta = @"SELECT  TercerosID,TercerosTipoDocumento,TercerosIdentificacion,
                                    TRIM(CONCAT(TercerosNombreCompleto,TerceroRazonSocial))As Nombre ,TercerosEmail FROM InveTerceros
                                    WHERE TercerosNombreCompleto like @filtro or TerceroRazonSocial LIKE @filtro
                                    or TercerosIdentificacion like @filtro";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@filtro", "%" + Filtro + "%");
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Datos.Add(new DatosTercero
                            {
                                TerceroID = reader.GetInt32(reader.GetOrdinal("TercerosID")),
                                TipoDocumento = reader.GetInt32(reader.GetOrdinal("TercerosTipoDocumento")),
                                TercerosIdentificacion = reader.GetString(reader.GetOrdinal("TercerosIdentificacion")),
                                TerceroNobres = reader.GetString(reader.GetOrdinal("Nombre")),
                                TerceroEmail = reader.GetString(reader.GetOrdinal("TercerosEmail"))
                            });
                        }
                    }
                }
            }
            return Datos;
        }


        public DataTable BuscarCedulaUnitario(string Cedula)
        {
            DataTable  dt = new DataTable();
            if (string.IsNullOrEmpty(Cedula))
                throw new System.ArgumentException("El parámetro no puede estar vacío ", nameof(Cedula));
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT  TercerosID,TercerosTipoDocumento,TercerosIdentificacion,TRIM(CONCAT(TercerosNombreCompleto,TerceroRazonSocial))As Nombre ,TercerosEmail FROM InveTerceros WHERE TercerosIdentificacion = @Cedula", conn))
                {
                    cmd.Parameters.AddWithValue("@Cedula", Cedula);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
                return dt;

        } 
    }
}
