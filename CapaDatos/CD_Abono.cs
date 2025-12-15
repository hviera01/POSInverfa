using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Abono
    {
        public bool Registrar(Abono abono, out string mensaje, out int idAbonoGenerado)
        {
            bool resultado = false;
            mensaje = string.Empty;
            idAbonoGenerado = 0;

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string sql = @"
                INSERT INTO Abonos
                    (IdVenta, MontoAbono, FechaAbono, NuevoSaldoPendiente,
                     InteresAplicado, MetodoPago, FechaUltimoInteres,
                     NoRecibo, SaldoAnterior)
                VALUES
                    (@IdVenta, @MontoAbono, @FechaAbono, @NuevoSaldoPendiente,
                     @InteresAplicado, @MetodoPago, @FechaUltimoInteres,
                     @NoRecibo, @SaldoAnterior);
                SELECT SCOPE_IDENTITY();";

                    SqlCommand cmd = new SqlCommand(sql, conexion);
                    cmd.Parameters.AddWithValue("@IdVenta", abono.IdVenta);
                    cmd.Parameters.AddWithValue("@MontoAbono", abono.MontoAbono);
                    cmd.Parameters.AddWithValue("@FechaAbono", abono.FechaAbono);
                    cmd.Parameters.AddWithValue("@NuevoSaldoPendiente", abono.NuevoSaldoPendiente);
                    cmd.Parameters.AddWithValue("@InteresAplicado", abono.InteresAplicado);
                    cmd.Parameters.AddWithValue("@MetodoPago", (object)abono.MetodoPago ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaUltimoInteres", (object)abono.FechaUltimoInteres ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NoRecibo", (object)abono.NoRecibo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SaldoAnterior", abono.SaldoAnterior);

                    conexion.Open();
                    object result = cmd.ExecuteScalar();
                    idAbonoGenerado = Convert.ToInt32(result);
                    resultado = true;
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                }
            }

            return resultado;
        }


        public List<Abono> ObtenerHistorialAbonos(int idVenta)
        {
            List<Abono> listaAbonos = new List<Abono>();

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string sql = @"
                SELECT 
                    IdAbono,
                    FechaAbono,
                    MontoAbono,
                    NuevoSaldoPendiente,
                    InteresAplicado,
                    MetodoPago,
                    FechaUltimoInteres,
                    NoRecibo,
                    ISNULL(SaldoAnterior, 0) AS SaldoAnterior
                FROM Abonos
                WHERE IdVenta = @IdVenta
                ORDER BY FechaAbono ASC, IdAbono ASC;";

                    SqlCommand cmd = new SqlCommand(sql, conexion);
                    cmd.Parameters.AddWithValue("@IdVenta", idVenta);

                    conexion.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Abono abono = new Abono
                        {
                            IdAbono = reader.GetInt32(0),                       // IdAbono
                            FechaAbono = reader.GetDateTime(1),                 // FechaAbono
                            MontoAbono = reader.GetDecimal(2),                  // MontoAbono
                            NuevoSaldoPendiente = reader.GetDecimal(3),         // NuevoSaldoPendiente
                            InteresAplicado = reader.GetDecimal(4),             // InteresAplicado
                            MetodoPago = reader.IsDBNull(5) ? null : reader.GetString(5),
                            FechaUltimoInteres = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                            NoRecibo = reader.IsDBNull(7) ? null : reader.GetString(7),
                            SaldoAnterior = reader.GetDecimal(8)                // 👈 SaldoAnterior
                        };

                        listaAbonos.Add(abono);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener el historial de abonos: " + ex.Message);
                }
            }

            return listaAbonos;
        }



        public bool Actualizar(Abono abono, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string sql = @"UPDATE Abonos SET 
                              MontoAbono = @MontoAbono,
                              FechaAbono = @FechaAbono,
                              NuevoSaldoPendiente = @NuevoSaldoPendiente,
                              InteresAplicado = @InteresAplicado,
                              MetodoPago = @MetodoPago,
                              FechaUltimoInteres = @FechaUltimoInteres,
                              NoRecibo = @NoRecibo
                           WHERE IdAbono = @IdAbono";

                    SqlCommand cmd = new SqlCommand(sql, conexion);
                    cmd.Parameters.AddWithValue("@IdAbono", abono.IdAbono);
                    cmd.Parameters.AddWithValue("@MontoAbono", abono.MontoAbono);
                    cmd.Parameters.AddWithValue("@FechaAbono", abono.FechaAbono);
                    cmd.Parameters.AddWithValue("@NuevoSaldoPendiente", abono.NuevoSaldoPendiente);
                    cmd.Parameters.AddWithValue("@InteresAplicado", abono.InteresAplicado);
                    cmd.Parameters.AddWithValue("@MetodoPago", abono.MetodoPago ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaUltimoInteres", abono.FechaUltimoInteres ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NoRecibo", abono.NoRecibo ?? (object)DBNull.Value);

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

        public bool Eliminar(int idAbono, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Abonos WHERE IdAbono = @IdAbono", conexion);
                    cmd.Parameters.AddWithValue("@IdAbono", idAbono);

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

    }
}
