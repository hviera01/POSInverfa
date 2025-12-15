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
    public class CD_Egreso
    {
        public bool Registrar(Egreso egreso, out string mensaje)
        {
            mensaje = string.Empty;
            try
            {
                using (var con = new SqlConnection(Conexion.cadena))
                using (var cmd = new SqlCommand("sp_RegistrarEgreso", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    var fecha = (egreso.Fecha < new DateTime(1753, 1, 1)) ? DateTime.Now : egreso.Fecha;

                    cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = fecha;
                    cmd.Parameters.Add("@Monto", SqlDbType.Decimal).Value = egreso.Monto;
                    cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 500).Value = egreso.Descripcion ?? "";
                    cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 100).Value = egreso.Usuario ?? "";
                    cmd.Parameters.Add("@MetodoPago", SqlDbType.NVarChar, 50).Value = egreso.MetodoPago ?? "Efectivo";
                    cmd.Parameters.Add("@Categoria", SqlDbType.NVarChar, 20).Value = string.IsNullOrWhiteSpace(egreso.Categoria) ? "Negocio" : egreso.Categoria;
                    cmd.Parameters.Add("@EsPagado", SqlDbType.Bit).Value = egreso.EsPagado;

                    var fp = egreso.EsPagado
                             ? (object)(egreso.FechaPago ?? fecha)
                             : (object)DBNull.Value;
                    cmd.Parameters.Add("@FechaPago", SqlDbType.DateTime).Value = fp;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex) { mensaje = ex.Message; return false; }
        }



        public bool Actualizar(Egreso egreso, out string mensaje)
        {
            mensaje = string.Empty;
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.cadena))
                using (SqlCommand cmd = new SqlCommand("sp_ActualizarEgreso", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdEgreso", egreso.IdEgreso);
                    cmd.Parameters.AddWithValue("@Fecha", egreso.Fecha);
                    cmd.Parameters.AddWithValue("@Monto", egreso.Monto);
                    cmd.Parameters.AddWithValue("@Descripcion", egreso.Descripcion);
                    cmd.Parameters.AddWithValue("@MetodoPago", egreso.MetodoPago);
                    cmd.Parameters.AddWithValue("@Categoria", egreso.Categoria);
                    cmd.Parameters.AddWithValue("@EsPagado", egreso.EsPagado);
                    // 👇 compatible con C# 7.3
                    cmd.Parameters.AddWithValue("@FechaPago", (object)egreso.FechaPago ?? DBNull.Value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex) { mensaje = ex.Message; return false; }
        }


        public bool Eliminar(int idEgreso, out string mensaje)
        {
            mensaje = string.Empty;
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.cadena))
                using (SqlCommand cmd = new SqlCommand("sp_EliminarEgreso", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdEgreso", idEgreso);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex) { mensaje = ex.Message; return false; }
        }

        public DataTable ObtenerLibroFinanciero(
     DateTime fechaInicio, DateTime fechaFin,
     string vista,               // "Flujo" | "Devengado"
     string filtroTipo = null,
     string filtroDescripcion = null,
     string filtroMetodo = null,
     string filtroCategoria = null,
     bool? filtroPagado = null)
        {
            var dt = new DataTable();

            using (var con = new SqlConnection(Conexion.cadena))
            using (var cmd = new SqlCommand("sp_ObtenerLibroFinanciero_v2", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Fechas
                var pIni = cmd.Parameters.Add("@FechaInicio", SqlDbType.DateTime);
                pIni.Value = fechaInicio;
                var pFin = cmd.Parameters.Add("@FechaFin", SqlDbType.DateTime);
                pFin.Value = fechaFin;

                // Vista
                var pVista = cmd.Parameters.Add("@Vista", SqlDbType.NVarChar, 20);
                pVista.Value = vista ?? "Flujo";

                // Filtros NVARCHAR (NULL si no aplica)
                var pTipo = cmd.Parameters.Add("@FiltroTipo", SqlDbType.NVarChar, 50);
                pTipo.Value = string.IsNullOrWhiteSpace(filtroTipo) ? (object)DBNull.Value : filtroTipo;

                var pDesc = cmd.Parameters.Add("@FiltroDescripcion", SqlDbType.NVarChar, 200);
                pDesc.Value = string.IsNullOrWhiteSpace(filtroDescripcion) ? (object)DBNull.Value : filtroDescripcion;

                var pMetodo = cmd.Parameters.Add("@FiltroMetodo", SqlDbType.NVarChar, 50);
                pMetodo.Value = string.IsNullOrWhiteSpace(filtroMetodo) ? (object)DBNull.Value : filtroMetodo;

                var pCategoria = cmd.Parameters.Add("@FiltroCategoria", SqlDbType.NVarChar, 20);
                pCategoria.Value = string.IsNullOrWhiteSpace(filtroCategoria) ? (object)DBNull.Value : filtroCategoria;

                // BIT (NULL si no aplica)
                var pPagado = cmd.Parameters.Add("@FiltroPagado", SqlDbType.Bit);
                if (filtroPagado.HasValue)
                    pPagado.Value = filtroPagado.Value;   // true/false -> BIT
                else
                    pPagado.Value = DBNull.Value;

                con.Open();
                using (var rdr = cmd.ExecuteReader())
                    dt.Load(rdr);
            }

            return dt;
        }





        public decimal ObtenerTotalEgresos(DateTime fechaInicio, DateTime fechaFin)
        {
            decimal total = 0;

            using (SqlConnection con = new SqlConnection(Conexion.cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_TotalEgresos", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                con.Open();
                total = Convert.ToDecimal(cmd.ExecuteScalar());
            }

            return total;
        }

        public decimal ObtenerTotalVentasPorMetodo(string metodoPago, DateTime fechaInicio, DateTime fechaFin)
        {
            decimal total = 0;
            using (SqlConnection con = new SqlConnection(Conexion.cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_TotalVentasPorMetodo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Metodo", metodoPago);
                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                con.Open();
                total = Convert.ToDecimal(cmd.ExecuteScalar());
            }
            return total;
        }

        public decimal ObtenerTotalEgresosPorMetodo(string metodoPago, DateTime fechaInicio, DateTime fechaFin)
        {
            decimal total = 0;
            using (SqlConnection con = new SqlConnection(Conexion.cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_TotalEgresosPorMetodo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Metodo", metodoPago);
                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                con.Open();
                total = Convert.ToDecimal(cmd.ExecuteScalar());
            }
            return total;
        }


        public bool RegistrarEgresoPorAnulacion(string numeroDocumento, decimal monto, string metodoPago, string usuario, out string mensaje)
        {
            mensaje = string.Empty;
            try
            {
                using (var con = new SqlConnection(Conexion.cadena))
                using (var cmd = new SqlCommand("sp_RegistrarEgreso", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    var ahora = DateTime.Now;

                    cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = ahora;
                    cmd.Parameters.Add("@Monto", SqlDbType.Decimal).Value = monto;
                    cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 500).Value = $"Factura anulada {numeroDocumento}";
                    cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 100).Value = usuario ?? "";
                    cmd.Parameters.Add("@MetodoPago", SqlDbType.NVarChar, 50).Value = metodoPago ?? "Efectivo";
                    cmd.Parameters.Add("@Categoria", SqlDbType.NVarChar, 20).Value = "Negocio";
                    cmd.Parameters.Add("@EsPagado", SqlDbType.Bit).Value = 1;
                    cmd.Parameters.Add("@FechaPago", SqlDbType.DateTime).Value = ahora;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex) { mensaje = ex.Message; return false; }
        }

    }
}
