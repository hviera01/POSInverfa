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
    public class CD_RegistroColores
    {
        public List<RegistroColores> Listar()
        {
            List<RegistroColores> lista = new List<RegistroColores>();

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT Id, Codigo, Cliente, Descripcion, UbicacionFisica, Pagina, FechaRegistro, Observaciones FROM RegistroColores");
                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new RegistroColores()
                            {
                                Id = Convert.ToInt32(dr["Id"]),
                                Codigo = dr["Codigo"] != DBNull.Value ? dr["Codigo"].ToString() : "",
                                Cliente = dr["Cliente"] != DBNull.Value ? dr["Cliente"].ToString() : "",
                                Descripcion = dr["Descripcion"] != DBNull.Value ? dr["Descripcion"].ToString() : "",
                                UbicacionFisica = dr["UbicacionFisica"] != DBNull.Value ? dr["UbicacionFisica"].ToString() : "",
                                Pagina = dr["Pagina"] != DBNull.Value ? dr["Pagina"].ToString() : "",
                                FechaRegistro = dr["FechaRegistro"] != DBNull.Value ? dr["FechaRegistro"].ToString() : "", // Como es string
                                Observaciones = dr["Observaciones"] != DBNull.Value ? dr["Observaciones"].ToString() : ""
                            });

                        }
                    }
                }

                catch (Exception ex)
                {
                    lista = new List<RegistroColores>();
                }

            }
            return lista;
        }

        public int Registrar(RegistroColores obj, out string Mensaje)
        {
            int idColorGenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarColor", oconexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("Codigo", (object)obj.Codigo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("Cliente", (object)obj.Cliente ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("Descripcion", (object)obj.Descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("UbicacionFisica", (object)obj.UbicacionFisica ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("Pagina", (object)obj.Pagina ?? DBNull.Value); // Ahora es un string
                    cmd.Parameters.AddWithValue("FechaRegistro", (object)obj.FechaRegistro ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("Observaciones", (object)obj.Observaciones ?? DBNull.Value);

                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    idColorGenerado = Convert.ToInt32(cmd.Parameters["Resultado"].Value);
                }
            }
            catch (Exception ex)
            {
                idColorGenerado = 0;
                Mensaje = ex.Message;
            }

            return idColorGenerado;
        }

        public bool Editar(RegistroColores obj, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_ModificarColor", oconexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("IdColor", obj.Id);
                    cmd.Parameters.AddWithValue("Codigo", (object)obj.Codigo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("Cliente", (object)obj.Cliente ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("Descripcion", (object)obj.Descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("UbicacionFisica", (object)obj.UbicacionFisica ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("Pagina", (object)obj.Pagina ?? DBNull.Value); // Ahora es un string
                    cmd.Parameters.AddWithValue("FechaRegistro", (object)obj.FechaRegistro ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("Observaciones", (object)obj.Observaciones ?? DBNull.Value);

                    oconexion.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    respuesta = filasAfectadas > 0;
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                Mensaje = ex.Message;
            }

            return respuesta;
        }

        public bool Eliminar(int idColor, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_EliminarColor", oconexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("IdColor", idColor);

                    oconexion.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    respuesta = filasAfectadas > 0;
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                Mensaje = ex.Message;
            }

            return respuesta;
        }

    }
}
