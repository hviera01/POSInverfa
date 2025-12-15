using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Reporte
    {
        public List<ReporteCompra> Compra(string fechainicio, string fechafin, int idproveedor)
        {
            List<ReporteCompra> lista = new List<ReporteCompra>();

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_ReporteCompras", oconexion);
                    cmd.Parameters.AddWithValue("fechainicio", fechainicio);
                    cmd.Parameters.AddWithValue("fechafin", fechafin);
                    cmd.Parameters.AddWithValue("idproveedor", idproveedor);
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new ReporteCompra()
                            {
                                FechaRegistro = dr["FechaRegistro"].ToString(),
                                TipoDocumento = dr["TipoDocumento"].ToString(),
                                NoFactura = dr["NoFactura"].ToString(),
                                NumeroDocumento = dr["NumeroDocumento"].ToString(),
                                MontoTotal = dr["MontoTotal"] != DBNull.Value ? Convert.ToDecimal(dr["MontoTotal"]).ToString("N2") : "0.00",
                                Impuesto = dr["Impuesto"] != DBNull.Value ? Convert.ToDecimal(dr["Impuesto"]).ToString("N2") : "0.00",
                                UsuarioRegistro = dr["UsuarioRegistro"].ToString(),
                                DocumentoProveedor = dr["DocumentoProveedor"].ToString(),
                                RazonSocial = dr["RazonSocial"].ToString(),
                                MetodoPago = dr["MetodoPago"].ToString(),
                                Condicion = dr["Condicion"].ToString(),
                                FechaVencimiento = dr["FechaVencimiento"] != DBNull.Value ? dr["FechaVencimiento"].ToString() : string.Empty,
                                CantidadProductos = dr["CantidadProductos"] != DBNull.Value ? Convert.ToInt32(dr["CantidadProductos"]) : 0,
                                Estado = dr["Estado"].ToString()   // <-- AGREGAR ESTA LÍNEA
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener el reporte de compras: " + ex.Message);
                }
            }

            return lista;
        }


        public List<ReporteVenta> Venta(string fechainicio, string fechafin, out string productoMasVendido)
        {
            List<ReporteVenta> lista = new List<ReporteVenta>();
            productoMasVendido = string.Empty;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_ReporteVentas", oconexion);
                    cmd.Parameters.AddWithValue("fechainicio", fechainicio);
                    cmd.Parameters.AddWithValue("fechafin", fechafin);
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        // Primer resultado: lista de ventas
                        while (dr.Read())
                        {
                            lista.Add(new ReporteVenta()
                            {
                                FechaRegistro = dr["FechaRegistro"].ToString(),
                                TipoDocumento = dr["TipoDocumento"].ToString(),
                                NumeroDocumento = dr["NumeroDocumento"].ToString(),
                                PorcentajeTarjeta = dr["PorcentajeTarjeta"].ToString() + " %",
                                ComisionBancaria = Convert.ToDecimal(dr["ComisionBanco"]).ToString("N2"),
                                TotalAPagar = Convert.ToDecimal(dr["TotalAPagar"]).ToString("N2"),
                                CantidadProductos = Convert.ToInt32(dr["CantidadProductos"]),
                                MetodoPago = dr["MetodoPago"].ToString(),
                                UsuarioRegistro = dr["UsuarioRegistro"].ToString(),
                                DocumentoCliente = dr["DocumentoCliente"].ToString(),
                                NombreCliente = dr["NombreCliente"].ToString(),
                                Impuesto = dr["Impuesto"].ToString(),
                                Condicion = dr["Condicion"].ToString(),
                                FechaVencimiento = dr["FechaVencimiento"].ToString(),
                                Estado = dr["Estado"].ToString()
                            });
                        }

                        // Segundo resultado: producto más vendido
                        if (dr.NextResult() && dr.Read())
                        {
                            productoMasVendido = dr["Producto"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    lista = new List<ReporteVenta>();
                    // Log opcional del error: ex.Message
                }
            }

            return lista;
        }


        public List<ReporteGananciaProducto> GananciaPorProducto(string fechainicio, string fechafin)
        {
            List<ReporteGananciaProducto> lista = new List<ReporteGananciaProducto>();

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_ReporteGananciaPorProducto", oconexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FechaInicio", fechainicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fechafin);

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new ReporteGananciaProducto
                            {
                                NombreProducto = dr["NombreProducto"].ToString(),
                                CantidadVendida = Convert.ToInt32(dr["CantidadVendida"]),
                                CostoTotal = Convert.ToDecimal(dr["CostoTotal"]).ToString("0.00"),
                                TotalVentas = Convert.ToDecimal(dr["TotalVentas"]).ToString("0.00"),
                                Ganancia = Convert.ToDecimal(dr["Ganancia"]).ToString("0.00")
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener el reporte de ganancia por producto: " + ex.Message);
                }
            }

            return lista;
        }

        public List<ReporteUtilidadBruta> ObtenerUtilidadBruta(string fechainicio, string fechafin, string metodoPago, string usuario, string condicion, string estadoFactura)
        {
            List<ReporteUtilidadBruta> lista = new List<ReporteUtilidadBruta>();

            using (SqlConnection con = new SqlConnection(Conexion.cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_ReporteUtilidadBruta", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicio", fechainicio);
                cmd.Parameters.AddWithValue("@FechaFin", fechafin);
                cmd.Parameters.AddWithValue("@MetodoPago", metodoPago);
                cmd.Parameters.AddWithValue("@Usuario", usuario);
                cmd.Parameters.AddWithValue("@Condicion", condicion);
                cmd.Parameters.AddWithValue("@EstadoFactura", estadoFactura); // NUEVO

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ReporteUtilidadBruta()
                        {
                            Fecha = Convert.ToDateTime(dr["Fecha"]).ToString("dd/MM/yyyy"),
                            Numero = dr["Numero"].ToString(),
                            Cliente = dr["Cliente"].ToString(),
                            Vendedor = dr["Vendedor"].ToString(),
                            MetodoPago = dr["MetodoPago"].ToString(),
                            Condicion = dr["Condicion"].ToString(),
                            TotalVenta = Convert.ToDecimal(dr["TotalVenta"]),
                            CostoTotal = Convert.ToDecimal(dr["CostoTotal"]),
                            UtilidadBruta = Convert.ToDecimal(dr["UtilidadBruta"]),
                            SaldoPendiente = Convert.ToDecimal(dr["SaldoPendiente"]),
                            EstadoFactura = dr["EstadoFactura"].ToString()
                        });
                    }
                }
            }

            return lista;
        }
        public List<ReporteUtilidadNeta> ObtenerUtilidadNeta(string fechainicio, string fechafin)
        {
            List<ReporteUtilidadNeta> lista = new List<ReporteUtilidadNeta>();

            using (SqlConnection con = new SqlConnection(Conexion.cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_ReporteUtilidadNeta", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("fechainicio", fechainicio);
                cmd.Parameters.AddWithValue("fechafin", fechafin);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ReporteUtilidadNeta()
                        {
                            TotalVentas = Convert.ToDecimal(dr["TotalVentas"]),
                            TotalCostos = Convert.ToDecimal(dr["TotalCostos"]), // ← esta línea nueva
                            TotalEgresos = Convert.ToDecimal(dr["TotalEgresos"]),
                            UtilidadNeta = Convert.ToDecimal(dr["UtilidadNeta"])
                        });
                    }
                }
            }

            return lista;
        }




        public List<ReporteGananciaPorCliente> ObtenerGananciaPorCliente(string fechainicio, string fechafin)
        {
            List<ReporteGananciaPorCliente> lista = new List<ReporteGananciaPorCliente>();

            using (SqlConnection con = new SqlConnection(Conexion.cadena))

            {
                SqlCommand cmd = new SqlCommand("sp_ReporteGananciaPorCliente", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("fechainicio", fechainicio);
                cmd.Parameters.AddWithValue("fechafin", fechafin);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ReporteGananciaPorCliente()
                        {
                            NombreCliente = dr["NombreCliente"].ToString(),
                            TotalVentas = Convert.ToDecimal(dr["TotalVentas"]),
                            TotalCostos = Convert.ToDecimal(dr["TotalCostos"]),
                            Ganancia = Convert.ToDecimal(dr["Ganancia"])
                        });
                    }
                }
            }

            return lista;
        }

        public List<ReporteCierreCaja> ObtenerReporteCierresCaja(string fechainicio, string fechafin)
        {
            List<ReporteCierreCaja> lista = new List<ReporteCierreCaja>();

            using (SqlConnection con = new SqlConnection(Conexion.cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_ReporteCierresCaja", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicio", fechainicio);
                cmd.Parameters.AddWithValue("@FechaFin", fechafin);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ReporteCierreCaja()
                        {
                            FechaInicio = Convert.ToDateTime(dr["FechaInicio"]).ToString("dd/MM/yyyy HH:mm"),
                            FechaFin = Convert.ToDateTime(dr["FechaFin"]).ToString("dd/MM/yyyy HH:mm"),
                            MontoInicial = dr["MontoInicial"].ToString(),
                            IngresosEfectivo = dr["IngresosEfectivo"].ToString(),
                            IngresosTarjeta = dr["IngresosTarjeta"].ToString(),
                            IngresosTransferencia = dr["IngresosTransferencia"].ToString(),
                            Egresos = dr["Egresos"].ToString(),
                            EgresosTransferencia = dr["EgresosTransferencia"].ToString(),
                            TotalCalculado = dr["TotalCalculado"].ToString(),
                            TotalTransferencia = dr["TotalTransferencia"].ToString(),
                            GranTotal = dr["GranTotal"].ToString(),
                            TotalReal = dr["TotalReal"].ToString(),
                            Diferencia = dr["Diferencia"].ToString(),
                            UsuarioResponsable = dr["UsuarioResponsable"].ToString(),
                            Observaciones = dr["Observaciones"].ToString()
                        });
                    }
                }
            }

            return lista;
        }



    }
}
