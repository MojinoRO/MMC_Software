using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MMC_Software.Repositorys
{
    public class RepositoryCreacionArticulos
    {
        string _SqlConection;
        SqlTransaction _Trans;
        SqlConnection _Conn;
        public RepositoryCreacionArticulos(string SqlConnection)
        {
            _SqlConection = SqlConnection;
        }

        public RepositoryCreacionArticulos(SqlTransaction Trans)
        {
            _Trans = Trans;
            _Conn= Trans.Connection;
        }
        public DataTable GetCategorias()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(_SqlConection))
            {
                conn.Open();

                string query = @"
                            SELECT 
                            CONCAT(CodigoCategoria, ' ', NombreCategoria) AS InfoCategorias,
	                        TarifaImpuesto,
                            CategoriasID
                            FROM ConfCategoriasInve";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                {
                    adp.Fill(dt);
                }
            }

            return dt;
        }

        public DataTable GetSubCategories(int CatagoriaID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_SqlConection))
            {
                conn.Open();
                string query = @"select CONCAT(CodigoSubCategoria,' ' ,NombreSubCategoria) as InfoSubCategoria,
                                SubCategoriaID
                                from ConfSubCategorias 
                                where CategoriasID=@categoriaid";
                using(SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@categoriaid",SqlDbType.Int)).Value = CatagoriaID;
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    {
                        adp.Fill(dt);
                    }
                }
            }
            return dt;
        }


        public DataTable GetMarcas()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_SqlConection))
            {
                conn.Open();
                string query = @"SELECT
	                            CONCAT(CodigoMarca , ' ',NombreMarca) AS InfoMarca, MarcasID 
	                            FROM  ConfMarcas";
                using (SqlCommand  cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    {
                        adp.Fill(dt);
                    }
                }
            }
            return dt;
        }

    }

}
