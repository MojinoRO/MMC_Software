using MMC_Software.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MMC_Software.VentanaBuscarArticulos;

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
                        (CodigoArticulo,
                        NombreArticulo,
                        CostoArticuloSinIva,
                        CostoArticuloMasIva,
                        ArticulosVenta,
                        ArticulosMargen,
                        ArticulosIncremento,
                        ArticulosUtilidad,
                        ArticulosVentaMinima,
                        CategoriasID,
                        SubCategoriaID,
                        ArticulosBarras,
                        MarcasID,
                        ArticulosReferencias,
                        CostoAnterior,
                        IvaArticulo)

                        VALUES

                        (@codigo,
                        @nombre,
                        @costosiniva,
                        @costomasiva,
                        @precioventa,
                        @margen,
                        @incremento,
                        @utilidad,
                        @ventaminima,
                        @categoriaid,
                        @subcategoriaid,
                        @barras,
                        @marcasid,
                        @referencias,
                        @costoanterior,
                        @ivaarticulo)";
            using (SqlCommand cmd = new SqlCommand(Query, _Conn, _Trans))
            {
                cmd.Parameters.Add(new SqlParameter("@codigo", SqlDbType.NVarChar)).Value = CodeArticle;
                cmd.Parameters.Add(new SqlParameter("@nombre", SqlDbType.NVarChar)).Value = NameArticle;
                cmd.Parameters.Add(new SqlParameter("@costosiniva", SqlDbType.Decimal)).Value = CosteSinIva;
                cmd.Parameters.Add(new SqlParameter("@costomasiva", SqlDbType.Decimal)).Value = CosteMasIva;
                cmd.Parameters.Add(new SqlParameter("@precioventa", SqlDbType.Decimal)).Value = PriceSale;
                cmd.Parameters.Add(new SqlParameter("@margen", SqlDbType.Decimal)).Value = Margen;
                cmd.Parameters.Add(new SqlParameter("@incremento", SqlDbType.Decimal)).Value = Incremento;
                cmd.Parameters.Add(new SqlParameter("@utilidad", SqlDbType.Decimal)).Value = Utility;
                cmd.Parameters.Add(new SqlParameter("@ventaminima", SqlDbType.Decimal)).Value = PriceMinim;
                cmd.Parameters.Add(new SqlParameter("@categoriaid", SqlDbType.Int)).Value = CategoryID;
                cmd.Parameters.Add(new SqlParameter("@subcategoriaid", SqlDbType.Int)).Value = SubCategoriaID;
                cmd.Parameters.Add(new SqlParameter("@barras", SqlDbType.NVarChar)).Value = string.IsNullOrEmpty(CodeBarr)?(object) DBNull.Value : CodeBarr;
                cmd.Parameters.Add(new SqlParameter("@marcasid", SqlDbType.Int)).Value = MarcaArticleID;
                cmd.Parameters.Add(new SqlParameter("@referencias", SqlDbType.NVarChar)).Value = Reference;
                cmd.Parameters.Add(new SqlParameter("@costoanterior", SqlDbType.Decimal)).Value = CosteSinIva;
                cmd.Parameters.Add(new SqlParameter("@ivaarticulo", SqlDbType.Decimal)).Value = TarifaIva;
                cmd.ExecuteNonQuery();
            }
        }

        public ArticulosDTO ChangedArticle(int articuloID)
        {
            string query = @"SELECT 
                            AR.ArticulosID, AR.CodigoArticulo,AR.NombreArticulo,AR.CostoArticuloSinIva,
                            AR.CostoArticuloMasIva, AR.ArticulosVenta,AR.ArticulosMargen,AR.ArticulosIncremento,
                            AR.ArticulosUtilidad, AR.ArticulosVentaMinima , CAT.CategoriasID,SCAT.SubCategoriaID,
                            AR.ArticulosBarras,AR.MarcasID,AR.ArticulosReferencias,AR.CostoAnterior,AR.IvaArticulo
                            FROM InveArticulos AR
                            INNER JOIN ConfCategoriasInve CAT ON CAT.CategoriasID=AR.CategoriasID
                            INNER JOIN ConfSubCategorias SCAT ON SCAT.SubCategoriaID=AR.SubCategoriaID
                            INNER JOIN ConfMarcas MAR ON MAR.MarcasID=AR.MarcasID  WHERE AR.ArticulosID=@articuloid";
            using (SqlCommand cmd = new SqlCommand(query, _Conn, _Trans))
            {
                cmd.Parameters.Add(new SqlParameter("@articuloid",SqlDbType.Int)).Value= articuloID;
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read()) return null;
                    return new ArticulosDTO
                    {
                        ArticulosID = articuloID,
                        CodigoArticulo = dr["CodigoArticulo"].ToString(),
                        NombreArticulo = dr["NombreArticulo"].ToString(),
                        Referencia = dr["ArticulosReferencias"].ToString(),
                        CodigoBarras = dr["ArticulosBarras"] as string,
                        CategoriasID = (int)dr["CategoriasID"],
                        SubCategoriaID = (int)dr["SubCategoriaID"],
                        MarcasID = (int)dr["MarcasID"],
                        CostoSinIva = (decimal)dr["CostoArticuloSinIva"],
                        CostoConIva = (decimal)dr["CostoArticuloMasIva"],
                        Iva = (decimal)dr["IvaArticulo"],
                        Incremento = (decimal)dr["ArticulosIncremento"],
                        Margen = (decimal)dr["ArticulosMargen"],
                        Utilidad = (decimal)dr["ArticulosUtilidad"],
                        PrecioVenta = (decimal)dr["ArticulosVenta"],
                        PrecioMinimo = (decimal)dr["ArticulosVentaMinima"]
                    };
                }
            }
        }
    }
}
