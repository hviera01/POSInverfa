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
    public class CD_HistorialStock
    {
        public List<HistorialStock> Listar(int idProducto)
        {
            List<HistorialStock> lista = new List<HistorialStock>();

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT hs.ID, hs.IdProducto, hs.Stock_Anterior, hs.Stock_Nuevo, hs.Fecha,");
                    query.AppendLine("u.NombreCompleto AS Usuario, p.Codigo, p.Nombre, hs.TipoMovimiento, hs.NumeroDocumento");
                    query.AppendLine("FROM HistorialStock hs");
                    query.AppendLine("INNER JOIN Producto p ON hs.IdProducto = p.IdProducto");
                    query.AppendLine("INNER JOIN USUARIO u ON u.IdUsuario = hs.IdUsuario");
                    query.AppendLine("WHERE hs.IdProducto = @IdProducto");
                    query.AppendLine("ORDER BY hs.Fecha DESC");

                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);
                    cmd.Parameters.AddWithValue("@IdProducto", idProducto);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new HistorialStock()
                            {
                                ID = Convert.ToInt32(dr["ID"]),
                                IdProducto = Convert.ToInt32(dr["IdProducto"]),
                                Codigo = dr["Codigo"].ToString(),
                                Nombre = dr["Nombre"].ToString(),
                                Stock_Anterior = Convert.ToDecimal(dr["Stock_Anterior"]),
                                Stock_Nuevo = Convert.ToDecimal(dr["Stock_Nuevo"]),
                                Fecha = Convert.ToDateTime(dr["Fecha"]),
                                TipoMovimiento = dr["TipoMovimiento"].ToString(),
                                NumeroDocumento = dr["NumeroDocumento"] == DBNull.Value ? "" : dr["NumeroDocumento"].ToString(),
                                oUsuario = new Usuario() { NombreCompleto = dr["Usuario"].ToString() }
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    lista = new List<HistorialStock>();
                }
            }

            return lista;
        }
    }
}
