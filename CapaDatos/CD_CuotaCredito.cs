using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_CuotaCredito
    {
        // ================================================================
        // OBTENER CUOTAS POR VENTA
        // ================================================================
        public List<CuotaCredito> ObtenerPorVenta(int idVenta)
        {
            var lista = new List<CuotaCredito>();

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            using (SqlCommand cmd = new SqlCommand("sp_CuotaCredito_ObtenerPorVenta", conexion))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdVenta", idVenta);

                conexion.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new CuotaCredito
                        {
                            IdCuota = Convert.ToInt32(dr["IdCuota"]),
                            IdVenta = Convert.ToInt32(dr["IdVenta"]),
                            NumeroCuota = Convert.ToInt32(dr["NumeroCuota"]),
                            FechaPago = Convert.ToDateTime(dr["FechaPago"]),
                            MontoCuota = Convert.ToDecimal(dr["MontoCuota"]),
                            MontoPagado = Convert.ToDecimal(dr["MontoPagado"]),
                            Estado = dr["Estado"].ToString()
                        });
                    }
                }
            }

            return lista;
        }

        public void EliminarPorVenta(int idVenta)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.cadena))
            using (SqlCommand cmd = new SqlCommand("sp_CuotaCredito_EliminarPorVenta", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdVenta", idVenta);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Registrar(CuotaCredito c)
        {
            using (SqlConnection cadena = new SqlConnection(Conexion.cadena))
            using (SqlCommand cmd = new SqlCommand("sp_CuotaCredito_Insertar", cadena))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdVenta", c.IdVenta);
                cmd.Parameters.AddWithValue("@NumeroCuota", c.NumeroCuota);
                cmd.Parameters.AddWithValue("@FechaPago", c.FechaPago);
                cmd.Parameters.AddWithValue("@MontoCuota", c.MontoCuota);
                cmd.Parameters.AddWithValue("@MontoPagado", c.MontoPagado);
                cmd.Parameters.AddWithValue("@Estado", c.Estado);

                cadena.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertarCuota(CuotaCredito c)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.cadena))
            using (SqlCommand cmd = new SqlCommand("sp_CuotaCredito_Insertar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdVenta", c.IdVenta);
                cmd.Parameters.AddWithValue("@NumeroCuota", c.NumeroCuota);
                cmd.Parameters.AddWithValue("@FechaPago", c.FechaPago);
                cmd.Parameters.AddWithValue("@MontoCuota", c.MontoCuota);
                cmd.Parameters.AddWithValue("@MontoPagado", c.MontoPagado);
                cmd.Parameters.AddWithValue("@Estado", c.Estado);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ================================================================
        // ACTUALIZAR CUOTA
        // ================================================================
        public void ActualizarCuota(CuotaCredito c)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.cadena))
            using (SqlCommand cmd = new SqlCommand("sp_CuotaCredito_Actualizar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdCuota", c.IdCuota);
                cmd.Parameters.AddWithValue("@MontoPagado", c.MontoPagado);
                cmd.Parameters.AddWithValue("@Estado", c.Estado);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

       
        

        public ResumenCuotas ObtenerResumen(int idVenta)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.cadena))
            using (SqlCommand cmd = new SqlCommand("sp_CuotaCredito_ObtenerResumen", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdVenta", idVenta);

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        // Usamos DateTime? explícito en el operador condicional
                        DateTime? proximoPago = dr["ProximoPago"] != DBNull.Value
                            ? (DateTime?)Convert.ToDateTime(dr["ProximoPago"])
                            : (DateTime?)null;

                        return new ResumenCuotas
                        {
                            NumeroCuotas = dr["NumeroCuotas"] != DBNull.Value
                                            ? Convert.ToInt32(dr["NumeroCuotas"])
                                            : 0,

                            CuotaActual = dr["CuotaActual"] != DBNull.Value
                                            ? Convert.ToInt32(dr["CuotaActual"])
                                            : 0,

                            ProximoPago = proximoPago,

                            MontoCuota = dr["MontoCuota"] != DBNull.Value
                                            ? Convert.ToDecimal(dr["MontoCuota"])
                                            : 0m,

                            EnMora = dr["EnMora"] != DBNull.Value
                                     && Convert.ToInt32(dr["EnMora"]) == 1
                        };
                    }
                }
            }

            // Si no hay fila, devolvemos algo vacío
            return new ResumenCuotas
            {
                NumeroCuotas = 0,
                CuotaActual = 0,
                ProximoPago = null,
                MontoCuota = 0m,
                EnMora = false
            };
        }

    }
}
