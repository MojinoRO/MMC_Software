using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows;

namespace MMC_Software
{
    public class RepositorySale
    {
        private SqlConnection _conn;
        private SqlTransaction _Trans;
        private string _ConexionSql ;

        public RepositorySale(string ConexionSql)
        {
            _ConexionSql = ConexionSql;
            _Trans =null;
            _conn = null;
        }

        public RepositorySale(SqlTransaction trans)
        {
            _Trans = trans;
            _conn = _Trans.Connection;
        }


        // Métodos para manejar las ventas en la base de datos

        public void ActualizarFechaVenta(int ventaID, DateTime nuevaFecha)
        {
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                string query = "UPDATE InveVentas SET Fecha = @NuevaFecha WHERE VentaID = @VentaID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@NuevaFecha",SqlDbType.Date)).Value = nuevaFecha;
                    cmd.Parameters.Add(new SqlParameter("@VentaID",SqlDbType.Int)).Value = ventaID;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void ActualizarClienteVenta(int ventaID, int nuevoTerceroID)
        {
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                string query = "Update InveVentas set TercerosID=@terceroidnew where VentaID=@ventaid";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@terceroidnew", SqlDbType.Int)).Value = nuevoTerceroID;
                    cmd.Parameters.Add(new SqlParameter("@ventaid", SqlDbType.Int)).Value = ventaID;
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public void ActualizarObservacionesVenta(int ventaID, string nuevasObservaciones)
        {
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                string query = "UPDATE InveVentas SET Detalle = @NuevasObservaciones WHERE VentaID = @VentaID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@NuevasObservaciones",SqlDbType.NVarChar, 500)).Value = nuevasObservaciones;
                    cmd.Parameters.Add(new SqlParameter("@VentaID",SqlDbType.Int)).Value = ventaID;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public int GenerarConsecutivoVentas(string CodigoDocto)
        {
            int ConsecutivoGenerado = 0;
            using (SqlCommand cmd = new SqlCommand("Sp_ActializarConsecutivo", _conn, _Trans))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@codigoDocumento", SqlDbType.NVarChar, 4)).Value = CodigoDocto;

                SqlParameter ConsecuitvoBD = new SqlParameter("@nuevoconsecutivo", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(ConsecuitvoBD);
                cmd.ExecuteNonQuery();
                ConsecutivoGenerado = Convert.ToInt32(ConsecuitvoBD.Value);
                if (ConsecuitvoBD.Value == DBNull.Value)
                    throw new Exception("No se pudo generar el consecutivo, verifique que el código del documento sea correcto.");
            }

            return ConsecutivoGenerado;
        }

        public int InsertarEncabezadoVentas(int DocumentoID, int Consecutivo, DateTime DateSale, int VendedorID, int TerceroID, string ObservacionVenta)
        {
            int VentaID = 0;
            using (SqlCommand cmd = new SqlCommand("Sp_InsertarVenta", _conn, _Trans))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@documentoid", SqlDbType.Int)).Value = DocumentoID;
                cmd.Parameters.Add(new SqlParameter("@numerodocumento", SqlDbType.Int)).Value = Consecutivo;
                cmd.Parameters.Add(new SqlParameter("@fecha", SqlDbType.Date)).Value = DateSale;
                cmd.Parameters.Add(new SqlParameter("@terceroid", SqlDbType.Int)).Value = TerceroID;
                cmd.Parameters.Add(new SqlParameter("@vendedorid", SqlDbType.Int)).Value = VendedorID;
                cmd.Parameters.Add(new SqlParameter("@detalle", SqlDbType.NVarChar, 500)).Value = ObservacionVenta;
                VentaID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return VentaID;
        }


       public int  InsertarVentaDetalle(int VentaID,int ArticuloID,int AlmacenID,decimal CostoSinIva,decimal IvaArticulo,decimal VentaSinIva,
           decimal VentaConIva, decimal Cantidad, string DetalleArticulo)
        {
            int VentaDetalleID = 0;
            using (SqlCommand cmd = new SqlCommand("Sp_InsertaVentaDetalle", _conn, _Trans))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ventaid", SqlDbType.Int).Value = VentaID;
                cmd.Parameters.Add("@articulid", SqlDbType.Int).Value = ArticuloID;
                cmd.Parameters.Add("@almacenid", SqlDbType.Int).Value = AlmacenID;
                cmd.Parameters.Add("@costoSinIva", SqlDbType.Money).Value = CostoSinIva;
                cmd.Parameters.Add("@ivaArticulo", SqlDbType.Decimal).Value = IvaArticulo;
                cmd.Parameters.Add("@ventasiniva", SqlDbType.Money).Value = VentaSinIva;
                cmd.Parameters.Add("@ventamasiva", SqlDbType.Money).Value = VentaConIva;
                cmd.Parameters.Add("@cantidad", SqlDbType.Decimal).Value = Cantidad;
                cmd.Parameters.Add("@detallearticulo", SqlDbType.NVarChar, 200).Value = DetalleArticulo;
                VentaDetalleID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return VentaDetalleID;
        }
    }
}
