using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MMC_Software
{
    public static class ConexionEmpresaActual
    {
        public static string BaseDeDatosSeleccionada { get; set; }
    }

    public static class ConfiguracionConexion
    {
        public static string RutaXML = "Configuracion.txt";

        public static string ObtenerCadenaConexion(string nombreBaseDatos)
        {
            try
            {
                //valido si existe el Archivo TXT EXISTE 

                if (!File.Exists(RutaXML))
                {
                    throw new FileNotFoundException("No se encontró el archivo Configuracion.xml");
                }

                //LUEGO LEO CADA LINEA DE CODIGO DEL ARCHUVO QUE TEDRA UN VALOR

                XDocument doc = XDocument.Load(RutaXML);
                var campos = doc.Descendants("Campo").Select(x => x.Value.Trim()).ToList();

                string servidor = campos.Count > 0 ? campos[0] : ".";
                string tipoConexion = campos.Count > 5 ? campos[5] : "0"; // VALIDACION DE USUARIO SQL DONDE  0 = SQL, 1 = Windows
                string usuario = campos.Count > 6 ? campos[6] : "sa"; // USUARIO DE SQL

                // aqui voy a almacenar mi clave que servira en la cadena de conexion
                string clave = "J3Sql*01-2008";

                if (tipoConexion == "1")
                {
                    return $"Data Source={servidor};Initial Catalog={nombreBaseDatos};Integrated Security=True;";
                }
                else
                {
                    return $"Data Source={servidor};Initial Catalog={nombreBaseDatos};User ID={usuario};Password={clave};";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cargar la cadena de conexión: " + ex.Message);
            }
        }
    }

}
