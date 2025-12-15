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
    public class CD_CierreCaja
    {
        public bool Registrar(CierreCaja cierre, out string mensaje)
        {
            mensaje = string.Empty;
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.cadena))
                using (SqlCommand cmd = new SqlCommand("sp_RegistrarCierreCaja", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FechaInicio", cierre.FechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", cierre.FechaFin);
                    cmd.Parameters.AddWithValue("@MontoInicial", cierre.MontoInicial);
                    cmd.Parameters.AddWithValue("@IngresosEfectivo", cierre.IngresosEfectivo);
                    cmd.Parameters.AddWithValue("@IngresosTarjeta", cierre.IngresosTarjeta);
                    cmd.Parameters.AddWithValue("@IngresosTransferencia", cierre.IngresosTransferencia);
                    cmd.Parameters.AddWithValue("@Egresos", cierre.Egresos);
                    cmd.Parameters.AddWithValue("@EgresosTransferencia", cierre.EgresosTransferencia);
                    cmd.Parameters.AddWithValue("@TotalCalculado", cierre.TotalCalculado);
                    cmd.Parameters.AddWithValue("@TotalTransferencia", cierre.TotalTransferencia);
                    cmd.Parameters.AddWithValue("@GranTotal", cierre.GranTotal);
                    cmd.Parameters.AddWithValue("@TotalReal", cierre.TotalReal);
                    cmd.Parameters.AddWithValue("@Diferencia", cierre.Diferencia);
                    cmd.Parameters.AddWithValue("@UsuarioResponsable", cierre.UsuarioResponsable);
                    cmd.Parameters.AddWithValue("@Observaciones", cierre.Observaciones);

                    con.Open();
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


        public (DateTime? UltimaFecha, decimal? MontoInicial) ObtenerUltimoCierre()
        {
            DateTime? fecha = null;
            decimal? monto = null;

            using (SqlConnection con = new SqlConnection(Conexion.cadena))
            using (SqlCommand cmd = new SqlCommand("sp_ObtenerUltimoCierreCaja", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        fecha = Convert.ToDateTime(dr["FechaInicio"]);
                        monto = Convert.ToDecimal(dr["MontoInicial"]);
                    }
                }
            }
            return (fecha, monto);
        }



        public bool GuardarMontoInicial(DateTime fechaInicio, decimal montoInicial, string usuario, out string mensaje)
        {
            mensaje = string.Empty;
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.cadena))
                using (SqlCommand cmd = new SqlCommand("sp_GuardarMontoInicial", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // 👇 Forzar tipo DATE y mandar .Date
                    var pFecha = cmd.Parameters.Add("@FechaInicio", SqlDbType.Date);
                    pFecha.Value = fechaInicio.Date;

                    cmd.Parameters.AddWithValue("@MontoInicial", montoInicial);
                    cmd.Parameters.AddWithValue("@UsuarioResponsable", usuario);

                    con.Open();
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


    }
}
