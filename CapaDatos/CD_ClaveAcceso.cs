using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_ClaveAcceso
    {
        public string ObtenerClave(string tipoAcceso)
        {
            string clave = string.Empty;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT Clave FROM ClaveAcceso WHERE TipoAcceso = @TipoAcceso", oconexion);
                    cmd.Parameters.AddWithValue("@TipoAcceso", tipoAcceso);
                    oconexion.Open();

                    clave = cmd.ExecuteScalar()?.ToString();
                }
                catch (Exception ex)
                {
                    // Manejo de excepciones
                    clave = string.Empty;
                }
            }

            return clave;
        }

        public bool CambiarClave(string tipoAcceso, string claveActual, string claveNueva, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("CambiarClaveAcceso", oconexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TipoAcceso", tipoAcceso);
                    cmd.Parameters.AddWithValue("@ClaveActual", claveActual);
                    cmd.Parameters.AddWithValue("@ClaveNueva", claveNueva);

                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    int result = (int)returnParameter.Value;

                    if (result == 1)
                    {
                        resultado = true;
                        mensaje = "Clave actualizada exitosamente.";
                    }
                    else
                    {
                        resultado = false;
                        mensaje = "La clave actual es incorrecta.";
                    }
                }
                catch (Exception ex)
                {
                    resultado = false;
                    mensaje = "Error: " + ex.Message;
                }
            }

            return resultado;
        }
    }
}
