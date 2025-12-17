using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static MMC_Software.ModuloPuntoVenta;
using static MMC_Software.VentanaBuscarDocumentos;

namespace MMC_Software
{
    public class RepositorioBuscarDocumentos
    {
        private readonly string _Conexion;

        public RepositorioBuscarDocumentos(string Conexion)
        {
            _Conexion = Conexion;
        }

        public DataTable LLenarDocumento(string Filtro)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select CONCAT(CodigoDocumento, ' ', NombreDocumento) as Documento, DocumentoID  from ConfDocumentos
                                    where CodigoDocumento like @filtro OR NombreDocumento like @filtro and TipoDocumento=2";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@filtro", "%" + Filtro + "%");
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public DataTable BuscarCCNit(string Cedula)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select TercerosTipoDocumento,TercerosIdentificacion,TercerosNombreCompleto , TerceroRazonSocial ,TercerosID from  InveTerceros where TercerosIdentificacion=@filtro";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@filtro", Cedula);
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public List<LlenarFacturasBuscadas> ObtenerVentasPorFechas(DateTime Desde, DateTime Hasta)
        {
            var ventas = new List<LlenarFacturasBuscadas>();
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select ven.VentaID,doc.CodigoDocumento as Docto, Ven.NumeroDocumento as NumeroDocto,Ven.Fecha as fecha,
                                    ter.TercerosIdentificacion as NitCC,ter.TercerosEstablecimiento as Nombres,
                                    (select COUNT(*) from InveVentasDetalle d where d.VentaID=Ven.VentaID ) as Items ,vent.TotalVenta as TotalVenta,
									(select Form.NombreFormaPago from ConfFormasPago Form 
									inner join ConfGuardaPagos pago on pago.FormaPagoID = Form.FormaPagoID and pago.DocumentoID=doc.DocumentoID and pago.NumeroDocumento=Ven.NumeroDocumento)
									as FormaPago
                                    from InveVentas Ven
                                    inner join ConfDocumentos doc on Ven.DocumentoID=doc.DocumentoID
                                    inner join InveTerceros ter on Ven.TercerosID=ter.TercerosID
                                    inner Join InveVentasTotales vent on Ven.VentaID=vent.VentaID 
                                    and  Ven.Fecha>=@desde and Ven.Fecha<=@hasta ORDER BY Ven.NumeroDocumento DESC";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@desde", Desde);
                    cmd.Parameters.AddWithValue("@hasta", Hasta);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ventas.Add(new LlenarFacturasBuscadas
                            {
                                VentaID = reader.GetInt32(reader.GetOrdinal("VentaID")),
                                Docto = reader.GetString(reader.GetOrdinal("Docto")),
                                NumeroDocto = Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("NumeroDocto"))),
                                Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                                NitCC = reader.GetString(reader.GetOrdinal("NitCC")),
                                Nombres = reader.GetString(reader.GetOrdinal("Nombres")),
                                Items = reader.GetInt32(reader.GetOrdinal("Items")),
                                TotalVenta = reader.GetDecimal(reader.GetOrdinal("TotalVenta")),
                                FormaPago = reader.GetString(reader.GetOrdinal("FormaPago"))
                            });
                        }
                    }
                }
            }
            return ventas;
        }

        public List<LlenarFacturasBuscadas> obtenerVentasXDoctoNumero(int? DocumentoID, int? NumeroDocto)
        {
            var ventas = new List<LlenarFacturasBuscadas>();

            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select ven.VentaID as VentaID,doc.CodigoDocumento as Docto, Ven.NumeroDocumento as NumeroDocto,Ven.Fecha as fecha,
                                    ter.TercerosIdentificacion as NitCC,ter.TercerosEstablecimiento as Nombres,
                                    (select COUNT(*) from InveVentasDetalle d where d.VentaID=Ven.VentaID ) as Items ,vent.TotalVenta as TotalVenta,
									(select Form.NombreFormaPago from ConfFormasPago Form 
									inner join ConfGuardaPagos pago on pago.FormaPagoID = Form.FormaPagoID and pago.DocumentoID=doc.DocumentoID and pago.NumeroDocumento=Ven.NumeroDocumento)
									as FormaPago
                                    from InveVentas Ven
                                    inner join ConfDocumentos doc on Ven.DocumentoID=doc.DocumentoID
                                    inner join InveTerceros ter on Ven.TercerosID=ter.TercerosID
                                    inner Join InveVentasTotales vent on Ven.VentaID=vent.VentaID 
                                    where doc.DocumentoID=@filtrodoc and Ven.NumeroDocumento=@filtroNumero";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@filtrodoc", DocumentoID);
                    cmd.Parameters.AddWithValue("@filtroNumero", NumeroDocto);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ventas.Add(new LlenarFacturasBuscadas
                            {
                                VentaID = reader.GetInt32(reader.GetOrdinal("VentaID")),
                                Docto = reader.GetString(reader.GetOrdinal("Docto")),
                                NumeroDocto = Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("NumeroDocto"))),
                                Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                                NitCC = reader.GetString(reader.GetOrdinal("NitCC")),
                                Nombres = reader.GetString(reader.GetOrdinal("Nombres")),
                                Items = reader.GetInt32(reader.GetOrdinal("Items")),
                                TotalVenta = reader.GetDecimal(reader.GetOrdinal("TotalVenta")),
                                FormaPago = reader.GetString(reader.GetOrdinal("FormaPago"))
                            });
                        }
                    }
                }
            }
            return ventas;
        }

        public List<LlenarFacturasBuscadas> ObtenerVentasXFechasTerceros(DateTime Desde, DateTime Hasta, int? TerceroID)
        {
            var ventas = new List<LlenarFacturasBuscadas>();
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select ven.VentaID as VentaID,doc.CodigoDocumento as Docto, Ven.NumeroDocumento as NumeroDocto,Ven.Fecha as fecha,
                                    ter.TercerosIdentificacion as NitCC,ter.TercerosEstablecimiento as Nombres,
                                    (select COUNT(*) from InveVentasDetalle d where d.VentaID=Ven.VentaID ) as Items ,vent.TotalVenta as TotalVenta,
									(select Form.NombreFormaPago from ConfFormasPago Form 
									inner join ConfGuardaPagos pago on pago.FormaPagoID = Form.FormaPagoID and pago.DocumentoID=doc.DocumentoID and pago.NumeroDocumento=Ven.NumeroDocumento)
									as FormaPago
                                    from InveVentas Ven
                                    inner join ConfDocumentos doc on Ven.DocumentoID=doc.DocumentoID
                                    inner join InveTerceros ter on Ven.TercerosID=ter.TercerosID
                                    inner Join InveVentasTotales vent on Ven.VentaID=vent.VentaID 
                                    and  Ven.Fecha>=@desde and Ven.Fecha<=@hasta and ter.TercerosID=@tercero ORDER BY Ven.NumeroDocumento DESC";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@desde", Desde);
                    cmd.Parameters.AddWithValue("@hasta", Hasta);
                    cmd.Parameters.AddWithValue("@tercero", TerceroID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ventas.Add(new LlenarFacturasBuscadas
                            {
                                VentaID = reader.GetInt32(reader.GetOrdinal("VentaID")),
                                Docto = reader.GetString(reader.GetOrdinal("Docto")),
                                NumeroDocto = Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("NumeroDocto"))),
                                Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                                NitCC = reader.GetString(reader.GetOrdinal("NitCC")),
                                Nombres = reader.GetString(reader.GetOrdinal("Nombres")),
                                Items = reader.GetInt32(reader.GetOrdinal("Items")),
                                TotalVenta = reader.GetDecimal(reader.GetOrdinal("TotalVenta")),
                                FormaPago = reader.GetString(reader.GetOrdinal("FormaPago"))
                            });
                        }
                    }
                }
            }

            return ventas;
        }

        public List<LlenarFacturasBuscadas> obtenerFacturasXDoctoXRangoFechasXTerceros(DateTime Desde, DateTime Hasta, int? DocumentoID, int? TerceroID)
        {
            var ventas = new List<LlenarFacturasBuscadas>();
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select ven.VentaID as VentaID,doc.CodigoDocumento as Docto, Ven.NumeroDocumento as NumeroDocto,Ven.Fecha as fecha,
                                    ter.TercerosIdentificacion as NitCC,ter.TercerosEstablecimiento as Nombres,
                                    (select COUNT(*) from InveVentasDetalle d where d.VentaID=Ven.VentaID ) as Items ,vent.TotalVenta as TotalVenta,
									(select Form.NombreFormaPago from ConfFormasPago Form 
									inner join ConfGuardaPagos pago on pago.FormaPagoID = Form.FormaPagoID and
									pago.DocumentoID=doc.DocumentoID and pago.NumeroDocumento=Ven.NumeroDocumento)as FormaPago
                                    from InveVentas Ven
                                    inner join ConfDocumentos doc on Ven.DocumentoID=doc.DocumentoID
                                    inner join InveTerceros ter on Ven.TercerosID=ter.TercerosID
                                    inner Join InveVentasTotales vent on Ven.VentaID=vent.VentaID 
                                    and  Ven.Fecha>=@desde and Ven.Fecha<=@hasta and ter.TercerosID=@terceroid and doc.DocumentoID=@docto
									ORDER BY Ven.NumeroDocumento DESC";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@desde", Desde);
                    cmd.Parameters.AddWithValue("hasta", Hasta);
                    cmd.Parameters.AddWithValue("@terceroid", TerceroID);
                    cmd.Parameters.AddWithValue("@docto", DocumentoID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ventas.Add(new LlenarFacturasBuscadas
                            {
                                VentaID = reader.GetInt32(reader.GetOrdinal("VentaID")),
                                Docto = reader.GetString(reader.GetOrdinal("Docto")),
                                NumeroDocto = Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("NumeroDocto"))),
                                Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                                NitCC = reader.GetString(reader.GetOrdinal("NitCC")),
                                Nombres = reader.GetString(reader.GetOrdinal("Nombres")),
                                Items = reader.GetInt32(reader.GetOrdinal("Items")),
                                TotalVenta = reader.GetDecimal(reader.GetOrdinal("TotalVenta")),
                                FormaPago = reader.GetString(reader.GetOrdinal("FormaPago"))
                            });
                        }
                    }
                }
            }
            return ventas;
        }

        //==============================================================================
        //                              LLENEDO REIMPRIMIR
        //==============================================================================

        public DataTable LlenarDatosEncabezado(int VentaID)
        {
            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string Consulta = @"select  ven.VentaID,Doc.CodigoDocumento , ven.NumeroDocumento ,ter.TercerosTipoDocumento,ter.TercerosIdentificacion,ter.TercerosNombreCompleto,
									ter.TerceroRazonSocial,CONCAT(vend.CodigoVendedor,' ',vend.NombreVendedor) AS Vendedor,
                                    ven.Fecha,ven.Detalle
                                    from  InveVentas ven 
                                    inner join ConfDocumentos Doc on ven.DocumentoID= Doc.DocumentoID
                                    inner join InveTerceros ter on ven.TercerosID=ter.TercerosID
									inner join ConfVendedores vend on ven.VendedorID=vend.VendedorID and ven.VentaID=@ventaid";
                using (SqlCommand cmd = new SqlCommand(Consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@ventaid", VentaID);
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public List<ArticulosAgregados> LLenarListadoArticulos(int VentaID)
        {
            var ventaBuscada = new List<ArticulosAgregados>();

            using (SqlConnection conn = new SqlConnection(_Conexion))
            {
                conn.Open();
                string ConsultaFactura = @"select art.ArticulosID,art.CodigoArticulo,art.NombreArticulo,alm.CodigoAlmacen,vend.Cantidad,vend.CostoSinIva,vend.IvaArticulo,
                                            vend.VentaSinIva,vend.VentaMasIva,vend.DetalleArticulo
                                            from  InveVentas ven 
                                            inner join ConfDocumentos Doc on ven.DocumentoID= Doc.DocumentoID
                                            inner join InveTerceros ter on ven.TercerosID=ter.TercerosID
                                            inner join InveVentasDetalle vend on ven.VentaID = vend.VentaID
                                            inner join ConfAlmacen alm on vend.AlmacenID =alm.IdAlmacen
                                            inner join InveArticulos art on vend.ArticuloID= art.ArticulosID and ven.VentaID=@ventaid";
                using (SqlCommand cmd = new SqlCommand(ConsultaFactura, conn))
                {
                    cmd.Parameters.AddWithValue("@ventaid", VentaID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ventaBuscada.Add(new ArticulosAgregados
                            {
                                ArticuloID = reader.GetInt32(reader.GetOrdinal("ArticulosID")),
                                Codigo = reader.GetString(reader.GetOrdinal("CodigoArticulo")),
                                Nombre = reader.GetString(reader.GetOrdinal("DetalleArticulo")),
                                AlmacenVenta = reader.GetString(reader.GetOrdinal("CodigoAlmacen")),
                                CantidadVenta = reader.GetDecimal(reader.GetOrdinal("Cantidad")),
                                IVA = reader.GetDecimal(reader.GetOrdinal("IvaArticulo")),
                                PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("VentaSinIva")),
                                PrecioTotal = reader.GetDecimal(reader.GetOrdinal("VentaMasIva")),
                                CostoSinIva = reader.GetDecimal(reader.GetOrdinal("CostoSinIva"))
                            });
                        }
                    }
                }

            }
            return ventaBuscada;
        }

    }
}
