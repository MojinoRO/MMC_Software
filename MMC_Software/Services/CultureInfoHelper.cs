using System;
using System.Globalization;

namespace MMC_Software.Helpers
{
    public static class CultureHelper
    {
        // Cultura regional por defecto (Colombia en este ejemplo)
        public static CultureInfo CulturaRegional { get; private set; }
            = new CultureInfo("es-CO");

        // Método por si en un futuro quieres cambiar la cultura dinámicamente
        public static void SetCulture(string cultureName)
        {
            CulturaRegional = new CultureInfo(cultureName);
        }

        // Para formatear números
        public static string FormatearNumero(decimal valor, string formato = "N2")
        {
            return valor.ToString(formato, CulturaRegional);
        }

        // Para formatear valores monetarios
        public static string FormatearMoneda(decimal valor)
        {
            return valor.ToString("C", CulturaRegional);
        }

        // Para parsear números respetando la configuración regional
        public static decimal ParseNumero(string valor)
        {
            return decimal.Parse(valor, CulturaRegional);
        }

        // Para formatear fechas dd/MM/yyyy
        public static string FormatearFecha(DateTime fecha)
        {
            return fecha.ToString("d", CulturaRegional);
        }

        // Para parsear fechas dd/MM/yyyy
        public static DateTime ParseFecha(string fecha)
        {
            return DateTime.Parse(fecha, CulturaRegional);
        }
    }
}
