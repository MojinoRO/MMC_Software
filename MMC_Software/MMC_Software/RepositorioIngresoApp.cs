using System;
using System.Data;
using System.Data.SqlClient;

namespace MMC_Software
{
    public class RepositorioIngresoApp
    {
        public readonly string _Conexion;

        public static class UsuariosID
        {
            public static int UsuarioID { get; set; }
        }

        public RepositorioIngresoApp(string Conexion)
        {
            _Conexion = Conexion;
        }


        public int BuscarUsuarioID(string Usuario, string Clave)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = "select  UsuariosID from  ConfUsuarios WHERE NombreUsuario = @usuario AND UsuarioClave = @clave";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@usuario", Usuario);
                    cmd.Parameters.AddWithValue("@clave", Clave);
                    int UsuarioID = Convert.ToInt32(cmd.ExecuteScalar());
                    UsuariosID.UsuarioID = UsuarioID;
                    LlenarDatosPredeterminados(UsuarioID);
                    return UsuarioID;
                }
            }
        }

        public int LeerTipoUsuario(int UsuarioID)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();

                string Consulta = @"select  TipoUsuario from  ConfUsuarios WHERE UsuariosID=@id";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@id", UsuarioID);
                    int TipoUsuarioID = Convert.ToInt32(cmd.ExecuteScalar());
                    return TipoUsuarioID;
                }
            }
        }

        //==========================================
        // Datos por defecto del usuario
        //==========================================

        public static class DatosPredeterminadosUsuarios
        {
            public static int? VendedorID { get; set; }
            public static int? AlmacenID { get; set; }
            public static int? DocumentoID { get; set; }
        }

        public void LlenarDatosPredeterminados(int UsuarioID)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select IdAlmacen,VendedorID,DocumentoID from ConfUsuarios where UsuariosID=@usuarioid";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@usuarioid", UsuarioID);
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            var Row = dt.Rows[0];
                            DatosPredeterminadosUsuarios.VendedorID = Row.Field<int?>("VendedorID");
                            DatosPredeterminadosUsuarios.AlmacenID = Row.Field<int?>("IdAlmacen");
                            DatosPredeterminadosUsuarios.DocumentoID = Row.Field<int?>("DocumentoID");
                        }
                    }
                }
            }
        }

        public DataTable LlenarDatosEmpresa()
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select EmpresaEstablecimiento ,CONCAT(EmpresaNit,'-',EmpresaDv)as Nit, EmpresaDireccion,EmpresaTelefono from ConfEmpresa";
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
    }
}
