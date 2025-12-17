using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
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



        ///Reportes 
        ///
        public static string ObtenerRutadeReportes(string NombreReporte)
        {
            try
            {
                if (!File.Exists(RutaXML))
                    throw new FileNotFoundException("No se ha encontrado archivo configuracion");

                XDocument doc = XDocument.Load (RutaXML);

                var Campos = doc.Descendants("Campo").Select(X=> X.Value.Trim()).ToList();

                // En el XML el segundo campo (índice 1) es la ruta de reportes
                string RutaReporte = Campos.Count > 1 ? Campos[1] : string.Empty;

                if (string.IsNullOrEmpty(RutaReporte))
                    throw new Exception("No se ha encontrado reporte");

                if (!Directory.Exists(RutaReporte))
                    throw new FileNotFoundException("No se encontro la ruta del reporte");

                var archivos = Directory.GetFiles(RutaReporte, NombreReporte,SearchOption.AllDirectories);
                if (archivos.Length == 0)
                    throw new FileNotFoundException("No se encontro reporte");

                return archivos[0];
            }
            catch(Exception ex)
            {
                throw new Exception("Error al cargar el archivo" + ex.Message);
            }
        }
    }

}
