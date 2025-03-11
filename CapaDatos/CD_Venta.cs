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
            int idcorrelativo = 0;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {

                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select count(*) + 1 from VENTA where TipoDocumento IN ('Factura', 'Boleta')");
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
        public int ObtenerCorrelativoVentaSinFacturar()
        {
            int idcorrelativo = 1; // Valor por defecto si no hay registros

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT COUNT(*) + 1 FROM VENTA WHERE TipoDocumento = 'Venta Sin'");

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
                    cmd.Parameters.AddWithValue("@cantidad", cantidad); // Asegúrate de que esto sea decimal
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
            bool resultado = false;
            mensaje = string.Empty;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarVentaCredito", oconexion);
                    cmd.Parameters.AddWithValue("@NumeroDocumento", obj.NumeroDocumento); // Almacena el NoFactura en NumeroDocumento
                    cmd.Parameters.AddWithValue("@DocumentoCliente", obj.DocumentoCliente); // Documento del cliente
                    cmd.Parameters.AddWithValue("@NombreCliente", obj.NombreCliente); // Nombre del cliente
                    cmd.Parameters.AddWithValue("@MontoTotal", obj.MontoTotal);
                    cmd.Parameters.AddWithValue("@SaldoPendiente", obj.SaldoPendiente);
                    cmd.Parameters.AddWithValue("@FechaVencimiento", obj.FechaVencimiento);
                    cmd.Parameters.AddWithValue("@MetodoPago", obj.MetodoPago);
                    cmd.Parameters.AddWithValue("@Condicion", obj.Condicion);
                    cmd.Parameters.AddWithValue("@FechaRegistro", obj.FechaRegistro);
                    cmd.Parameters.AddWithValue("@TipoDocumento", obj.TipoDocumento); // Tipo de documento
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                    resultado = true;
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    resultado = false;
                }
            }

            return resultado;
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



        public bool Registrar(Venta obj, DataTable DetalleVenta, out string Mensaje)
        {
            bool Respuesta = false;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("usp_RegistrarVenta", oconexion);
                    cmd.Parameters.AddWithValue("IdUsuario", obj.oUsuario.IdUsuario);
                    cmd.Parameters.AddWithValue("TipoDocumento", obj.TipoDocumento);
                    cmd.Parameters.AddWithValue("NumeroDocumento", obj.NumeroDocumento);
                    cmd.Parameters.AddWithValue("DocumentoCliente", obj.DocumentoCliente);
                    cmd.Parameters.AddWithValue("NombreCliente", obj.NombreCliente);
                    cmd.Parameters.AddWithValue("MontoPago", obj.MontoPago);
                    cmd.Parameters.AddWithValue("MontoCambio", obj.MontoCambio);
                    cmd.Parameters.AddWithValue("MontoTotal", obj.MontoTotal);
                    cmd.Parameters.AddWithValue("MetodoPago", obj.MetodoPago);
                    cmd.Parameters.AddWithValue("Impuesto", obj.Impuesto);
                    cmd.Parameters.AddWithValue("Condicion", obj.Condicion);
                    cmd.Parameters.AddWithValue("FechaVencimiento", obj.FechaVencimiento);
                    cmd.Parameters.AddWithValue("DetalleVenta", DetalleVenta);
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    Respuesta = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                Respuesta = false;
                Mensaje = ex.Message;
            }

            return Respuesta;
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

                    query.AppendLine("SELECT v.IdVenta, u.NombreCompleto,");
                    query.AppendLine("v.DocumentoCliente, v.NombreCliente,");
                    query.AppendLine("v.TipoDocumento, v.NumeroDocumento,");
                    query.AppendLine("v.MontoPago, v.MontoCambio, v.MontoTotal,");
                    query.AppendLine("v.MetodoPago, v.Impuesto, v.Condicion, v.FechaVencimiento,"); // Asegúrate de incluir estas columnas
                    query.AppendLine("CONVERT(CHAR(10), v.FechaRegistro, 103) AS FechaRegistro");
                    query.AppendLine("FROM VENTA v");
                    query.AppendLine("INNER JOIN USUARIO u ON u.IdUsuario = v.IdUsuario");
                    query.AppendLine("WHERE v.NumeroDocumento = @numero");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.CommandType = System.Data.CommandType.Text;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {

                        while (dr.Read())
                        {
                            obj = new Venta()
                            {
                                IdVenta = int.Parse(dr["IdVenta"].ToString()),
                                oUsuario = new Usuario() { NombreCompleto = dr["NombreCompleto"].ToString() },
                                DocumentoCliente = dr["DocumentoCliente"].ToString(),
                                NombreCliente = dr["NombreCliente"].ToString(),
                                TipoDocumento = dr["TipoDocumento"].ToString(),
                                MetodoPago = dr["MetodoPago"].ToString(),
                                NumeroDocumento = dr["NumeroDocumento"].ToString(),
                                MontoPago = Convert.ToDecimal(dr["MontoPago"].ToString()),
                                MontoCambio = Convert.ToDecimal(dr["MontoCambio"].ToString()),
                                MontoTotal = Convert.ToDecimal(dr["MontoTotal"].ToString()),
                                Impuesto = Convert.ToDecimal(dr["Impuesto"].ToString()),
                                Condicion = dr["Condicion"].ToString(), // Asegúrate de que esta línea esté presente
                                FechaVencimiento = dr["FechaVencimiento"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["FechaVencimiento"]) : null, // Manejo de nulos
                                FechaRegistro = dr["FechaRegistro"].ToString()
                            };
                        }
                    }

                }
                catch
                {
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
                                    IdProducto = Convert.ToInt32(dr["IdProducto"]),  // 🔹 Asigna IdProducto correctamente
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

                    // Crear el comando para ejecutar el procedimiento almacenado
                    using (SqlCommand cmd = new SqlCommand("AnularVenta", oconexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Agregar el parámetro del idVenta
                        cmd.Parameters.Add(new SqlParameter("@IdVenta", SqlDbType.Int) { Value = idVenta });

                        // Ejecutar el procedimiento almacenado para anular la venta
                        cmd.ExecuteNonQuery();
                    }

                    // Si todo salió bien
                    respuesta = true;
                    mensaje = "Factura anulada correctamente.";
                }
                catch (Exception ex)
                {
                    // Si ocurre algún error, se captura y muestra el mensaje
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

                    // Crear la consulta SQL para obtener el ID de la venta basado en el número de documento
                    string query = "SELECT IdVenta FROM VENTA WHERE NumeroDocumento = @NumeroDocumento AND Estado = 'Activa'";

                    using (SqlCommand cmd = new SqlCommand(query, oconexion))
                    {
                        cmd.Parameters.Add(new SqlParameter("@NumeroDocumento", SqlDbType.NVarChar) { Value = numeroDocumento });

                        // Ejecutar la consulta y obtener el ID de la venta
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            idVenta = Convert.ToInt32(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // En caso de error, manejar la excepción (puedes agregar un mensaje de error aquí)
                    Console.WriteLine("Error al obtener el ID de la venta: " + ex.Message);
                }
            }

            return idVenta;
        }


        public List<Venta> ObtenerFacturasCredito()
        {
            List<Venta> lista = new List<Venta>();

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    conexion.Open();
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT IdVenta, NumeroDocumento, NombreCliente, MontoTotal, FechaVencimiento, SaldoPendiente, FechaRegistro");
                    query.AppendLine("FROM VENTA");
                    query.AppendLine("WHERE Condicion = 'Credito'");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.CommandType = CommandType.Text;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Venta()
                            {
                                IdVenta = Convert.ToInt32(dr["IdVenta"]),
                                NumeroDocumento = dr["NumeroDocumento"].ToString(),
                                NombreCliente = dr["NombreCliente"].ToString(),
                                MontoTotal = dr["MontoTotal"] != DBNull.Value ? Convert.ToDecimal(dr["MontoTotal"]) : 0,
                                FechaVencimiento = dr["FechaVencimiento"] != DBNull.Value ? Convert.ToDateTime(dr["FechaVencimiento"]) : DateTime.MinValue,
                                SaldoPendiente = dr["SaldoPendiente"] != DBNull.Value ? Convert.ToDecimal(dr["SaldoPendiente"]) : 0,
                                FechaRegistro = dr["FechaRegistro"] != DBNull.Value ? dr["FechaRegistro"].ToString() : string.Empty
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener facturas a crédito: " + ex.Message);
                }
            }
            return lista;
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

    }
}