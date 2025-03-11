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
                                NumeroDocumento = dr["NumeroDocumento"].ToString(),
                                MontoTotal = dr["MontoTotal"].ToString(),
                                UsuarioRegistro = dr["UsuarioRegistro"].ToString(),
                                DocumentoProveedor = dr["DocumentoProveedor"].ToString(),
                                RazonSocial = dr["RazonSocial"].ToString(),
                                MetodoPago = dr["MetodoPago"].ToString(),
                                Condicion = dr["Condicion"].ToString(),
                                FechaVencimiento = dr["FechaVencimiento"].ToString(),
                                CantidadProductos = Convert.ToInt32(dr["CantidadProductos"]) // Asegúrate de que esta propiedad esté en la clase ReporteCompra
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Aquí puedes registrar el error en un log o lanzar la excepción
                    throw new Exception("Error al obtener el reporte de compras: " + ex.Message);
                }
            }

            return lista;
        }


        public List<ReporteVenta> Venta(string fechainicio, string fechafin, out string productoMasVendido)
        {
            List<ReporteVenta> lista = new List<ReporteVenta>();
            productoMasVendido = string.Empty; // Inicializar el producto más vendido

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
                        // Leer los resultados de la primera consulta
                        while (dr.Read())
                        {
                            lista.Add(new ReporteVenta()
                            {
                                FechaRegistro = dr["FechaRegistro"].ToString(),
                                TipoDocumento = dr["TipoDocumento"].ToString(),
                                NumeroDocumento = dr["NumeroDocumento"].ToString(),
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

                        // Leer el resultado del producto más vendido
                        if (dr.NextResult() && dr.Read())
                        {
                            productoMasVendido = dr["Producto"].ToString(); // Obtener el nombre del producto más vendido
                        }
                    }
                }
                catch (Exception ex)
                {
                    lista = new List<ReporteVenta>();
                    // Manejo de errores
                }
            }

            return lista;

        }


    }
}
