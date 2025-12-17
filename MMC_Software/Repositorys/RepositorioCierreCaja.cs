using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using static MMC_Software.VentanaInfoCierre;

namespace MMC_Software
{
    public class RepositorioCierreCaja
    {

        private readonly string _Conexion;
        public int _ConteoID;

        public RepositorioCierreCaja(string conexion)
        {
            _Conexion = conexion;
        }

        public void InsertarConteoCierre(int VendedorID, DateTime Fecha, int b100, int b50, int b20, int b10, int b5, int b2,
                                         int m1000, int m500, int m200, int m100, int m50)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                using (SqlCommand cmd = new SqlCommand("Sp_InsertaConteoCierre", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@VendedorID", SqlDbType.Int)).Value = VendedorID;
                    cmd.Parameters.Add(new SqlParameter("@Fecha", SqlDbType.Date)).Value = Fecha;
                    cmd.Parameters.Add(new SqlParameter("@b100", SqlDbType.Int)).Value = b100;
                    cmd.Parameters.Add(new SqlParameter("@b50", SqlDbType.Int)).Value = b50;
                    cmd.Parameters.Add(new SqlParameter("@b20", SqlDbType.Int)).Value = b20;
                    cmd.Parameters.Add(new SqlParameter("@b10", SqlDbType.Int)).Value = b10;
                    cmd.Parameters.Add(new SqlParameter("@b5", SqlDbType.Int)).Value = b5;
                    cmd.Parameters.Add(new SqlParameter("@b2", SqlDbType.Int)).Value = b2;
                    cmd.Parameters.Add(new SqlParameter("@m1000", SqlDbType.Int)).Value = m1000;
                    cmd.Parameters.Add(new SqlParameter("@m500", SqlDbType.Int)).Value = m500;
                    cmd.Parameters.Add(new SqlParameter("@m200", SqlDbType.Int)).Value = m200;
                    cmd.Parameters.Add(new SqlParameter("@m100", SqlDbType.Int)).Value = m100;
                    cmd.Parameters.Add(new SqlParameter("@m50", SqlDbType.Int)).Value = m50;

                    var ConteoID = new SqlParameter("@NewConteoID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(ConteoID);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void ActualizarConteoCiere(int ConteoID, int b100, int b50, int b20, int b10, int b5, int b2,
                                         int m1000, int m500, int m200, int m100, int m50)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string update = @"update ConfConteoCierreDetalle set
                                Bill100=@b100,
                                Bill50=@b50,
                                Bill20=@b20,
                                Bill10=@b10,
                                Bill5=@b5,
                                Bill2=@b2,
                                Mon1000=@m1000,
                                Mon500=@m500,
                                Mon200=@m200,
                                Mon100=@m100,
                                Mon50=@m50
                                where ConteoID=@conteoid;";
                using (SqlCommand cmd = new SqlCommand(update, conn))
                {
                    cmd.Parameters.AddWithValue("@b100", b100);
                    cmd.Parameters.AddWithValue("@b50", b50);
                    cmd.Parameters.AddWithValue("@b20", b20);
                    cmd.Parameters.AddWithValue("@b10", b10);
                    cmd.Parameters.AddWithValue("@b5", b5);
                    cmd.Parameters.AddWithValue("@b2", b2);
                    cmd.Parameters.AddWithValue("@m1000", m1000);
                    cmd.Parameters.AddWithValue("@m500", m500);
                    cmd.Parameters.AddWithValue("@m200", m200);
                    cmd.Parameters.AddWithValue("@m100", m100);
                    cmd.Parameters.AddWithValue("@m50", m50);
                    cmd.Parameters.AddWithValue("@conteoid", ConteoID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public DataTable ValidaExisteConteo(int VendedorID, DateTime Fecha)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select COUNT(*) from ConfConteoCierre where VendedorID=@vendedorid and Fecha=@fecha";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("VendedorID", VendedorID);
                    cmd.Parameters.Add(new SqlParameter("@fecha", SqlDbType.Date)).Value = Fecha;
                    int existe = Convert.ToInt32(cmd.ExecuteScalar());
                    if (existe > 0)
                    {
                        string ConsultaID = @"select ConteoID from ConfConteoCierre where VendedorID=@vendedorid and Fecha=@fecha";
                        using (SqlCommand cmd2 = new SqlCommand(ConsultaID, conn))
                        {
                            cmd2.Parameters.Add(new SqlParameter("@vendedorid", SqlDbType.Int)).Value = VendedorID;
                            cmd2.Parameters.Add(new SqlParameter("@fecha", SqlDbType.Date)).Value = Fecha;
                            using (SqlDataAdapter adp = new SqlDataAdapter(cmd2))
                            {
                                DataTable dt = new DataTable();
                                adp.Fill(dt);
                                if (dt.Rows.Count > 0)
                                {
                                    _ConteoID = Convert.ToInt32(dt.Rows[0]["ConteoID"]);
                                    DataTable dt2 = LlenarConetoCaja(_ConteoID);
                                    return dt2;
                                }
                            }
                        }
                    }
                }
            }
            return new DataTable();
        }

        public DataTable LlenarConetoCaja(int conteoID)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string consulta = @"select ConteoID,Bill100,Bill50,Bill20,Bill10,Bill5,Bill2,Mon1000,Mon500,Mon200,Mon100,Mon50
	                                from ConfConteoCierreDetalle where ConteoID=@conteoid";
                using (SqlCommand cmd = new SqlCommand(consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@conteoid", conteoID);
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        return dt;
                    }
                }
            }
        }


