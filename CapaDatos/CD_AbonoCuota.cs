using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_AbonoCuota
    {
        public void RegistrarDetalle(int idAbono, int idCuota, decimal montoAplicado)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.cadena))
            using (SqlCommand cmd = new SqlCommand("sp_RegistrarAbonoCuota", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAbono", idAbono);
                cmd.Parameters.AddWithValue("@IdCuota", idCuota);
                cmd.Parameters.AddWithValue("@MontoAplicado", montoAplicado);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public DataTable ObtenerDetallePorAbono(int idAbono)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.cadena))
            using (SqlCommand cmd = new SqlCommand("sp_AbonoCuota_ObtenerDetallePorAbono", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAbono", idAbono);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public void EliminarPorAbono(int idAbono)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.cadena))
            using (SqlCommand cmd = new SqlCommand("sp_AbonoCuota_EliminarPorAbono", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAbono", idAbono);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
