using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_AbonoCompra
    {
        public bool Registrar(AbonoCompra abono, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO AbonoCompra (IdCompra, MontoAbono, FechaAbono, NuevoSaldoPendiente, InteresAplicado, MetodoPago) VALUES (@IdCompra, @MontoAbono, @FechaAbono, @NuevoSaldoPendiente, @InteresAplicado, @MetodoPago)", conexion);
                    cmd.Parameters.AddWithValue("@IdCompra", abono.IdCompra);
                    cmd.Parameters.AddWithValue("@MontoAbono", abono.MontoAbono);
                    cmd.Parameters.AddWithValue("@FechaAbono", abono.FechaAbono);
                    cmd.Parameters.AddWithValue("@NuevoSaldoPendiente", abono.NuevoSaldoPendiente);
                    cmd.Parameters.AddWithValue("@InteresAplicado", abono.InteresAplicado);
                    cmd.Parameters.AddWithValue("@MetodoPago", abono.MetodoPago ?? (object)DBNull.Value); // Manejo de nulos

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                    resultado = true;
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                }
            }

            return resultado;
        }

        public List<AbonoCompra> ObtenerHistorialAbonos(int idCompra)
        {
            List<AbonoCompra> listaAbonos = new List<AbonoCompra>();

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT FechaAbono, MontoAbono, NuevoSaldoPendiente, InteresAplicado, MetodoPago FROM AbonoCompra WHERE IdCompra = @IdCompra", conexion);
                    cmd.Parameters.AddWithValue("@IdCompra", idCompra);

                    conexion.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        listaAbonos.Add(new AbonoCompra
                        {
                            FechaAbono = reader.GetDateTime(0),
                            MontoAbono = reader.GetDecimal(1),
                            NuevoSaldoPendiente = reader.GetDecimal(2),
                            InteresAplicado = reader.GetDecimal(3),
                            MetodoPago = reader.IsDBNull(4) ? null : reader.GetString(4) // Manejo de nulos
                        });
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener el historial de abonos: " + ex.Message);
                }
            }

            return listaAbonos;
        }
    }
}
