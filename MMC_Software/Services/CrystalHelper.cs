using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Data.SqlClient;

namespace MMC_Software
{
    public static class CrystalHelper
    {
        public static void AplicarConexion(ReportDocument reporte, string cadenaConexion)
        {
            var builder = new SqlConnectionStringBuilder(cadenaConexion);

            ConnectionInfo connectionInfo = new ConnectionInfo()
            {
                ServerName = builder.DataSource,
                DatabaseName = builder.InitialCatalog,
                UserID = builder.UserID,
                Password = builder.Password,
                IntegratedSecurity = builder.IntegratedSecurity
            };

            // 🔹 Aplicar la conexión a cada tabla del reporte principal
            foreach (Table table in reporte.Database.Tables)
            {
                TableLogOnInfo logOnInfo = table.LogOnInfo;
                logOnInfo.ConnectionInfo = connectionInfo;
                table.ApplyLogOnInfo(logOnInfo);

                // Importante: cambiar ubicación de base
                if (!string.IsNullOrEmpty(connectionInfo.DatabaseName))
                    table.Location = $"{connectionInfo.DatabaseName}.dbo.{table.Name}";
            }

            // 🔹 También a cada subreporte
            foreach (Section section in reporte.ReportDefinition.Sections)
            {
                foreach (ReportObject reportObject in section.ReportObjects)
                {
                    if (reportObject.Kind == ReportObjectKind.SubreportObject)
                    {
                        SubreportObject subreportObject = (SubreportObject)reportObject;
                        ReportDocument subReport = subreportObject.OpenSubreport(subreportObject.SubreportName);

                        foreach (Table table in subReport.Database.Tables)
                        {
                            TableLogOnInfo logOnInfo = table.LogOnInfo;
                            logOnInfo.ConnectionInfo = connectionInfo;
                            table.ApplyLogOnInfo(logOnInfo);

                            if (!string.IsNullOrEmpty(connectionInfo.DatabaseName))
                                table.Location = $"{connectionInfo.DatabaseName}.dbo.{table.Name}";
                        }
                    }
                }
            }
        }
    }
}
