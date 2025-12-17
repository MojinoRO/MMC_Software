using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;

namespace MMC_Software
{
    public class RepositorioGuardaPagos
    {
        private string _ConexionSql;

        public RepositorioGuardaPagos(string conexionSql)
        {
            _ConexionSql = conexionSql;
        }

        public void InsertarGuardaPagos(int FormaPagoID,int DocumentoID, int NumeroDocumento,DateTime FechaCompra,
            int? VendedorID, decimal ValorPagado)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_ConexionSql))
                {
                    conn.Open();
                    string Consulta = @"INSERT INTO ConfGuardaPagos(FormaPagoID,DocumentoID,NumeroDocumento,Fecha,VendedorID,ValorPagado)
                                    VALUES(@formapagoid,@documentoid,@numerodocumento,@fecha,@vendedorid,@valorpagado)";
                    using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                    {
                        cmd.Parameters.Add("@formapagoid", SqlDbType.Int).Value = FormaPagoID;
                        cmd.Parameters.Add("@documentoid", SqlDbType.Int).Value = DocumentoID;
                        cmd.Parameters.Add("@numerodocumento", SqlDbType.Int).Value = NumeroDocumento;
                        cmd.Parameters.Add("@fecha", SqlDbType.Date).Value = FechaCompra;
                        cmd.Parameters.Add("@vendedorid", SqlDbType.Int).Value = VendedorID;
                        cmd.Parameters.Add("@valorpagado", SqlDbType.Money).Value = ValorPagado;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error " + ex.ToString());
            }
        }
    }
}
