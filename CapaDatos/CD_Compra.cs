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
                    cmd.Parameters.AddWithValue("IdUsuario", obj.oUsuario.IdUsuario);
                    cmd.Parameters.AddWithValue("IdProveedor", obj.oProveedor.IdProveedor);
                    cmd.Parameters.AddWithValue("TipoDocumento", obj.TipoDocumento);
                    cmd.Parameters.AddWithValue("NumeroDocumento", obj.NumeroDocumento);
                    cmd.Parameters.AddWithValue("NoFactura", obj.NoFactura);
                    cmd.Parameters.AddWithValue("MontoTotal", obj.MontoTotal);
                    cmd.Parameters.AddWithValue("MetodoPago", obj.MetodoPago);
                    cmd.Parameters.AddWithValue("Condicion", obj.Condicion);
                    cmd.Parameters.AddWithValue("SaldoPendiente", obj.SaldoPendiente);
                    cmd.Parameters.AddWithValue("FechaVencimiento", string.IsNullOrEmpty(obj.FechaVencimiento) ? (object)DBNull.Value : DateTime.ParseExact(obj.FechaVencimiento, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("DetalleCompra", DetalleCompra);
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

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
                    query.AppendLine("c.TipoDocumento, c.NumeroDocumento,c.NoFactura, c.MontoTotal, convert(char(10), c.FechaRegistro, 103) [FechaRegistro],");
                    query.AppendLine("c.MetodoPago, c.Condicion, c.SaldoPendiente, c.FechaVencimiento ");
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
                                FechaRegistro = dr["FechaRegistro"].ToString(),
                                MetodoPago = dr["MetodoPago"].ToString(),
                                Condicion = dr["Condicion"].ToString(),
                                SaldoPendiente = Convert.ToDecimal(dr["SaldoPendiente"].ToString()),
                                FechaVencimiento = dr["FechaVencimiento"] != DBNull.Value ? Convert.ToDateTime(dr["FechaVencimiento"]).ToString("dd/MM/yyyy") : null // Convertir a string
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

                    query.AppendLine("select p.Nombre, dc.PrecioCompra, dc.Cantidad, dc.MontoTotal from DetalleCompra dc");
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
                                Cantidad = Convert.ToDecimal(dr["Cantidad"].ToString()), // Cambia a decimal
                                MontoTotal = Convert.ToDecimal(dr["MontoTotal"].ToString()),
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
                    query.AppendLine("WHERE c.Condicion = 'Credito'"); // Filtrar solo compras a crédito

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
                    resultado = filasAfectadas > 0; // Si se actualizó al menos una fila, el resultado es verdadero
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
                    cmd.Parameters.AddWithValue("FechaVencimiento", compra.FechaVencimiento); // Mantener como string
                    cmd.Parameters.AddWithValue("MetodoPago", compra.MetodoPago);
                    cmd.Parameters.AddWithValue("Condicion", compra.Condicion);
                    cmd.Parameters.AddWithValue("FechaRegistro", compra.FechaRegistro); // Mantener como string
                    cmd.Parameters.AddWithValue("TipoDocumento", compra.TipoDocumento);
                    cmd.Parameters.AddWithValue("NumeroDocumento", compra.NumeroDocumento);

                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    resultado = filasAfectadas > 0; // Si se insertó al menos una fila, el resultado es verdadero
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                }
            }
            return resultado;
        }

    }
}
