using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Security.RightsManagement;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using static MMC_Software.VentanaRegistroCompras;

namespace MMC_Software
{
    public class RepositorioCompras
    {
        string _ConexionSql;
        public RepositorioCompras(string ConexionSql)
        {
            _ConexionSql = ConexionSql;
        }

        public int GenerarConsecutivoCompras(string CodigoDocumento)
        {
            int CompraID = 0;
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("Sp_ActializarConsecutivo", conn, trans))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codigoDocumento", SqlDbType.NVarChar)).Value = CodigoDocumento;

                        SqlParameter Consecutivo = new SqlParameter("@nuevoconsecutivo", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(Consecutivo);
                        cmd.ExecuteNonQuery();
                        CompraID = Convert.ToInt32(Consecutivo.Value);

                    }
                    trans.Commit();
                    return CompraID;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
        public int InsertCompra(int DocumentoID, int NumeroDocumento, string NumeroFactura, int TerceroID, int? VendedorID,
            DateTime FechaCompra, string DetalleCompra)
        {
            int _CompraID;
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("SP_InsertarCompra", conn, trans))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@documentoid", SqlDbType.Int)).Value = DocumentoID;
                        cmd.Parameters.Add(new SqlParameter("@numerodocumento", SqlDbType.Int)).Value = NumeroDocumento;
                        cmd.Parameters.Add(new SqlParameter("@numerofactura", SqlDbType.NVarChar)).Value = NumeroFactura;
                        cmd.Parameters.Add(new SqlParameter("@Terceroid", SqlDbType.Int)).Value = (int)TerceroID;
                        cmd.Parameters.Add(new SqlParameter("@Vendedorid", SqlDbType.Int)).Value = (int)VendedorID;
                        cmd.Parameters.Add(new SqlParameter("@fechacompra", SqlDbType.Date)).Value = FechaCompra;
                        cmd.Parameters.Add(new SqlParameter("@detallecompra", SqlDbType.NVarChar, 500)).Value = DetalleCompra;

                        SqlParameter CompraID = new SqlParameter("@CompraID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(CompraID);
                        cmd.ExecuteNonQuery();
                        _CompraID = Convert.ToInt32(CompraID.Value);
                    }

