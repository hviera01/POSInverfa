using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Venta
    {
        public int ObtenerCorrelativo()
        {
            int idcorrelativo = 4932; // Valor inicial (sin ceros a la izquierda)

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT MAX(TRY_CAST(NumeroDocumento AS INT)) FROM VENTA WHERE TipoDocumento IN ('Factura', 'Boleta')");
                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        int maxCorrelativo = Convert.ToInt32(result);
                        idcorrelativo = maxCorrelativo + 1; // Sumar 1 al máximo encontrado
                    }
                    else
                    {
                        idcorrelativo = 4933; // Si no hay registros, el siguiente correlativo es 4933
                    }

                    // Asegurarse de que el correlativo no sea menor que 4933
                    if (idcorrelativo < 4933)
                    {
                        idcorrelativo = 4933;
                    }
                }
                catch (Exception ex)
                {
                    idcorrelativo = 4932; // En caso de error, mantener el valor inicial
                }
            }
            return idcorrelativo;
        }



        public int ObtenerCorrelativoCotizacion()
        {
            int idcorrelativo = 0;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {

                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select count(*) + 1 from VENTA where TipoDocumento = 'Cotizacion'");
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

        

        public int ObtenerCorrelativoCanje()
        {
            int idcorrelativo = 0;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {

                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select count(*) + 1 from VENTA where TipoDocumento = 'Canje'");
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
        public int ObtenerCorrelativoVentaSinFacturar()
        {
            int idcorrelativo = 1; 

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT COUNT(*) + 1 FROM VENTA WHERE TipoDocumento = 'Venta'");

                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();
                    idcorrelativo = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    idcorrelativo = 1;
                }
            }
            return idcorrelativo;
        }

        public bool RestarStock(int idproducto, decimal cantidad)
        {
            bool respuesta = true;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("update producto set stock = stock - @cantidad where idproducto = @idproducto");

                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);
                    cmd.Parameters.AddWithValue("@cantidad", cantidad); 
                    cmd.Parameters.AddWithValue("@idproducto", idproducto);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    respuesta = cmd.ExecuteNonQuery() > 0 ? true : false;
                }
                catch (Exception ex)
                {
                    respuesta = false;
                }
            }
            return respuesta;

        }


        public bool RegistrarCredito(Venta obj, out string mensaje)
        {
            bool ok = false;
            mensaje = string.Empty;

            using (SqlConnection cn = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_RegistrarVentaCredito", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // <-- algunos proveedores esperan este parámetro de retorno
                        var ret = cmd.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
                        ret.Direction = ParameterDirection.ReturnValue;

                        cmd.Parameters.Add("@NumeroDocumento", SqlDbType.NVarChar, 50)
                           .Value = string.IsNullOrWhiteSpace(obj.NumeroDocumento) ? "N/A" : obj.NumeroDocumento;

                        cmd.Parameters.Add("@DocumentoCliente", SqlDbType.NVarChar, 50)
                           .Value = string.IsNullOrWhiteSpace(obj.DocumentoCliente) ? "N/A" : obj.DocumentoCliente;

                        cmd.Parameters.Add("@NombreCliente", SqlDbType.NVarChar, 100)
                           .Value = string.IsNullOrWhiteSpace(obj.NombreCliente) ? "N/A" : obj.NombreCliente;

                        var pTotal = cmd.Parameters.Add("@MontoTotal", SqlDbType.Decimal);
                        pTotal.Precision = 18; pTotal.Scale = 2; pTotal.Value = obj.MontoTotal;

                        var pSaldo = cmd.Parameters.Add("@SaldoPendiente", SqlDbType.Decimal);
                        pSaldo.Precision = 18; pSaldo.Scale = 2; pSaldo.Value = obj.SaldoPendiente;

                        cmd.Parameters.Add("@FechaVencimiento", SqlDbType.DateTime)
                           .Value = (object)obj.FechaVencimiento ?? DBNull.Value;

                        cmd.Parameters.Add("@MetodoPago", SqlDbType.NVarChar, 50)
                           .Value = string.IsNullOrWhiteSpace(obj.MetodoPago) ? "N/A" : obj.MetodoPago;

                        cmd.Parameters.Add("@Condicion", SqlDbType.NVarChar, 50)
                           .Value = string.IsNullOrWhiteSpace(obj.Condicion) ? "Credito" : obj.Condicion;

                        cmd.Parameters.Add("@FechaRegistro", SqlDbType.DateTime)
                           .Value = (obj.FechaRegistro == default(DateTime)) ? DateTime.Now : obj.FechaRegistro;

                        cmd.Parameters.Add("@TipoDocumento", SqlDbType.NVarChar, 50)
                           .Value = string.IsNullOrWhiteSpace(obj.TipoDocumento) ? "Factura" : obj.TipoDocumento;

                        cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 200)
                           .Value = string.IsNullOrWhiteSpace(obj.Descripcion) ? "N/A" : obj.Descripcion;

                        cn.Open();
                        cmd.ExecuteNonQuery();

                        // opcional: leer código de retorno
                        int rc = (ret.Value is int v) ? v : 0;
                        ok = (rc == 0);
                        if (!ok) mensaje = "El SP devolvió código: " + rc;
                    }
                }
                catch (Exception ex)
                {
                    ok = false;
                    // mensaje claro para ti en producción
                    mensaje = "Error al ejecutar dbo.sp_RegistrarVentaCredito: " + ex.Message;
                }
            }
            return ok;
        }


        public bool SumarStock(int idproducto, decimal cantidad)
        {
            bool respuesta = true;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("update producto set stock = stock + @cantidad where idproducto = @idproducto");
                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);
                    cmd.Parameters.AddWithValue("@cantidad", cantidad);
                    cmd.Parameters.AddWithValue("@idproducto", idproducto);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    respuesta = cmd.ExecuteNonQuery() > 0 ? true : false;
                }
                catch (Exception ex)
                {
                    respuesta = false;
                }
            }
            return respuesta;

        }



        public int Registrar(Venta obj, DataTable DetalleVenta, out string Mensaje)
        {
            int idResultado = 0;
            Mensaje = string.Empty;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_RegistrarVenta", oconexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdUsuario", obj.oUsuario.IdUsuario);
                    cmd.Parameters.AddWithValue("@TipoDocumento", obj.TipoDocumento);
                    cmd.Parameters.AddWithValue("@NumeroDocumento", obj.NumeroDocumento);
                    cmd.Parameters.AddWithValue("@DocumentoCliente", obj.DocumentoCliente);
                    cmd.Parameters.AddWithValue("@NombreCliente", obj.NombreCliente);
                    cmd.Parameters.AddWithValue("@MontoPago", obj.MontoPago);
                    cmd.Parameters.AddWithValue("@MontoCambio", obj.MontoCambio);
                    cmd.Parameters.AddWithValue("@MontoTotal", obj.MontoTotal);
                    cmd.Parameters.AddWithValue("@MetodoPago", obj.MetodoPago);
                    cmd.Parameters.AddWithValue("@Condicion", obj.Condicion);
                    cmd.Parameters.AddWithValue("@FechaRegistro", obj.FechaRegistro);

                    SqlParameter p_detalle = cmd.Parameters.AddWithValue("@DetalleVenta", DetalleVenta);
                    p_detalle.SqlDbType = SqlDbType.Structured;

                    cmd.Parameters.Add("@Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    idResultado = Convert.ToInt32(cmd.Parameters["@Resultado"].Value);
                    Mensaje = cmd.Parameters["@Mensaje"].Value.ToString();
                }
                catch (Exception ex)
                {
                    idResultado = 0;
                    Mensaje = ex.Message;
                }
            }

            return idResultado;
        }

        public Venta ObtenerVenta(string numero)
        {
            Venta obj = new Venta();

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    conexion.Open();

                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT");
                    query.AppendLine("  v.IdVenta,");
                    query.AppendLine("  u.NombreCompleto,");
                    query.AppendLine("  v.DocumentoCliente,");
                    query.AppendLine("  v.NombreCliente,");
                    query.AppendLine("  v.TipoDocumento,");
                    query.AppendLine("  v.NumeroDocumento,");
                    query.AppendLine("  v.MontoPago,");
                    query.AppendLine("  v.MontoCambio,");
                    query.AppendLine("  v.MontoTotal,");
                    query.AppendLine("  v.MetodoPago,");
                    query.AppendLine("  v.Impuesto,");
                    query.AppendLine("  v.Condicion,");
                    query.AppendLine("  v.FechaVencimiento,");
                    query.AppendLine("  v.FechaRegistro,"); // ← coma corregida
                    query.AppendLine("  v.Estado");
                    query.AppendLine("FROM VENTA v");
                    query.AppendLine("INNER JOIN USUARIO u ON u.IdUsuario = v.IdUsuario");
                    query.AppendLine("WHERE v.NumeroDocumento = @numero");

                    using (SqlCommand cmd = new SqlCommand(query.ToString(), conexion))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@numero", numero ?? (object)DBNull.Value);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                obj = new Venta()
                                {
                                    IdVenta = dr["IdVenta"] != DBNull.Value ? Convert.ToInt32(dr["IdVenta"]) : 0,
                                    oUsuario = new Usuario() { NombreCompleto = dr["NombreCompleto"]?.ToString() ?? string.Empty },
                                    DocumentoCliente = dr["DocumentoCliente"]?.ToString() ?? string.Empty,
                                    NombreCliente = dr["NombreCliente"]?.ToString() ?? string.Empty,
                                    TipoDocumento = dr["TipoDocumento"]?.ToString() ?? string.Empty,
                                    NumeroDocumento = dr["NumeroDocumento"]?.ToString() ?? string.Empty,
                                    MontoPago = dr["MontoPago"] != DBNull.Value ? Convert.ToDecimal(dr["MontoPago"]) : 0m,
                                    MontoCambio = dr["MontoCambio"] != DBNull.Value ? Convert.ToDecimal(dr["MontoCambio"]) : 0m,
                                    MontoTotal = dr["MontoTotal"] != DBNull.Value ? Convert.ToDecimal(dr["MontoTotal"]) : 0m,
                                    MetodoPago = dr["MetodoPago"]?.ToString() ?? string.Empty,
                                    Impuesto = dr["Impuesto"] != DBNull.Value ? Convert.ToDecimal(dr["Impuesto"]) : 0m,
                                    Condicion = dr["Condicion"]?.ToString() ?? string.Empty,
                                    FechaVencimiento = dr["FechaVencimiento"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["FechaVencimiento"]) : null,
                                    FechaRegistro = dr["FechaRegistro"] != DBNull.Value ? Convert.ToDateTime(dr["FechaRegistro"]) : DateTime.MinValue,
                                    Estado = dr["Estado"]?.ToString() ?? string.Empty,
                                    oDetalle_Venta = new List<Detalle_Venta>() // se rellena abajo
                                };
                            }
                        }
                    }

                    // Cargar detalle si hay IdVenta válido
                    if (obj != null && obj.IdVenta > 0)
                    {
                        obj.oDetalle_Venta = ObtenerDetalleVenta(obj.IdVenta);
                    }
                }
                catch
                {
                    // Ante cualquier excepción, devolvemos una venta vacía (IdVenta = 0)
                    obj = new Venta();
                }
            }

            return obj;
        }



        public List<Detalle_Venta> ObtenerDetalleVenta(int idVenta)
        {
            List<Detalle_Venta> oLista = new List<Detalle_Venta>();

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    conexion.Open();
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT p.IdProducto, p.Nombre, dv.PrecioVenta, dv.Cantidad, dv.SubTotal,dv.NombreProducto ");
                    query.AppendLine("FROM DETALLE_VENTA dv ");
                    query.AppendLine("INNER JOIN PRODUCTO p ON p.IdProducto = dv.IdProducto ");
                    query.AppendLine("WHERE dv.IdVenta = @idventa");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.AddWithValue("@idventa", idVenta);
                    cmd.CommandType = CommandType.Text;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            oLista.Add(new Detalle_Venta()
                            {
                                oProducto = new Producto()
                                {
                                    IdProducto = Convert.ToInt32(dr["IdProducto"]),  
                                    Nombre = dr["Nombre"].ToString()
                                },
                                PrecioVenta = Convert.ToDecimal(dr["PrecioVenta"]),
                                Cantidad = Convert.ToDecimal(dr["Cantidad"]),
                                SubTotal = Convert.ToDecimal(dr["SubTotal"]),
                                NombreProducto = dr["NombreProducto"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener detalle de venta: " + ex.Message);
                    oLista = new List<Detalle_Venta>();
                }
            }
            return oLista;
        }

        public bool AnularVenta(int idVenta, out string mensaje)
        {
            bool respuesta = false;
            mensaje = string.Empty;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    oconexion.Open();

                    
                    using (SqlCommand cmd = new SqlCommand("AnularVenta", oconexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        
                        cmd.Parameters.Add(new SqlParameter("@IdVenta", SqlDbType.Int) { Value = idVenta });

                       
                        cmd.ExecuteNonQuery();
                    }

                   
                    respuesta = true;
                    mensaje = "Factura anulada correctamente.";
                }
                catch (Exception ex)
                {
                    
                    mensaje = "Error al anular la factura: " + ex.Message;
                }
            }

            return respuesta;
        }

        public int ObtenerIdVentaPorNumeroDocumento(string numeroDocumento)
        {
            int idVenta = 0;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    oconexion.Open();

                    // Buscamos SIEMPRE la última venta con ese NúmeroDocumento
                    // (en tu caso "N/A") sin filtrar por Estado.
                    string query = @"
                SELECT TOP 1 IdVenta
                FROM VENTA
                WHERE NumeroDocumento = @NumeroDocumento
                ORDER BY IdVenta DESC";

                    using (SqlCommand cmd = new SqlCommand(query, oconexion))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@NumeroDocumento", SqlDbType.NVarChar, 50)
                                      .Value = (object)(numeroDocumento ?? (object)DBNull.Value);

                        var result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                            idVenta = Convert.ToInt32(result);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener el ID de la venta: " + ex.Message);
                    idVenta = 0;
                }
            }

            return idVenta;
        }



        public List<VentaCreditoResumen> ObtenerFacturasCredito()
        {
            var lista = new List<VentaCreditoResumen>();

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Venta_ObtenerFacturasCredito", oconexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var obj = new VentaCreditoResumen()
                            {
                                IdVenta = Convert.ToInt32(dr["IdVenta"]),
                                FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                                NumeroDocumento = dr["NumeroDocumento"].ToString(),
                                CodCliente = dr["CodCliente"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CodCliente"]),
                                NombreCliente = dr["NombreCliente"].ToString(),
                                MontoTotal = Convert.ToDecimal(dr["MontoTotal"]),
                                SaldoPendiente = Convert.ToDecimal(dr["SaldoPendiente"]),
                                FechaVencimiento = dr["FechaVencimiento"] == DBNull.Value
                                                    ? (DateTime?)null
                                                    : Convert.ToDateTime(dr["FechaVencimiento"]),
                                Estado = dr["Estado"].ToString(),
                                Vencida = dr["Vencida"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                NumeroCuotas = dr["NumeroCuotas"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NumeroCuotas"]),
                                ProximoPago = dr["ProximoPago"] == DBNull.Value
                                                    ? (DateTime?)null
                                                    : Convert.ToDateTime(dr["ProximoPago"]),
                                Mora = dr["Mora"].ToString()
                            };

                            lista.Add(obj);
                        }
                    }
                }
            }

            return lista;
        }

        public bool ActualizarDatosCredito(Venta v, out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                using (SqlConnection cn = new SqlConnection(Conexion.cadena))
                using (SqlCommand cmd = new SqlCommand("sp_Venta_ActualizarDatosCredito", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdVenta", v.IdVenta);
                    cmd.Parameters.AddWithValue("@NumeroDocumento", v.NumeroDocumento);
                    cmd.Parameters.AddWithValue("@NombreCliente", v.NombreCliente);
                    cmd.Parameters.AddWithValue("@DocumentoCliente", v.DocumentoCliente);
                    cmd.Parameters.AddWithValue("@Descripcion", v.Descripcion ?? "N/A");
                    cmd.Parameters.AddWithValue("@FechaVencimiento",
                        (object)v.FechaVencimiento ?? DBNull.Value);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }


        public bool ActualizarEstadoFactura(int idVenta, string estado)
        {
            bool resultado = false;

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("UPDATE VENTA SET Condicion = @Condicion WHERE IdVenta = @IdVenta", conexion);
                    cmd.Parameters.AddWithValue("@Condicion", estado);
                    cmd.Parameters.AddWithValue("@IdVenta", idVenta);

                    conexion.Open();
                    resultado = cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al actualizar el estado de la factura: " + ex.Message);
                }
            }

            return resultado;
        }

        public bool ActualizarSaldoPendiente(int idVenta, decimal TotalPagar)
        {
            bool resultado = false;

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("UPDATE VENTA SET SaldoPendiente = @SaldoPendiente WHERE IdVenta = @IdVenta", conexion);
                    cmd.Parameters.AddWithValue("@SaldoPendiente", TotalPagar);
                    cmd.Parameters.AddWithValue("@IdVenta", idVenta);

                    conexion.Open();
                    resultado = cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al actualizar el saldo pendiente: " + ex.Message);
                }
            }

            return resultado;
        }

        public List<Venta> ObtenerVentasPorProducto(int idProducto)
        {
            List<Venta> lista = new List<Venta>();

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    conexion.Open();
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT * FROM VENTA v");
                    query.AppendLine("INNER JOIN DETALLE_VENTA dv ON v.IdVenta = dv.IdVenta");
                    query.AppendLine("WHERE dv.IdProducto = @idProducto");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.AddWithValue("@idProducto", idProducto);
                    cmd.CommandType = CommandType.Text;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Venta()
                            {
                                IdVenta = Convert.ToInt32(dr["IdVenta"]),
                                NumeroDocumento = dr["NumeroDocumento"].ToString(),
                                FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])

                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener ventas por producto: " + ex.Message);
                }
            }
            return lista;
        }

        public bool RegistrarPreparacion(int idVenta, int idUsuario, DataTable tvpPreparacion, out string mensaje)
        {
            bool ok = false;
            mensaje = string.Empty;

            using (SqlConnection cn = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarPreparacionVenta", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdVenta", idVenta);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    SqlParameter p = new SqlParameter("@Consumos", SqlDbType.Structured);
                    p.TypeName = "dbo.EPreparacionVenta";
                    p.Value = tvpPreparacion;
                    cmd.Parameters.Add(p);

                    SqlParameter perr = new SqlParameter("@Error", SqlDbType.VarChar, 400) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(perr);

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    string err = Convert.ToString(perr.Value);
                    if (!string.IsNullOrEmpty(err))
                    {
                        mensaje = err;
                        ok = false;
                    }
                    else ok = true;
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    ok = false;
                }
            }
            return ok;
        }

        public DataTable ObtenerPreparacionPorVenta(int idVenta)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.cadena))
            {
                using (SqlCommand cmd = new SqlCommand(@"
;WITH Map AS(
    SELECT dv.IdDetalleVenta,
           ROW_NUMBER() OVER(ORDER BY dv.IdDetalleVenta) AS NumeroItem
    FROM DETALLE_VENTA dv
    WHERE dv.IdVenta = @IdVenta
)
SELECT  m.NumeroItem,
        p.Nombre       AS Insumo,
        d.UnidadElegida AS Unidad,
        d.EquivalenteEn,
        d.CantidadUsada
FROM PREPARACION_VENTA pv
JOIN Map m                    ON m.IdDetalleVenta = pv.IdDetalleVenta
JOIN PREPARACION_VENTA_DET d  ON d.IdPreparacion  = pv.IdPreparacion
JOIN PRODUCTO p               ON p.IdProducto     = d.IdProductoInsumo
ORDER BY m.NumeroItem, p.Nombre;", cn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@IdVenta", idVenta);

                    DataTable dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);
                    return dt;
                }
            }
        }
        public bool EliminarCreditoCompleto(int idVenta, out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                using (SqlConnection cn = new SqlConnection(Conexion.cadena))
                using (SqlCommand cmd = new SqlCommand("sp_Venta_EliminarCreditoCompleto", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdVenta", idVenta);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }

        public bool EliminarVentaCredito(int idVenta, out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                using (SqlConnection cn = new SqlConnection(Conexion.cadena))
                using (SqlCommand cmd = new SqlCommand("sp_Venta_EliminarCredito", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdVenta", idVenta);

                    SqlParameter pMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 500)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(pMensaje);

                    SqlParameter pResultado = new SqlParameter("@Resultado", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(pResultado);

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    bool ok = Convert.ToBoolean(pResultado.Value);
                    mensaje = pMensaje.Value.ToString();
                    return ok;
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }

        public Venta ObtenerVentaCreditoPorId(int idVenta)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.cadena))
            using (SqlCommand cmd = new SqlCommand("sp_Venta_ObtenerCreditoPorId", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdVenta", idVenta);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                        return null;

                    var v = new Venta
                    {
                        IdVenta = Convert.ToInt32(dr["IdVenta"]),
                        NumeroDocumento = dr["NumeroDocumento"].ToString(),
                        NombreCliente = dr["NombreCliente"].ToString(),
                        DocumentoCliente = dr["DocumentoCliente"].ToString(),
                        MontoTotal = Convert.ToDecimal(dr["MontoTotal"]),
                        SaldoPendiente = Convert.ToDecimal(dr["SaldoPendiente"]),
                        FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                        FechaVencimiento = dr["FechaVencimiento"] == DBNull.Value
                                            ? (DateTime?)null
                                            : Convert.ToDateTime(dr["FechaVencimiento"]),
                        Descripcion = dr["Descripcion"].ToString(),

                       
                    };

                    return v;
                }
            }
        }
        public bool ExisteNumeroDocumento(string numeroDocumento)
        {
            bool existe = false;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT COUNT(*)");
                    query.AppendLine("FROM VENTA");
                    query.AppendLine("WHERE NumeroDocumento = @NumeroDocumento");

                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);
                    cmd.Parameters.AddWithValue("@NumeroDocumento", numeroDocumento);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    existe = count > 0;
                }
                catch (Exception)
                {
                    existe = false;
                }
            }

            return existe;
        }


    }
}