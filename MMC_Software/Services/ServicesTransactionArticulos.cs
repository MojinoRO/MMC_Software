using MMC_Software.Repositorys;
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
        private static readonly RepositoryCreacionArticulos RepoArticle= new RepositoryCreacionArticulos(ConfiguracionConexion.ObtenerCadenaConexion(
            ConexionEmpresaActual.BaseDeDatosSeleccionada));
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
    }

}
