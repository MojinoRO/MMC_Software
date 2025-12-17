using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MMC_Software
{
    public class RespositoryInsertBD
    {
        public readonly string _Conexion;
        public RespositoryInsertBD(string Conexion)
        {
            _Conexion = Conexion;
        }

        public void CrearUsuarios(
                                    int TipoUsuario, string NombreUsuario, int? AlmacenID, int? VendedorID, int? DocumentosID,
                                    int ModuloPos, int ModuloTaller, int ModuloCartera, int ModuloNomina, int ModuloContabilidad, int ModuloInventarios)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_Conexion))
                {
                    conn.Open();
                    string Clave = "123";

                    string Insert = @"
                                    INSERT INTO ConfUsuarios 
                                    (TipoUsuario, NombreUsuario, UsuarioClave, VendedorID, IdAlmacen, DocumentoID, 
                                     ModuloInventarios, ModuloContabilidad, ModuloCartera, ModuloPos, ModuloNomina, ModuloTaller) 
                                    VALUES
                                    (@tipo, @nombre, @clave, @vendedor, @almacen, @documento, 
                                     @inve, @conta, @cartera, @pos, @nomina, @taller)";

                    using (SqlCommand cmd = new SqlCommand(Insert, conn))
                    {
                        cmd.Parameters.AddWithValue("@tipo", TipoUsuario);
                        cmd.Parameters.AddWithValue("@nombre", NombreUsuario);
                        cmd.Parameters.AddWithValue("@clave", Clave);
                        cmd.Parameters.AddWithValue("@vendedor", (object)VendedorID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@almacen", (object)AlmacenID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@documento", (object)DocumentosID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@inve", ModuloInventarios);
                        cmd.Parameters.AddWithValue("@conta", ModuloContabilidad);
                        cmd.Parameters.AddWithValue("@cartera", ModuloCartera);
                        cmd.Parameters.AddWithValue("@pos", ModuloPos);
                        cmd.Parameters.AddWithValue("@nomina", ModuloNomina);
                        cmd.Parameters.AddWithValue("@taller", ModuloTaller);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message, "MENSAJE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
