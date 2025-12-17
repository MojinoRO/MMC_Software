using CrystalDecisions.Shared;
using MMC_Software;
using System;
using System.Data.SqlClient;
using System.Windows;

public class ServicesTransactionSale
{
    private string _Sqlconnection;

    public ServicesTransactionSale(string Sqlconnection)
    {
        _Sqlconnection = Sqlconnection;
    }

    public int TransacctionInsertSaleDatails(
        VentanaRegistroVentaViewModel.ArticulosVenta Fila,
        VentanaRegistroVentaViewModel vm)
    {
        using (SqlConnection conn = new SqlConnection(_Sqlconnection))
        {
            conn.Open();
            SqlTransaction trans = conn.BeginTransaction();

            try
            {
                var repoVenta = new RepositoryInventoryControl(trans);
                var repoSale = new RepositorySale(trans);

                int VentaDetalleID;

                if (Fila.VentaDetalleID == 0)
                {
                    // INSERTA DETALLE
                    VentaDetalleID = repoSale.InsertarVentaDetalle(
                        vm.VentaID,
                        Fila.ArticuloID,
                        Fila.AlmacenID,
                        Fila.ValorCosto,
                        Fila.IvaArticulo,
                        Fila.TotalSinIva,
                        Fila.TotalConIva,
                        Fila.CantidadVenta,
                        Fila.DetalleArticulo);

                    // Guarda el ID en el objeto
                    Fila.VentaDetalleID = VentaDetalleID;

                    // DISMINUIR SALDO
                    repoVenta.ActualizarCantidadSaldo
                        (2, // salida
                        Fila.ArticuloID,
                        Fila.CantidadVenta,
                        Fila.AlmacenID);

                    // INSERTA MOVIMIENTO
                    repoVenta.InsertarMoviminetoArticulos(
                        vm.SaleDate,
                        2, // salida
                        vm.DocumentoID,
                        vm.NumeroVenta,
                        Fila.ArticuloID,
                        Fila.AlmacenID,
                        Fila.CantidadVenta,
                        Fila.ValorCosto,
                        VentaDetalleID,
                        vm.VendedorID,
                        "Salida por venta");
                }
                else
                {
                    VentaDetalleID = Fila.VentaDetalleID;
                    decimal diferencia = Fila.CantidadVenta - Fila.CantidadOld;

                    if(diferencia > 0)
                    {
                        repoVenta.ActualizarCantidadSaldo
                            (2, // salida
                            Fila.ArticuloID,
                            diferencia,
                            Fila.AlmacenID);
                    }
                    else if(diferencia < 0)
                    {
                        repoVenta.ActualizarCantidadSaldo
                            (1, // entrada
                            Fila.ArticuloID,
                            Math.Abs(diferencia),
                            Fila.AlmacenID);
                    }
                }

                // COMMIT OBLIGATORIO
                trans.Commit();

                // DEVUELVE EL ID REAL
                return VentaDetalleID;
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }
    }
}
