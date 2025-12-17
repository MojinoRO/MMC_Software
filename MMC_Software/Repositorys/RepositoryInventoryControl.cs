using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static MMC_Software.VentanaBuscarArticulos;

namespace MMC_Software
{

    public class RepositoryInventoryControl
    {
        SqlTransaction _Trans;
        SqlConnection _conn;

        public RepositoryInventoryControl(SqlTransaction trans)
        {
            _Trans = trans;
            _conn =_Trans.Connection;
        }

        // Métodos para manejar el control de inventario en la base de datos

        public void InsertarMoviminetoArticulos(DateTime FechaMovimiento, int TipoMovimiento, int DocumentoID, int numdocumento,
          int ArticuloID, int IdAlmacen, decimal Cantidad, decimal CostoUnitario, int ReferenciaOrigen, int? VendedorID, string Observacion)
        {
            try
            {
                string Consulta = @"INSERT INTO InveMovInventarios(FechaMovimiento,TipoMovimiento,DocumentoID,NumDocumento,
                                    ArticulosID,IdAlmacen,Cantidad,CostoUnitario,ReferenciaOrigen,VendedorID,Observaciones)
                                    VALUES(@fechamovimiento,@tipomovimiento,@documentoid,@numdocumento,@articulosid,@idalmacen,
                                    @cantidad,@costounitario,@referenciaorigen,@vendedorid,@observaciones)";
                using (SqlCommand cmd = new SqlCommand(Consulta,_conn,_Trans))
                {
                    cmd.Parameters.Add(new SqlParameter("@fechamovimiento", SqlDbType.Date)).Value = FechaMovimiento;
                    cmd.Parameters.Add(new SqlParameter("@tipomovimiento", SqlDbType.Int)).Value = TipoMovimiento;
                    cmd.Parameters.Add(new SqlParameter("@documentoid", SqlDbType.Int)).Value = DocumentoID;
                    cmd.Parameters.Add(new SqlParameter("@numdocumento", SqlDbType.Int)).Value = numdocumento;
                    cmd.Parameters.Add(new SqlParameter("@articulosid", SqlDbType.Int)).Value = ArticuloID;
                    cmd.Parameters.Add(new SqlParameter("@idalmacen", SqlDbType.Int)).Value = IdAlmacen;
                    cmd.Parameters.Add(new SqlParameter("@cantidad", SqlDbType.Decimal)).Value = Cantidad;
                    cmd.Parameters.Add(new SqlParameter("@costounitario", SqlDbType.Decimal)).Value = CostoUnitario;
                    cmd.Parameters.Add(new SqlParameter("@referenciaorigen", SqlDbType.Decimal)).Value = ReferenciaOrigen;
                    cmd.Parameters.Add(new SqlParameter("@vendedorid", SqlDbType.Int)).Value = VendedorID;
                    cmd.Parameters.Add(new SqlParameter("@observaciones", SqlDbType.NVarChar)).Value = Observacion;
                    cmd.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error al insertar movimiento de artículos: " + ex.Message);
            }
        }

        public void InsertaSaldoArticulo(int ArticuloID, int AlmacenID, decimal Cantidad, decimal CostoSinIva)
        {
            try
            {
                string ConsultaSaldo = @"
                                            IF EXISTS (SELECT 1 FROM InveSaldoInvetarios WHERE ArticulosID=@articuloid AND IdAlmacen=@idalmacen)
                                                UPDATE InveSaldoInvetarios
                                                SET SaldoActual = SaldoActual + @cantidad,
                                                    UltimoCosto = @costounitario,
                                                    FechaUltimoMov = GETDATE()
                                                WHERE ArticulosID = @articuloid AND IdAlmacen = @idalmacen;
                                            ELSE
                                                INSERT INTO InveSaldoInvetarios (ArticulosID, IdAlmacen, SaldoActual, UltimoCosto, FechaUltimoMov)
                                                VALUES (@articuloid, @idalmacen, @cantidad, @costounitario, GETDATE());";

                using (SqlCommand cmd = new SqlCommand(ConsultaSaldo, _conn,_Trans))
                {
                    cmd.Parameters.Add(new SqlParameter("@articuloid", SqlDbType.Int)).Value = ArticuloID;
                    cmd.Parameters.Add(new SqlParameter("@idalmacen", SqlDbType.Int)).Value = AlmacenID;
                    cmd.Parameters.Add(new SqlParameter("@cantidad", SqlDbType.Decimal)).Value = Cantidad;
                    cmd.Parameters.Add(new SqlParameter("@costounitario", SqlDbType.Decimal)).Value = CostoSinIva;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar saldos de artículos: " + ex.Message);
            }
        }

        public void ActualizarCantidadSaldo(int TipoProceso, int ArticuloID, decimal CantidadNew, int IdAlmacen)
        {
            //tipo consulta es 
            //1= suma 
            //2= resta

            string ConsultaSuma = @"UPDATE InveSaldoInvetarios SET 
                                        SaldoActual= SaldoActual + @cantnueva 
                                        where ArticulosID=@articuloid and IdAlmacen=@idalmacen";

            string ConsultaResta = @"UPDATE InveSaldoInvetarios SET 
                                        SaldoActual= SaldoActual - ABS(@cantnueva) 
                                        where ArticulosID=@articuloid and IdAlmacen=@idalmacen";

            string ConsultaFinal = (TipoProceso == 1) ? ConsultaSuma : ConsultaResta;
            try
            {
                using (SqlCommand cmd = new SqlCommand(ConsultaFinal, _conn,_Trans))
                {
                    cmd.Parameters.Add(new SqlParameter("@cantnueva", SqlDbType.Decimal)).Value = CantidadNew;
                    cmd.Parameters.Add(new SqlParameter("@articuloid", SqlDbType.Int)).Value = ArticuloID;
                    cmd.Parameters.Add(new SqlParameter("@idalmacen", SqlDbType.Int)).Value = IdAlmacen;
                    cmd.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error al actualizar cantidad en saldo de artículos: " + ex.Message);
            }
        }
    }
}
