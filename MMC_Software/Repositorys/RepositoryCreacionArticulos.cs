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


        public void CreateArticleBD(string CodeArticle, string NameArticle, string Reference, string CodeBarr,
            int CategoryID, int SubCategoriaID, int MarcaArticleID, decimal CosteSinIva, decimal CosteMasIva, decimal TarifaIva,
            decimal Incremento, decimal Margen, decimal Utility, decimal PriceSale, decimal PriceMinim)
        {
            string Query = @"INSERT INTO InveArticulos
                            (CodigoArticulo, NombreArticulo, CostoArticuloSinIva, CostoArticuloMasIva,
                            ArticulosVenta, ArticulosMargen, ArticulosIncremento,ArticulosUtilidad, ArticulosVentaMinima, CategoriasID,
                            SubCategoriaID, ArticulosBarras , MarcasID, ArticulosReferencias, CostoAnterior,IvaArticulo)
                            VALUES(@codigo,@nombrearticulo,@costosiniva,@costomasiva,@articuloventa,@articulosmargen,@articulosincremento,@articulosutilidad,
                            @articulosventaminima, @categoriaid,@subcategoriaid,@articulosBarras,@marcasid,@articulosreferencia,@costoanterior,@ivaarticulo)";
            using (SqlCommand cmd = new SqlCommand(Query, _Conn, _Trans))
            {
                cmd.Parameters.Add(new SqlParameter("@codigo", SqlDbType.NVarChar)).Value = CodeArticle;
                cmd.Parameters.Add(new SqlParameter("@nombrearticulo", SqlDbType.NVarChar)).Value = NameArticle;
                cmd.Parameters.Add(new SqlParameter("@costosiniva", SqlDbType.Decimal)).Value = CosteSinIva;
                cmd.Parameters.Add(new SqlParameter("@costomasiva", SqlDbType.Decimal)).Value = CosteMasIva;
                cmd.Parameters.Add(new SqlParameter("@articuloventa", SqlDbType.Decimal)).Value = PriceSale;
                cmd.Parameters.Add(new SqlParameter("@articulosmargen", SqlDbType.Decimal)).Value = Margen;
                cmd.Parameters.Add(new SqlParameter("@articulosincremento", SqlDbType.Decimal)).Value = Incremento;
                cmd.Parameters.Add(new SqlParameter("@articulosutilidad", SqlDbType.Decimal)).Value = Utility;
                cmd.Parameters.Add(new SqlParameter("@articulosventaminima", SqlDbType.Decimal)).Value = PriceMinim;
                cmd.Parameters.Add(new SqlParameter("@categoriaid", SqlDbType.Int)).Value = CategoryID;
                cmd.Parameters.Add(new SqlParameter("@subcategoriaid", SqlDbType.Int)).Value = SubCategoriaID;
                cmd.Parameters.Add(new SqlParameter("@articulosBarras", SqlDbType.NVarChar)).Value = CodeBarr;
                cmd.Parameters.Add(new SqlParameter("@marcasid", SqlDbType.Int)).Value = MarcaArticleID;
                cmd.Parameters.Add(new SqlParameter("@articulosreferencia", SqlDbType.NVarChar)).Value = Reference;
                cmd.Parameters.Add(new SqlParameter("@costoanterior", SqlDbType.Decimal)).Value = CosteSinIva;
                cmd.Parameters.Add(new SqlParameter("@ivaarticulo", SqlDbType.Decimal)).Value = TarifaIva;
                cmd.ExecuteNonQuery();
            }
        }
    }

}
