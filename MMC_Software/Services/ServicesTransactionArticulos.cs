using MMC_Software.Repositorys;
using MMC_Software.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MMC_Software.Services
{
    public class serviceTransactionCreationArticle
    {
        string _ConexionSql = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
        public serviceTransactionCreationArticle()
        {

        }
        public string  GenerateCodeArticle()
        {
            string CodigoNew = "";
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    using (SqlCommand  cmd = new SqlCommand("ObtenerCodigoArticulo", conn, trans))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        CodigoNew = Convert.ToString(cmd.ExecuteScalar());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al generar codigo Articulo"+ ex.Message, "Mensaje", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return CodigoNew;
        }

        public bool ServiceCreateArticle(ArticulosViewModel vm)
        {
            bool Creado = false;
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    RepositoryCreacionArticulos repo =
                        new RepositoryCreacionArticulos(trans);

                    repo.CreateArticleBD(
                        vm.CodigoArticulo,
                        vm.Namearticle,
                        vm.Referencia,
                        vm.Codebar,
                        vm.CategoriaSeleccionadaID,
                        vm.SubCategoriesSelecctionID,
                        vm.MarcaSelecctionChanged,
                        vm.CostoSinIva,
                        vm.CostoConIva,
                        vm.IvaCategoria,
                        vm.Incremento,
                        vm.Margen,
                        vm.Utilidad,
                        vm.PrecioVenta,
                        vm.PrecioMinimo
                    );

                    trans.Commit();
                    Creado = true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show(
                        "Error al crear el artículo:\n" + ex.Message,
                        "Error",MessageBoxButton.OK,MessageBoxImage.Error
                    );
                }
            }
            return Creado;
        }
    }

}
