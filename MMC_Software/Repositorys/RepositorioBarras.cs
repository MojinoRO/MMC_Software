using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using static MMC_Software.VentanaCodigosBarraArticulos;

namespace MMC_Software
{
    public class RepositorioBarras
    {
        public string _ConexionSql;

        public RepositorioBarras(string conexionSql)
        {
            _ConexionSql = conexionSql;
        }

        public bool insertBarras(int ArticuloID, List<CodigosBarras> Barras)
        {
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                int ID = ArticuloID;
                conn.Open();
                SqlTransaction Trans = conn.BeginTransaction();
                try
                {
                    foreach (var BR in Barras)
                    {
                        if (string.IsNullOrEmpty(BR.CodigoBarras))
                            continue;
                        using (SqlCommand cmd = new SqlCommand("SP_InsertarCodigoBarras", conn, Trans))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@articulosid", SqlDbType.Int)).Value = ID;
                            cmd.Parameters.Add(new SqlParameter("@codigobarras", SqlDbType.NVarChar)).Value = BR.CodigoBarras;
                            cmd.Parameters.Add(new SqlParameter("@nombrebarras", SqlDbType.NVarChar)).Value = BR.NombreBarras;
                            cmd.Parameters.Add(new SqlParameter("@cantidad", SqlDbType.Decimal)).Value = BR.Cantidad;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    Trans.Commit();
                    return true;
                }
                catch (Exception)
                {

                    Trans.Rollback();
                    throw;
                }
            }
        }

        public bool ValidaExisteCodigoBarras(string Barras)
        {
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                bool existe = false;
                conn.Open();
                string Consulta = @"select count(*) from InveBarrasArticulos where CodigoBarras=@barras";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@barras", Barras);
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            existe = true;
                        }
                    }
                }
                return existe;
            }
        }

        public DataTable TraerNombreExisteBarras(string Barras)
        {
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                DataTable Datos = new DataTable();

                string ConsultaArticulo = @"SELECT A.NombreArticulo
                                           FROM InveBarrasArticulos b
                                           INNER JOIN InveArticulos a on b.ArticulosID=a.ArticulosID AND b.CodigoBarras=@barras";
                using (SqlCommand cmd2 = new SqlCommand(ConsultaArticulo, conn))
                {
                    cmd2.Parameters.AddWithValue("@barras", Barras);
                    using (SqlDataAdapter adp2 = new SqlDataAdapter(cmd2))
                    {
                        DataTable dt2 = new DataTable();
                        adp2.Fill(dt2);
                        return dt2;
                    }
                }

            }
        }

        public ObservableCollection<CodigosBarras> ListadoCodigosBD(int ArticulosID)
        {

            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                string Consulta = @"SELECT CodigoBarras,NombreBarras,Cantidad
                                FROM InveBarrasArticulos  WHERE ArticulosID=@articulosid";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@articulosid", ArticulosID);
                    using (SqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            barras.Add(new CodigosBarras
                            {
                                CodigoBarras = Reader.GetString(Reader.GetOrdinal("CodigoBarras")),
                                NombreBarras = Reader.GetString(Reader.GetOrdinal("NombreBarras")),
                                Cantidad = Reader.GetDecimal(Reader.GetOrdinal("Cantidad")),
                                Yaguardado = true
                            });
                        }
                    }
                }
                return barras;
            }
        }
    }
}
