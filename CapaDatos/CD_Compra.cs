using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using CapaEntidad;
using System.Globalization;


namespace CapaDatos
{

    public class ResumenAbonoProveedor
    {
        public int IdProveedor { get; set; }
        public string Proveedor { get; set; }
        public decimal TotalAbonado { get; set; }
        public int NumeroAbonos { get; set; }
    }
    public class CD_Compra
    {
        public int ObtenerCorrelativo()
        {
            int idcorrelativo = 0;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {

                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select count(*) + 1 from COMPRA");
                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    idcorrelativo = Convert.ToInt32(cmd.ExecuteScalar());

                }
                catch (Exception ex)
                {
                    idcorrelativo = 0;
                }
            }
            return idcorrelativo;
        }


        public bool Registrar(Compra obj, DataTable DetalleCompra, out string Mensaje)
        {
            bool Respuesta = false;
            Mensaje = string.Empty;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarCompra", oconexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("IdUsuario", obj.oUsuario.IdUsuario);
                    cmd.Parameters.AddWithValue("IdProveedor", obj.oProveedor.IdProveedor);
                    cmd.Parameters.AddWithValue("TipoDocumento", obj.TipoDocumento);
                    cmd.Parameters.AddWithValue("NumeroDocumento", obj.NumeroDocumento);
                    cmd.Parameters.AddWithValue("NoFactura", obj.NoFactura);
                    cmd.Parameters.AddWithValue("MontoTotal", obj.MontoTotal);
                    cmd.Parameters.AddWithValue("MetodoPago", obj.MetodoPago);
                    cmd.Parameters.AddWithValue("Condicion", obj.Condicion);
                    cmd.Parameters.AddWithValue("SaldoPendiente", obj.SaldoPendiente);
                    cmd.Parameters.AddWithValue("Impuesto", obj.Impuesto);

                    // NOTA: FechaCompra no se usa en tu versión actual, pero lo dejamos comentado si luego activás ese campo
                    // cmd.Parameters.AddWithValue("FechaCompra", obj.FechaCompra);

                    cmd.Parameters.AddWithValue("FechaVencimiento",
                        string.IsNullOrEmpty(obj.FechaVencimiento)
                            ? (object)DBNull.Value
                            : DateTime.ParseExact(obj.FechaVencimiento, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                    );

                    // Aquí se pasa el DataTable con columna DescuentoPorcentaje
                    cmd.Parameters.AddWithValue("DetalleCompra", DetalleCompra);

                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    Respuesta = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
                catch (Exception ex)
                {
                    Respuesta = false;
                    Mensaje = ex.Message;
                }
            }

            return Respuesta;
        }