                    trans.Commit();
                    return _CompraID;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("ERROR" + ex.ToString(), "MENSAJE", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw;
                }
            }
        }

        public int InsertaCompraDetalle(int CompraID, int ArticuloID, int AlmacenID, decimal Cantidad, decimal CostoAnterior, decimal CostoSinIva,
            decimal CostoConIva, decimal IvaCompra, string Detalle, decimal PorDescu, decimal VrDescuento, decimal CantDev)
        {
            int CompraDetalleID = 0;
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                SqlTransaction Trans = conn.BeginTransaction();

                try
                {
                    string ConsultaDetalle = @"INSERT INTO InveComprasDetalle
                                            (CompraID, ArticulosID, IdAlmacen, Cantidad, CostoAnterior,
                                             CostoSinIva, CostoMasIva, IvaCompra, DetalleArticulo,
                                             PorcetajeDesct, ValorDesct, CantDevuleta)
                                            VALUES
                                            (@compraid, @articuloid, @almacenid, @cantidad, @costoanterior,
                                             @costosiniva, @costomasiva, @ivacompra, @detalle,
                                             @pordesc, @vrdescu, @cantdev)

                                             SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(ConsultaDetalle, conn, Trans))
                    {
                        cmd.Parameters.Add(new SqlParameter("@compraid", SqlDbType.Int)).Value = CompraID;
                        cmd.Parameters.Add(new SqlParameter("@articuloid", SqlDbType.Int)).Value = ArticuloID;
                        cmd.Parameters.Add(new SqlParameter("@almacenid", SqlDbType.Int)).Value = AlmacenID;
                        cmd.Parameters.Add(new SqlParameter("@cantidad", SqlDbType.Decimal)).Value = Cantidad;
                        cmd.Parameters.Add(new SqlParameter("@costoanterior", SqlDbType.Decimal)).Value = CostoAnterior;
                        cmd.Parameters.Add(new SqlParameter("@costosiniva", SqlDbType.Decimal)).Value = CostoSinIva;
                        cmd.Parameters.Add(new SqlParameter("@costomasiva", SqlDbType.Decimal)).Value = CostoConIva;
                        cmd.Parameters.Add(new SqlParameter("@ivacompra", SqlDbType.Decimal)).Value = IvaCompra;
                        cmd.Parameters.Add(new SqlParameter("@detalle", SqlDbType.NVarChar, 200)).Value =
                            string.IsNullOrEmpty(Detalle) ? (object)DBNull.Value : Detalle;
                        cmd.Parameters.Add(new SqlParameter("@pordesc", SqlDbType.Decimal)).Value = PorDescu;
                        cmd.Parameters.Add(new SqlParameter("@vrdescu", SqlDbType.Decimal)).Value = VrDescuento;
                        cmd.Parameters.Add(new SqlParameter("@cantdev", SqlDbType.Decimal)).Value = CantDev;

                        object Resultado = cmd.ExecuteScalar();
                        CompraDetalleID = Convert.ToInt32(Resultado);
                    }

                    Trans.Commit();
                    return CompraDetalleID;
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    MessageBox.Show("Error " + ex.ToString());
                }
            }
            return CompraDetalleID;
        }

        public void ActualizarCostoCompraDetalle(decimal CostoSinIva, decimal CostoConIva, decimal CostoAnterior, int CompraDetalleID)
        {
            using (SqlConnection Conn = new SqlConnection(_ConexionSql))
            {
                Conn.Open();
                try
                {
                    string Consulta = @"Update InveComprasDetalle SET
                                    CostoSinIva=@costosiniva,
                                    CostoMasIva=@costomasiva,
                                    CostoAnterior=@costoanterior
                                    WHERE CompraDetalleID=@compradetalleid";
                    using (SqlCommand cmd = new SqlCommand(Consulta, Conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@costosiniva", SqlDbType.Decimal)).Value = CostoSinIva;
                        cmd.Parameters.Add(new SqlParameter("@costomasiva", SqlDbType.Decimal)).Value = CostoConIva;
                        cmd.Parameters.Add(new SqlParameter("@costoanterior", SqlDbType.Decimal)).Value = CostoAnterior;
                        cmd.Parameters.Add(new SqlParameter("@compradetalleid", SqlDbType.Int)).Value = CompraDetalleID;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error " + ex.ToString());
                }
            }
        }

        public void ActualizarCostoArticulos(decimal CostoAnterior, decimal CostoSinIva, decimal CostoConIva, decimal Margen, decimal Incremento,
            decimal PrecioVentaNew, int ArticuloID)
        {
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                try
                {
                    string ConsultaUpdateArt = @"
                                                UPDATE InveArticulos SET
                                                    CostoAnterior = @costoanterior,
                                                    CostoArticuloSinIva = @costosinivanew,
                                                    CostoArticuloMasIva = @costoconivanew,
                                                    ArticulosMargen = @margen,
                                                    ArticulosIncremento = @incremento,
                                                    ArticulosVenta = @venta
                                                WHERE ArticulosID = @articuloid;";

                    using (SqlCommand cmd = new SqlCommand(ConsultaUpdateArt, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@costoanterior", SqlDbType.Decimal)).Value = CostoAnterior;
                        cmd.Parameters.Add(new SqlParameter("@costosinivanew", SqlDbType.Decimal)).Value = CostoSinIva;
                        cmd.Parameters.Add(new SqlParameter("@costoconivanew", SqlDbType.Decimal)).Value = CostoConIva;
                        cmd.Parameters.Add(new SqlParameter("@margen", SqlDbType.Decimal)).Value = Margen;
                        cmd.Parameters.Add(new SqlParameter("@incremento", SqlDbType.Decimal)).Value = Incremento;
                        cmd.Parameters.Add(new SqlParameter("@venta", SqlDbType.Decimal)).Value = PrecioVentaNew;
                        cmd.Parameters.Add(new SqlParameter("@articuloid", SqlDbType.Int)).Value = ArticuloID;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error " + ex.ToString());
                }
            }
        }

        public void InsertaSaldoArticulo(int ArticuloID, int AlmacenID, decimal Cantidad, decimal CostoSinIva)
        {

            using (SqlConnection Conn = new SqlConnection(_ConexionSql))
            {
                Conn.Open();
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

                    using (SqlCommand cmd = new SqlCommand(ConsultaSaldo, Conn))
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
                    MessageBox.Show("Error " + ex.ToString());
                }
            }
        }

        public void InsertarMoviminetoArticulos(DateTime FechaMovimiento, int TipoMovimiento, int DocumentoID, int numdocumento,
            int ArticuloID, int IdAlmacen, decimal Cantidad, decimal CostoUnitario, int ReferenciaOrigen, int? VendedorID, string Observacion)
        {
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                try
                {
                    string Consulta = @"INSERT INTO InveMovInventarios(FechaMovimiento,TipoMovimiento,DocumentoID,NumDocumento,
                                    ArticulosID,IdAlmacen,Cantidad,CostoUnitario,ReferenciaOrigen,VendedorID,Observaciones)
                                    VALUES(@fechamovimiento,@tipomovimiento,@documentoid,@numdocumento,@articulosid,@idalmacen,
                                    @cantidad,@costounitario,@referenciaorigen,@vendedorid,@observaciones)";
                    using (SqlCommand cmd = new SqlCommand(Consulta, conn))
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
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR " + ex.ToString());
                }
            }
        }

        public void ActualizarCantidadSaldo(int TipoProceso, int ArticuloID, decimal CantidadNew, int IdAlmacen)
        {
            //tipo consulta es 
            //1= suma 
            //2= resta
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                string ConsultaSuma = @"UPDATE InveSaldoInvetarios SET 
                                        SaldoActual= SaldoActual + @cantnueva 
                                        where ArticulosID=@articuloid and IdAlmacen=@idalmacen";

                string ConsultaResta = @"UPDATE InveSaldoInvetarios SET 
                                        SaldoActual= SaldoActual - ABS(@cantnueva) 
                                        where ArticulosID=@articuloid and IdAlmacen=@idalmacen";

                string ConsultaFinal = (TipoProceso == 1) ? ConsultaSuma : ConsultaResta;
                try
                {
                    using (SqlCommand cmd = new SqlCommand(ConsultaFinal, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@cantnueva", SqlDbType.Decimal)).Value = CantidadNew;
                        cmd.Parameters.Add(new SqlParameter("@articuloid", SqlDbType.Int)).Value = ArticuloID;
                        cmd.Parameters.Add(new SqlParameter("@idalmacen", SqlDbType.Int)).Value = IdAlmacen;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error " + ex.Message);
                }
            }
        }

        public void ActualizarCantidadCompraDetalleID(int CompraDetalleID, decimal CantidadNew)
        {
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                string Conuslta = @"UPDATE InveComprasDetalle SET Cantidad=@cantidad WHERE CompraDetalleID=@compradetalleid";
                try
                {
                    using (SqlCommand cmd = new SqlCommand(Conuslta, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@cantidad", SqlDbType.Decimal)).Value = CantidadNew;
                        cmd.Parameters.Add(new SqlParameter("@compradetalleid", SqlDbType.Int)).Value = CompraDetalleID;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error " + ex.ToString());
                }
            }
        }

        public void ActualizarCantidadMovInventario(int CompraDetalleID, decimal CantidadNew)
        {
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                string Conuslta = @"UPDATE InveMovInventarios SET Cantidad=@cantidad WHERE ReferenciaOrigen=@compradetalleid";
                try
                {
                    using (SqlCommand cmd = new SqlCommand(Conuslta, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@cantidad", SqlDbType.Decimal)).Value = CantidadNew;
                        cmd.Parameters.Add(new SqlParameter("@compradetalleid", SqlDbType.Int)).Value = CompraDetalleID;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error " + ex.ToString());
                }
            }
        }

        public void InsertCompraTotales(int CompraID, decimal SubTotal, decimal Exento, decimal Gravado19, decimal Gravado5,
            decimal Iva19, decimal Iva5, decimal Descuentos, decimal RteFuente, decimal TotalCompra)
        {
            using (SqlConnection con = new SqlConnection(_ConexionSql))
            {
                con.Open();
                string Insert = @"IF EXISTS (select 1 from InveComprasTotales where CompraID=@compraid)
	                                    Update InveComprasTotales SET 
	                                    SubTotal=@subtotal,
	                                    Exento=@exento,
	                                    Gravado19=@gravado19,
	                                    Gravado5=@gravado5,
	                                    Iva19=@iva19,
	                                    Iva5=@iva5,
	                                    Descuentos=@descuento,
	                                    RteFuente=@rtefuente,
	                                    TotalCompra=@totalcompra WHERE CompraID=@compraid
                                    ELSE 
                                    INSERT INTO InveComprasTotales(CompraID,SubTotal,Exento,Gravado19,Gravado5,Iva19,Iva5,Descuentos,RteFuente,TotalCompra)
                                    VALUES(@compraid,@subtotal,@exento,@gravado19,@gravado5,@iva19,@iva5,@descuento,@rtefuente,@totalcompra)";
                try
                {
                    using (SqlCommand cmd = new SqlCommand(Insert, con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@compraid", SqlDbType.Int)).Value = CompraID;
                        cmd.Parameters.Add(new SqlParameter("@subtotal", SqlDbType.Decimal)).Value = SubTotal;
                        cmd.Parameters.Add(new SqlParameter("@exento", SqlDbType.Decimal)).Value = Exento;
                        cmd.Parameters.Add(new SqlParameter("@gravado19", SqlDbType.Decimal)).Value = Gravado19;
                        cmd.Parameters.Add(new SqlParameter("@gravado5", SqlDbType.Decimal)).Value = Gravado5;
                        cmd.Parameters.Add(new SqlParameter("@iva19", SqlDbType.Decimal)).Value = Iva19;
                        cmd.Parameters.Add(new SqlParameter("@iva5", SqlDbType.Decimal)).Value = Iva5;
                        cmd.Parameters.Add(new SqlParameter("@descuento", SqlDbType.Decimal)).Value = Descuentos;
                        cmd.Parameters.Add(new SqlParameter("@rtefuente", SqlDbType.Decimal)).Value = RteFuente;
                        cmd.Parameters.Add(new SqlParameter("@totalcompra", SqlDbType.Decimal)).Value = TotalCompra;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error " + ex.ToString());
                }
            }
        }

        public void ElimianrRegistrosCompraDetalle(int CompraID, int ArticuloID, decimal CantidadEliminar)
        {
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                try
                {
                    string DeleteCompraDetalle = @"DELETE FROM InveComprasDetalle WHERE CompraID=@compraid AND ArticulosID=@articuloid AND Cantidad=@cantidad";
                    using (SqlCommand cmd = new SqlCommand(DeleteCompraDetalle, conn))
                    {
                        cmd.Parameters.Add("@compraid", SqlDbType.Int).Value = CompraID;
                        cmd.Parameters.Add("@articuloid", SqlDbType.Int).Value = ArticuloID;
                        cmd.Parameters.Add("@cantidad", SqlDbType.Decimal).Value = CantidadEliminar;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error " + ex.ToString());
                }
            }
        }

        public void ActualizarSaldoArticulosEliminados(int ArticuloID, decimal Cantidad)
        {
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                try
                {
                    string RestarSaldoInvetarios = @"UPDATE InveSaldoInvetarios SET 
                                                SaldoActual=SaldoActual - @Cantidad
                                                WHERE ArticulosID=@articulosid";
                    using (SqlCommand cmd = new SqlCommand(RestarSaldoInvetarios, conn))
                    {
                        cmd.Parameters.Add("@articulosid", SqlDbType.Int).Value = ArticuloID;
                        cmd.Parameters.Add("@Cantidad", SqlDbType.Decimal).Value = Cantidad;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error " + ex.ToString());
                }
            }
        }

        public void EliminarMovInventarios(int CompraDetalleID)
        {
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                try
                {
                    string BorrarMovInventarios = @"delete  from InveMovInventarios  where TipoMovimiento=1 and  ReferenciaOrigen=@compradetalleid ";
                    using (SqlCommand cmd = new SqlCommand(BorrarMovInventarios, conn))
                    {
                        cmd.Parameters.Add("@compradetalleid", SqlDbType.Int).Value = CompraDetalleID;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error" + ex.ToString());
                }
            }
        }




        /// Buscador
        /// 

        public DataTable DatosEncabezadoCompra(int CompraID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                try
                {
                    string Query = @"SELECT * FROM VInveCompras WHERE CompraID=@compraid";
                    using (SqlCommand cmd = new SqlCommand(Query, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@compraid", SqlDbType.Int)).Value = CompraID;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error " + ex.ToString());
                }
            }
            return dt;
        }
        public ObservableCollection<ArticulosAgregados> ArticulosBuscados(int CompraID)
        {
            var Art = new ObservableCollection<ArticulosAgregados>();
            using (SqlConnection con = new SqlConnection(_ConexionSql))
            {
                try
                {
                    con.Open();
                    string Query = @"SELECT *
                                FROM VInveCompras VC
                                INNER JOIN VInveComprasDetalle VCD ON VC.CompraID=VCD.CompraID WHERE VC.CompraID=@compraid";
                    using (SqlCommand cmd = new SqlCommand(Query, con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@compraid", SqlDbType.Int)).Value = CompraID;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            int Contador = 0;
                            while (reader.Read())
                            {
                                var Item = new ArticulosAgregados
                                {
                                    ArticulosID = reader.GetInt32(reader.GetOrdinal("ArticulosID")),
                                    CodigoArticulo = reader.GetString(reader.GetOrdinal("CodigoArticulo")),
                                    NombreArticulo = reader.GetString(reader.GetOrdinal("NombreArticulo")),
                                    AlmacenID = reader.GetInt32(reader.GetOrdinal("IdAlmacen")),
                                    CodigoAlmacen = reader.GetString(reader.GetOrdinal("CodigoAlmacen")),
                                    Cantidad = reader.GetDecimal(reader.GetOrdinal("Cantidad")),
                                    IvaCompra = reader.GetDecimal(reader.GetOrdinal("IvaCompra")),
                                    CostoSinIva = reader.GetDecimal(reader.GetOrdinal("CostoSinIva")),
                                    CostoConIVA = reader.GetDecimal(reader.GetOrdinal("CostoMasIva")),
                                    CostoAnterior = reader.GetDecimal(reader.GetOrdinal("CostoSinIva")),
                                    Items = Contador += 1,
                                    CompraDetalleID = reader.GetInt32(reader.GetOrdinal("CompraDetalleID")),
                                    Estado = 1
                                };

                                Art.Add(Item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error " + ex.ToString());
                }

            }

            return Art;
        }

        public bool ElimarCompras(int CompraID)
        {
            bool Resultado = false;
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("SP_EliminarCompra", conn, trans))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@CompraID", SqlDbType.Int)).Value = CompraID;
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                    Resultado = true;
                }

                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("ERROR " + ex.ToString());
                }
            }
            return Resultado;
        }

        public void ActualizarTerceroCompra(int CompraID, int TerceroID)
        {
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                try
                {
                    string Consulta = @"UPDATE InveCompras SET
                                        TercerosID= @terceroid 
                                        WHERE CompraID=@compraid";
                    using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@terceroid", SqlDbType.Int)).Value = TerceroID;
                        cmd.Parameters.Add(new SqlParameter("@compraid", SqlDbType.Int)).Value = CompraID;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error " + ex.ToString());
                }
            }
        }

        public void ActualizarNumeroFacturaCompra(int CompraID, string NumeroFactura)
        {
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                try
                {
                    string Consulta = @"UPDATE InveCompras SET
                                        NumeroFactura= @numerofactura 
                                        WHERE CompraID=@compraid";
                    using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@numerofactura", SqlDbType.NVarChar)).Value = NumeroFactura;
                        cmd.Parameters.Add(new SqlParameter("@compraid", SqlDbType.Int)).Value = CompraID;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error " + ex.ToString());
                }
            }
        }

        public void ActualizarFechaCompra(int CompraID, DateTime FechaCompra)
        {
            using (SqlConnection conn = new SqlConnection(_ConexionSql))
            {
                conn.Open();
                try
                {
                    string Consulta = @"UPDATE InveCompras SET
                                        FechaCompra= @fechacompra 
                                        WHERE CompraID=@compraid";
                    using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@fechacompra", SqlDbType.Date)).Value = FechaCompra;
                        cmd.Parameters.Add(new SqlParameter("@compraid", SqlDbType.Int)).Value = CompraID;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error " + ex.ToString());
                }
            }
        }
    }
}
