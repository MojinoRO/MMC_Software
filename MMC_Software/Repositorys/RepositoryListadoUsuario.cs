using System;
using System.Data;
using System.Data.SqlClient;

namespace MMC_Software
{
    public class ListadoUsuariosRepositorio
    {

        public readonly string _Conexion;

        public int _UsuarioID;

        public ListadoUsuariosRepositorio(string Conexion)
        {
            _Conexion = Conexion;
        }

        public DataTable ListadoUsuarios()
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();

                string Consulta = @"select NombreUsuario,UsuariosID  from ConfUsuarios";

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

        public DataTable LlenarDatos(int UsuarioID)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                string Consulta = @"select TipoUsuario,NombreUsuario,VendedorID,IdAlmacen,DocumentoID,ModuloPos,ModuloTaller,
                ModuloCartera,ModuloNomina,ModuloInventarios,ModuloContabilidad from ConfUsuarios where UsuariosID=@usuarioid";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@usuarioid", UsuarioID);
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public bool ExisteUsuario(int UsuarioID)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string consulta = "select COUNT(*) from ConfUsuarios where UsuariosID=@FILTRO";
                using (SqlCommand cmd = new SqlCommand(consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@filtro", UsuarioID);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    int Cantidad = Convert.ToInt32(cmd.ExecuteScalar());
                    if (Cantidad > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }


        public int ConsultarUsuarioID(string UsuarioNombre)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select UsuariosID from ConfUsuarios where NombreUsuario=@filtro";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@filtro", UsuarioNombre);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    int UsuarioID = Convert.ToInt32(cmd.ExecuteScalar());
                    return UsuarioID;
                }
            }
        }
        public void UpdateDatos(int TipoUsuario, string NombreUsuario, int? AlmacenID, int? VendedorID, int? DocumentosID,
                                    int ModuloPos, int ModuloTaller, int ModuloCartera, int ModuloNomina, int ModuloContabilidad, int ModuloInventarios, int ID)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Update = @"update ConfUsuarios set
                                NombreUsuario=@nombrenew,
                                TipoUsuario=@tiponew,
                                VendedorID=@vendedornew,
                                IdAlmacen=@almacennew,
                                DocumentoID=@documentonew,
                                ModuloPos=@moduloposnew,
                                ModuloTaller=@modulotallernew,
                                ModuloCartera=@modulocarteranew,
                                ModuloInventarios=@moduloinventarionew,
                                ModuloContabilidad=@modulocontabilidadnew
                                where UsuariosID=@usuarioID";
                using (SqlCommand cmd = new SqlCommand(Update, conn))
                {
                    cmd.Parameters.AddWithValue("@nombrenew", NombreUsuario);
                    cmd.Parameters.AddWithValue("@tiponew", TipoUsuario);
                    cmd.Parameters.AddWithValue("@vendedornew", (object)VendedorID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@almacennew", (object)AlmacenID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@documentonew", (object)DocumentosID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@moduloposnew", ModuloPos);
                    cmd.Parameters.AddWithValue("@modulotallernew", ModuloTaller);
                    cmd.Parameters.AddWithValue("@modulocarteranew", ModuloCartera);
                    cmd.Parameters.AddWithValue("@moduloinventarionew", ModuloInventarios);
                    cmd.Parameters.AddWithValue("@modulocontabilidadnew", ModuloContabilidad);
                    cmd.Parameters.AddWithValue("@usuarioID", ID);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public void EliminarUsuarios(int UsuarioID)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                String Consulta = @"delete from ConfUsuarios where UsuariosID=@filtro";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@filtro", UsuarioID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool ConsultaNombreUsuario(string NombreUsuario)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select COUNT(*) from ConfUsuarios where NombreUsuario=@filtro";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@filtro", NombreUsuario);
                    int Existe = Convert.ToInt32(cmd.ExecuteScalar());
                    if (Existe > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

    }
}