        //=============================================================================
        //               LLENAR INFO DEL CIERRE
        //=============================================================================

        public ObservableCollection<RelacionFacturas> Facturas(int VendedorID, DateTime FechaCierre)
        {
            var Facturas = new ObservableCollection<RelacionFacturas>();
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string consulta = @"SELECT 
	                                CONCAT(D.CodigoDocumento,' ',D.NombreDocumento) AS Documento,
	                                CONCAT(D.CodigoDocumento,' ',D.NombreDocumento) AS Documento,
	                                CAST(MIN(V.NumeroDocumento)As int) AS NumeroInical,
	                                CAST(MAX(V.NumeroDocumento)AS int) AS NumeroFinal,
	                                COUNT(V.NumeroDocumento) AS CantidadFacturas
                                    FROM InveVentas V
                                    INNER JOIN ConfDocumentos D ON V.DocumentoID=D.DocumentoID
                                    INNER JOIN ConfVendedores VD ON V.VendedorID=VD.VendedorID
                                    WHERE V.Fecha=@fecha  AND  VD.VendedorID=@vendedorid
                                    GROUP BY 
	                                    VD.CodigoVendedor,VD.NombreVendedor ,D.CodigoDocumento ,D.NombreDocumento
                                        ORDER BY
	                                        VD.NombreVendedor";
                using (SqlCommand cmd = new SqlCommand(consulta, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@fecha", SqlDbType.Date)).Value = FechaCierre;
                    cmd.Parameters.Add(new SqlParameter("@vendedorid", SqlDbType.Int)).Value = VendedorID;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Facturas.Add(new RelacionFacturas
                            {
                                Docto = reader.GetString(reader.GetOrdinal("Documento")),
                                NumDesde = Convert.ToInt32(reader.GetInt32(reader.GetOrdinal("NumeroInical"))),
                                NumHasta = Convert.ToInt32(reader.GetInt32(reader.GetOrdinal("NumeroFinal"))),
                                Cantidad = Convert.ToInt32(reader.GetInt32(reader.GetOrdinal("CantidadFacturas")))
                            });
                        }
                    }
                }
            }
            return Facturas;
        }

        public ObservableCollection<RelacionImpuesto> Impuestos(int VendedorID, DateTime Fecha)
        {
            var facturas = new ObservableCollection<RelacionImpuesto>();
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"SELECT Tarifa,
		                                    SUM(Base) As Base,
		                                    SUM(Iva) As Iva,
		                                    SUM(Base+Iva) As Total
                                    FROM (SELECT '19%' AS Tarifa,
		                                    V.Gravado19 As Base,
		                                    V.Iva19 As Iva
 	                                      FROM InveVentasTotales V
	                                      INNER JOIN InveVentas VE ON V.VentaID=VE.VentaID
	                                      WHERE VE.VendedorID=@vendedorid AND Fecha=@fecha

	                                      UNION ALL

	                                      SELECT  '5%' AS Tarifa,
	                                      V.Gravado5 AS Base,
	                                      V.Iva5 AS Iva
	                                      FROM InveVentasTotales V
	                                      INNER JOIN InveVentas VE ON V.VentaID=VE.VentaID
	                                      WHERE VE.VendedorID=@vendedorid AND Fecha=@fecha

	                                      UNION ALL

	                                      SELECT 'Exento' AS Tarifa,
	                                      V.Exento Tarifa,
	                                      0 AS Iva
	                                      FROM InveVentasTotales V
	                                      INNER JOIN InveVentas VE ON VE.VentaID=V.VentaID
	                                      WHERE VE.VendedorID=@vendedorid AND Fecha=@fecha
	                                    ) AS T

	                                    GROUP BY Tarifa
	                                    ORDER BY Tarifa";

                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@fecha", SqlDbType.Date)).Value = Fecha;
                    cmd.Parameters.Add(new SqlParameter("@vendedorid", SqlDbType.Int)).Value = VendedorID;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            facturas.Add(new RelacionImpuesto
                            {
                                Tarifa = reader.GetString(reader.GetOrdinal("Tarifa")),
                                Base = reader.GetDecimal(reader.GetOrdinal("Base")),
                                Impuesto = reader.GetDecimal(reader.GetOrdinal("Iva")),
                                BaseMasImpuesto = reader.GetDecimal(reader.GetOrdinal("Total"))
                            });
                        }
                    }
                }
            }
            return facturas;
        }

        public ObservableCollection<RelacionCategorias> Categorias(int VendedorID, DateTime Fecha)
        {
            var Categorias = new ObservableCollection<RelacionCategorias>();
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"SELECT 
	                                    CONCAT(CA.CodigoCategoria,' ',CA.NombreCategoria) AS Categoria,
	                                    cast(SUM(VD.Cantidad) AS int )AS Cantidad
                                    FROM  InveVentasDetalle VD
                                    INNER JOIN InveVentas VE ON VD.VentaID=VE.VentaID
                                    INNER JOIN InveArticulos AR ON AR.ArticulosID=VD.ArticuloID
                                    INNER JOIN ConfCategoriasInve CA ON AR.CategoriasID=CA.CategoriasID
                                    WHERE VE.VendedorID=@vendedorid AND VE.Fecha=@fecha
                                    GROUP BY 
	                                    CA.CodigoCategoria, CA.NombreCategoria
                                    ORDER BY 
	                                    CA.CodigoCategoria";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@vendedorid", SqlDbType.Int)).Value = VendedorID;
                    cmd.Parameters.Add(new SqlParameter("@fecha", SqlDbType.Date)).Value = Fecha;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Categorias.Add(new RelacionCategorias
                            {
                                Categoria = reader.GetString(reader.GetOrdinal("Categoria")),
                                Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad"))
                            });
                        }
                    }
                }
            }

            return Categorias;
        }

        public ObservableCollection<RelacionFormasPago> FormasPago(int VendedorID, DateTime Fecha)
        {
            var FormasPago = new ObservableCollection<RelacionFormasPago>();
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"SELECT
	                                    CONCAT(F.CodigoFormaPago,' ',F.NombreFormaPago) AS FormaPago,
	                                    COUNT(G.FormaPagoID) AS Cantidad,
	                                    CAST(sum(G.ValorPagado)AS int )As ValorPagado
                                    FROM ConfGuardaPagos G
                                    INNER JOIN ConfFormasPago F ON G.FormaPagoID=F.FormaPagoID WHERE VendedorID=@vendedorid
                                    AND  G.Fecha=@fecha
                                    GROUP BY
	                                    F.CodigoFormaPago,F.NombreFormaPago";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@vendedorid", SqlDbType.Int)).Value = VendedorID;
                    cmd.Parameters.Add(new SqlParameter("@fecha", SqlDbType.Date)).Value = Fecha;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FormasPago.Add(new RelacionFormasPago
                            {
                                FormaPago = reader.GetString(reader.GetOrdinal("FormaPago")),
                                Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                                ValorTotal = reader.GetInt32(reader.GetOrdinal("ValorPagado"))
                            });
                        }
                    }
                }
            }

            return FormasPago;
        }

        public int LlenarBaseCajaConteo(int VendedorID)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"SELECT CAST(BaseCaja AS int) FROM  ConfOpcionesVendedores WHERE VendedorID=@vendedorid";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@vendedorid", SqlDbType.Int)).Value = VendedorID;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int ValorBase = reader.GetInt32(0);
                            return ValorBase;
                        }
                    }
                }
            }
            return 0;
        }

        public bool InsertarRetirosCaja(int VendedorID, decimal ValorAnticipo, string Detalle, DateTime Fecha, TimeSpan Hora)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Insert = @"INSERT INTO ConfRetirosCaja(VendedorID,ValorRetiro,DetalleRetiro,FechaRetiro,HoraRetiro) values
                                    (@vendedorid,@valor,@detalle,@fecha,@hora)";
                using (SqlCommand cmd = new SqlCommand(Insert, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@vendedorid", SqlDbType.Int)).Value = VendedorID;
                    cmd.Parameters.Add(new SqlParameter("@valor", SqlDbType.Money)).Value = ValorAnticipo;
                    cmd.Parameters.Add(new SqlParameter("@detalle", SqlDbType.NVarChar)).Value = Detalle;
                    cmd.Parameters.Add(new SqlParameter("@fecha", SqlDbType.Date)).Value = Fecha;
                    cmd.Parameters.Add(new SqlParameter("@hora", SqlDbType.Time)).Value = Hora;
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }

        public DataTable llenarRetirosCaja(int VendedorID, DateTime Fecha)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string ValidaExiste = @"select COUNT(*) from ConfRetirosCaja where VendedorID=@vendedorid And FechaRetiro=@fecha";
                using (SqlCommand cmd = new SqlCommand(ValidaExiste, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@vendedorid", SqlDbType.Int)).Value = VendedorID;
                    cmd.Parameters.Add(new SqlParameter("@fecha", SqlDbType.Date)).Value = Fecha;
                    int Existe = Convert.ToInt32(cmd.ExecuteScalar());
                    if (Existe > 0)
                    {
                        string Datos = @"SELECT CAST(SUM(ValorRetiro) AS int) AS ValorCierre,
	                                   COUNT(ValorRetiro)AS Cantidad
                                       FROM  ConfRetirosCaja where VendedorID=@vendedorid AND FechaRetiro=@fecha";
                        using (SqlCommand cmd2 = new SqlCommand(Datos, conn))
                        {
                            cmd2.Parameters.AddWithValue("@vendedorid", VendedorID);
                            cmd2.Parameters.AddWithValue("@fecha", Fecha);
                            using (SqlDataAdapter adp = new SqlDataAdapter(cmd2))
                            {
                                DataTable dt = new DataTable();
                                adp.Fill(dt);
                                return dt;
                            }
                        }
                    }
                }
            }
            return new DataTable();
        }

        //==================================================================================
        //                              INSERT CIERRE
        //==================================================================================

        public int InsertarCierreCaja(string vendeodor, DateTime Fecha, decimal BaseEntregada, decimal ConteoCaja, decimal TotalVenta,
            decimal EfectivoEntregar, decimal RetirosCaja, decimal Estado, List<RelacionFacturas> Facturas,
            List<RelacionFormasPago> Formaspago, List<RelacionImpuesto> impuestos, List<RelacionCategorias> Categorias)
        {
            int _CierreID;
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                SqlTransaction Trans = conn.BeginTransaction();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("SP_InsertarCierreCaja", conn, Trans))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@vendedor", SqlDbType.NVarChar)).Value = vendeodor;
                        cmd.Parameters.Add(new SqlParameter("@fechacierre", SqlDbType.Date)).Value = Fecha;
                        cmd.Parameters.Add(new SqlParameter("@basecaja", SqlDbType.Money)).Value = BaseEntregada;
                        cmd.Parameters.Add(new SqlParameter("@ConteoCaja", SqlDbType.Money)).Value = ConteoCaja;
                        cmd.Parameters.Add(new SqlParameter("@totalventa", SqlDbType.Money)).Value = TotalVenta;
                        cmd.Parameters.Add(new SqlParameter("@efectivoentregar", SqlDbType.Money)).Value = EfectivoEntregar;
                        cmd.Parameters.Add(new SqlParameter("@retiroscaja", SqlDbType.Decimal)).Value = RetirosCaja;
                        cmd.Parameters.Add(new SqlParameter("@estadocaja", SqlDbType.Money)).Value = Estado;

                        SqlParameter CierreID = new SqlParameter("CierreID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(CierreID);
                        cmd.ExecuteNonQuery();
                        _CierreID = Convert.ToInt32(CierreID.Value);
                    }

                    foreach (var FP in Formaspago)
                    {
                        using (SqlCommand cmd = new SqlCommand("SP_InsertaCierreFormasPago", conn, Trans))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@CierreID", SqlDbType.Int)).Value = _CierreID;
                            cmd.Parameters.Add(new SqlParameter("@FormaPago", SqlDbType.NVarChar)).Value = FP.FormaPago;
                            cmd.Parameters.Add(new SqlParameter("@Cantidad", SqlDbType.Int)).Value = FP.Cantidad;
                            cmd.Parameters.Add(new SqlParameter("@ValorTotal", SqlDbType.Money)).Value = FP.ValorTotal;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    foreach (var I in impuestos)
                    {
                        using (SqlCommand cmd = new SqlCommand("SP_InsertaCierreImpuestos", conn, Trans))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@CierreID", SqlDbType.Int)).Value = _CierreID;
                            cmd.Parameters.Add(new SqlParameter("@Tarifa", SqlDbType.NVarChar)).Value = I.Tarifa;
                            cmd.Parameters.Add(new SqlParameter("@Base", SqlDbType.Money)).Value = I.Base;
                            cmd.Parameters.Add(new SqlParameter("@Impuesto", SqlDbType.Money)).Value = I.Impuesto;
                            cmd.Parameters.Add(new SqlParameter("@Total", SqlDbType.Money)).Value = I.BaseMasImpuesto;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    foreach (var C in Categorias)
                    {
                        using (SqlCommand cmd = new SqlCommand("SP_InsertaCierreCategorias", conn, Trans))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@CierreID", SqlDbType.Int)).Value = _CierreID;
                            cmd.Parameters.Add(new SqlParameter("@Categorias", SqlDbType.NVarChar)).Value = C.Categoria;
                            cmd.Parameters.Add(new SqlParameter("@Cantidad", SqlDbType.Int)).Value = C.Cantidad;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    foreach (var F in Facturas)
                    {
                        using (SqlCommand cmd = new SqlCommand("SP_InsertaCierreFacturas", conn, Trans))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@cierreID", SqlDbType.Int)).Value = _CierreID;
                            cmd.Parameters.Add(new SqlParameter("@documento", SqlDbType.NVarChar)).Value = F.Docto;
                            cmd.Parameters.Add(new SqlParameter("@numeroDesde", SqlDbType.Int)).Value = F.NumDesde;
                            cmd.Parameters.Add(new SqlParameter("@numeroHasta", SqlDbType.Int)).Value = F.NumHasta;
                            cmd.Parameters.Add(new SqlParameter("@cantidadRegistros", SqlDbType.Int)).Value = F.Cantidad;

                            cmd.ExecuteNonQuery();
                        }

                    }

                    Trans.Commit();
                    return _CierreID;
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    MessageBox.Show("ERROR " + ex.ToString(), "MENSAJE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            return 0;
        }
    }
}
