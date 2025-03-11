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
    public class CD_Negocio
    {
        public Negocio ObtenerDatos() {

            Negocio obj = new Negocio();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    conexion.Open();

                    string query = "SELECT IdNegocio, Nombre, RUC, Direccion, Telefono, FechaLimiteEmision, CorreoElectronico, CAI, RangoAutorizado, RangoInicial, Eslogan, Logo, LogoBlancoNegro FROM NEGOCIO WHERE IdNegocio = 1";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            obj = new Negocio()
                            {
                                IdNegocio = int.Parse(dr["IdNegocio"].ToString()),
                                Nombre = dr["Nombre"].ToString(),
                                RUC = dr["RUC"].ToString(),
                                Direccion = dr["Direccion"].ToString(),
                                Telefono = dr["Telefono"].ToString(),
                                FechaLimiteEmision = DateTime.Parse(dr["FechaLimiteEmision"].ToString()),
                                CorreoElectronico = dr["CorreoElectronico"].ToString(),
                                CAI = dr["CAI"].ToString(),
                                RangoAutorizado = dr["RangoAutorizado"].ToString(),
                                RangoInicial = dr["RangoInicial"].ToString(),
                                Eslogan = dr["Eslogan"].ToString(),
                                Logo = dr["Logo"] as byte[],
                                LogoBlancoNegro = dr["LogoBlancoNegro"] as byte[]
                            };
                        }
                    }
                }
            }
            catch
            {
                obj = new Negocio();
            }

            return obj;
        }

        public bool GuardarDatos(Negocio objeto, out string mensaje) {

            mensaje = string.Empty;
            bool respuesta = true;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    conexion.Open();

                    StringBuilder query = new StringBuilder();
                    query.AppendLine("UPDATE NEGOCIO SET Nombre = @nombre,");
                    query.AppendLine("RUC = @ruc,");
                    query.AppendLine("Direccion = @direccion,");
                    query.AppendLine("Telefono = @telefono,");
                    query.AppendLine("FechaLimiteEmision = @fechaLimiteEmision,");
                    query.AppendLine("CorreoElectronico = @correoElectronico,");
                    query.AppendLine("CAI = @cai,");
                    query.AppendLine("RangoAutorizado = @rangoAutorizado,");
                    query.AppendLine("Rangoinicial = @rangoinicial,");
                    query.AppendLine("Eslogan = @eslogan");
                    query.AppendLine("WHERE IdNegocio = 1;");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.AddWithValue("@nombre", objeto.Nombre);
                    cmd.Parameters.AddWithValue("@ruc", objeto.RUC);
                    cmd.Parameters.AddWithValue("@direccion", objeto.Direccion);
                    cmd.Parameters.AddWithValue("@telefono", objeto.Telefono);
                    cmd.Parameters.AddWithValue("@fechaLimiteEmision", objeto.FechaLimiteEmision);
                    cmd.Parameters.AddWithValue("@correoElectronico", objeto.CorreoElectronico);
                    cmd.Parameters.AddWithValue("@cai", objeto.CAI);
                    cmd.Parameters.AddWithValue("@rangoAutorizado", objeto.RangoAutorizado);
                    cmd.Parameters.AddWithValue("@rangoinicial", objeto.RangoInicial);
                    cmd.Parameters.AddWithValue("@eslogan", objeto.Eslogan);
                    cmd.CommandType = CommandType.Text;

                    if (cmd.ExecuteNonQuery() < 1)
                    {
                        mensaje = "No se pudo guardar los datos";
                        respuesta = false;
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                respuesta = false;
            }

            return respuesta;

        }


        public byte[] ObtenerLogo(out bool obtenido) {
            obtenido = true;
            byte[] LogoBytes = new byte[0];
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    conexion.Open();
                    string query = "SELECT Logo FROM NEGOCIO WHERE IdNegocio = 1";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            LogoBytes = (byte[])dr["Logo"];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                obtenido = false;
                LogoBytes = new byte[0];
            }

            return LogoBytes;
        }

        public byte[] ObtenerLogoBlancoNegro(out bool obtenido)
        {
            obtenido = true;
            byte[] LogoBlancoNegroBytes = new byte[0];
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    conexion.Open();
                    string query = "SELECT LogoBlancoNegro FROM NEGOCIO WHERE IdNegocio = 1";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            LogoBlancoNegroBytes = (byte[])dr["LogoBlancoNegro"];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                obtenido = false;
                LogoBlancoNegroBytes = new byte[0];
            }

            return LogoBlancoNegroBytes;
        }

        public bool ActualizarLogo(byte[] image, out string mensaje) {
            mensaje = string.Empty;
            bool respuesta = true;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    conexion.Open();

                    StringBuilder query = new StringBuilder();
                    query.AppendLine("UPDATE NEGOCIO SET Logo = @imagen");
                    query.AppendLine("WHERE IdNegocio = 1;");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.AddWithValue("@imagen", image);
                    cmd.CommandType = CommandType.Text;

                    if (cmd.ExecuteNonQuery() < 1)
                    {
                        mensaje = "No se pudo actualizar el logo";
                        respuesta = false;
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                respuesta = false;
            }

            return respuesta;

        }

        public bool ActualizarLogoBlancoNegro(byte[] image, out string mensaje)
        {
            mensaje = string.Empty;
            bool respuesta = true;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    conexion.Open();

                    StringBuilder query = new StringBuilder();
                    query.AppendLine("UPDATE NEGOCIO SET LogoBlancoNegro = @imagen");
                    query.AppendLine("WHERE IdNegocio = 1;");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.AddWithValue("@imagen", image);
                    cmd.CommandType = CommandType.Text;

                    if (cmd.ExecuteNonQuery() < 1)
                    {
                        mensaje = "No se pudo actualizar el logo en blanco y negro";
                        respuesta = false;
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                respuesta = false;
            }

            return respuesta;
        }
    }
}