        public Compra ObtenerCompra(string numero)
        {
            Compra obj = new Compra();
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select c.IdCompra,");
                    query.AppendLine("u.NombreCompleto,");
                    query.AppendLine("pr.Documento, pr.RazonSocial,");
                    query.AppendLine("c.TipoDocumento, c.NumeroDocumento, c.NoFactura, c.MontoTotal, c.Impuesto,"); // <-- coma aquí
                    query.AppendLine("convert(char(10), c.FechaRegistro, 103) [FechaRegistro],");
                    query.AppendLine("c.MetodoPago, c.Condicion, c.SaldoPendiente, c.FechaVencimiento,");
                    query.AppendLine("c.Estado");
                    query.AppendLine("from COMPRA c");
                    query.AppendLine("inner join USUARIO u on u.IdUsuario = c.IdUsuario");
                    query.AppendLine("inner join PROVEEDOR pr on pr.IdProveedor = c.IdProveedor");
                    query.AppendLine("where c.NumeroDocumento = @numero");

                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            obj = new Compra()
                            {
                                IdCompra = Convert.ToInt32(dr["IdCompra"]),
                                oUsuario = new Usuario() { NombreCompleto = dr["NombreCompleto"].ToString() },
                                oProveedor = new Proveedor() { Documento = dr["Documento"].ToString(), RazonSocial = dr["RazonSocial"].ToString() },
                                TipoDocumento = dr["TipoDocumento"].ToString(),
                                NumeroDocumento = dr["NumeroDocumento"].ToString(),
                                NoFactura = dr["NoFactura"].ToString(),
                                MontoTotal = Convert.ToDecimal(dr["MontoTotal"].ToString()),
                                Impuesto = Convert.ToDecimal(dr["Impuesto"]), 
                                FechaRegistro = dr["FechaRegistro"].ToString(),
                                MetodoPago = dr["MetodoPago"].ToString(),
                                Condicion = dr["Condicion"].ToString(),
                                SaldoPendiente = Convert.ToDecimal(dr["SaldoPendiente"].ToString()),
                                FechaVencimiento = dr["FechaVencimiento"] != DBNull.Value ? Convert.ToDateTime(dr["FechaVencimiento"]).ToString("dd/MM/yyyy") : null,
                                Estado = dr["Estado"].ToString()
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    obj = new Compra();
                }
            }
            return obj;
        }


        public List<Detalle_Compra> ObtenerDetalleCompra(int idcompra)
        {
            List<Detalle_Compra> oLista = new List<Detalle_Compra>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    conexion.Open();
                    StringBuilder query = new StringBuilder();

                    query.AppendLine("select p.Nombre, dc.PrecioCompra, dc.Cantidad, dc.MontoTotal, dc.DescuentoPorcentaje from DetalleCompra dc");
                    query.AppendLine("inner join PRODUCTO p on p.IdProducto = dc.IdProducto");
                    query.AppendLine("where dc.IdCompra = @idcompra");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.AddWithValue("@idcompra", idcompra);
                    cmd.CommandType = CommandType.Text;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            oLista.Add(new Detalle_Compra()
                            {
                                oProducto = new Producto() { Nombre = dr["Nombre"].ToString() },
                                PrecioCompra = Convert.ToDecimal(dr["PrecioCompra"].ToString()),
                                Cantidad = Convert.ToDecimal(dr["Cantidad"].ToString()), 
                                MontoTotal = Convert.ToDecimal(dr["MontoTotal"].ToString()),
                                DescuentoPorcentaje = Convert.ToDecimal(dr["DescuentoPorcentaje"].ToString())
                            
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oLista = new List<Detalle_Compra>();
            }
            return oLista;
        }

        public List<Compra> ObtenerComprasCredito()
        {
            List<Compra> lista = new List<Compra>();
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT c.IdCompra, c.NumeroDocumento, c.NoFactura, c.MontoTotal, c.SaldoPendiente, c.FechaVencimiento,");
                    query.AppendLine("convert(char(10), c.FechaRegistro, 103) AS FechaRegistro, pr.RazonSocial");
                    query.AppendLine("FROM COMPRA c");
                    query.AppendLine("INNER JOIN PROVEEDOR pr ON pr.IdProveedor = c.IdProveedor");
                    query.AppendLine("WHERE c.Condicion = 'Credito' AND c.Estado = 'Activa'"); // <-- SOLO ACTIVAS

                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Compra()
                            {
                                IdCompra = Convert.ToInt32(dr["IdCompra"]),
                                NumeroDocumento = dr["NumeroDocumento"].ToString(),
                                NoFactura = dr["NoFactura"].ToString(),
                                MontoTotal = Convert.ToDecimal(dr["MontoTotal"]),
                                SaldoPendiente = Convert.ToDecimal(dr["SaldoPendiente"]),
                                // Convertir FechaVencimiento a string en formato "dd/MM/yyyy"
                                FechaVencimiento = dr["FechaVencimiento"] != DBNull.Value
                                    ? Convert.ToDateTime(dr["FechaVencimiento"]).ToString("dd/MM/yyyy")
                                    : null, // Asignar null si es DBNull
                                FechaRegistro = dr["FechaRegistro"].ToString(),
                                oProveedor = new Proveedor() { RazonSocial = dr["RazonSocial"].ToString() }
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    lista = new List<Compra>();
                }
            }
            return lista;
        }

        public bool ActualizarSaldoPendiente(int idCompra, decimal nuevoSaldoPendiente, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("UPDATE COMPRA SET SaldoPendiente = @SaldoPendiente WHERE IdCompra = @IdCompra", oconexion);
                    cmd.Parameters.AddWithValue("@SaldoPendiente", nuevoSaldoPendiente);
                    cmd.Parameters.AddWithValue("@IdCompra", idCompra);

                    oconexion.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    resultado = filasAfectadas > 0; 
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                }
            }
            return resultado;
        }

        public bool RegistrarCredito(Compra compra, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarCredito", oconexion);
                    cmd.Parameters.AddWithValue("NoFactura", compra.NoFactura);
                    cmd.Parameters.AddWithValue("IdProveedor", compra.oProveedor.IdProveedor);
                    cmd.Parameters.AddWithValue("MontoTotal", compra.MontoTotal);
                    cmd.Parameters.AddWithValue("SaldoPendiente", compra.SaldoPendiente);
                    cmd.Parameters.AddWithValue("FechaVencimiento", compra.FechaVencimiento);
                    cmd.Parameters.AddWithValue("MetodoPago", compra.MetodoPago);
                    cmd.Parameters.AddWithValue("Condicion", compra.Condicion);
                    cmd.Parameters.AddWithValue("FechaRegistro", compra.FechaRegistro);
                    cmd.Parameters.AddWithValue("TipoDocumento", compra.TipoDocumento);
                    cmd.Parameters.AddWithValue("NumeroDocumento", compra.NumeroDocumento);
                    cmd.Parameters.AddWithValue("Impuesto", 0); 

                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    resultado = filasAfectadas > 0;
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                }
            }
            return resultado;
        }

        public bool AnularCompra(int idCompra, out string mensaje)
        {
            bool respuesta = false;
            mensaje = string.Empty;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("AnularCompra", oconexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdCompra", idCompra);

                        oconexion.Open();
                        cmd.ExecuteNonQuery();
                    }
                    respuesta = true;
                    mensaje = "Compra anulada correctamente.";
                }
                catch (Exception ex)
                {
                    mensaje = "Error al anular la compra: " + ex.Message;
                    respuesta = false;
                }
            }
            return respuesta;
        }

        public int ObtenerIdCompraPorNumeroDocumento(string numeroDocumento)
        {
            int idCompra = 0;
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    oconexion.Open();
                    string query = "SELECT IdCompra FROM COMPRA WHERE NumeroDocumento = @NumeroDocumento AND Estado = 'Activa'";
                    using (SqlCommand cmd = new SqlCommand(query, oconexion))
                    {
                        cmd.Parameters.AddWithValue("@NumeroDocumento", numeroDocumento);
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            idCompra = Convert.ToInt32(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener el ID de la compra: " + ex.Message);
                }
            }
            return idCompra;
        }

        public List<Compra> ObtenerComprasPorProducto(int idProducto)
        {
            List<Compra> lista = new List<Compra>();

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    conexion.Open();
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT c.IdCompra, c.NumeroDocumento, c.FechaRegistro");
                    query.AppendLine("FROM COMPRA c");
                    query.AppendLine("INNER JOIN DetalleCompra dc ON c.IdCompra = dc.IdCompra");
                    query.AppendLine("WHERE dc.IdProducto = @idProducto");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.AddWithValue("@idProducto", idProducto);
                    cmd.CommandType = CommandType.Text;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Compra()
                            {
                                IdCompra = Convert.ToInt32(dr["IdCompra"]),
                                NumeroDocumento = dr["NumeroDocumento"].ToString(),
                                FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]).ToString("dd/MM/yyyy HH:mm:ss")
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener compras por producto: " + ex.Message);
                }
            }
            return lista;
        }

        public List<ResumenAbonoProveedor> ObtenerResumenAbonosPorProveedor(DateTime fechaInicio, DateTime fechaFin, int? idProveedor = null)
        {
            var lista = new List<ResumenAbonoProveedor>();

            using (SqlConnection cn = new SqlConnection(Conexion.cadena))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_ResumenAbonosPorProveedor", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                    if (idProveedor.HasValue)
                        cmd.Parameters.AddWithValue("@IdProveedor", idProveedor.Value);
                    else
                        cmd.Parameters.AddWithValue("@IdProveedor", DBNull.Value);

                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new ResumenAbonoProveedor
                            {
                                IdProveedor = Convert.ToInt32(dr["IdProveedor"]),
                                Proveedor = dr["Proveedor"].ToString(),
                                TotalAbonado = Convert.ToDecimal(dr["TotalAbonado"]),
                                NumeroAbonos = Convert.ToInt32(dr["NumeroAbonos"])
                            });
                        }
                    }
                }
            }

            return lista;
        }


    }
}
