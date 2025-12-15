using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Cliente
    {
        private CD_Clientes objcd_Cliente = new CD_Clientes();
        public List<Cliente> Listar()
        {
            return objcd_Cliente.Listar();
        }

        public int Registrar(Cliente obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.Documento == "")
            {
                Mensaje += "Es necesario el DNI del Cliente\n";
            }

            if (obj.NombreCompleto == "")
            {
                Mensaje += "Es necesario el nombre completo del Cliente\n";
            }



            if (Mensaje != string.Empty)
            {
                return 0;
            }

            else
            {
                return objcd_Cliente.Registrar(obj, out Mensaje);
            }


        }

        public Cliente ObtenerPorId(int idCliente)
        {
            return new CD_Clientes().ObtenerPorId(idCliente);
        }


        public bool ActualizarCliente(Cliente obj, out string mensaje)
        {
            // Validación si es necesario, por ejemplo, si los puntos son negativos o algo así
            if (obj.Puntos < 0)
            {
                mensaje = "Los puntos no pueden ser negativos.";
                return false;
            }

            return objcd_Cliente.ActualizarCliente(obj, out mensaje);
        }

        public Cliente ObtenerCliente(int idCliente)
        {
            return objcd_Cliente.Listar().FirstOrDefault(c => c.idCliente == idCliente);
        }

        public bool ActualizarCliente(Cliente cliente)
        {
            string mensaje;
            return objcd_Cliente.Editar(cliente, out mensaje);
        }


        // Obtener puntos del cliente
        public int ObtenerPuntosCliente(int idCliente)
        {
            return objcd_Cliente.ObtenerPuntos(idCliente);
        }

        // Actualizar puntos del cliente
        public bool ActualizarPuntosCliente(int idCliente, int nuevosPuntos, out string mensaje)
        {
            return objcd_Cliente.ActualizarPuntos(idCliente, nuevosPuntos, out mensaje);
        }


        public bool Editar(Cliente obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.Documento == "")
            {
                Mensaje += "Es necesario el DNI del Cliente\n";
            }

            if (obj.NombreCompleto == "")
            {
                Mensaje += "Es necesario el nombre completo del Cliente\n";
            }

          


            if (Mensaje != string.Empty)
            {
                return false;
            }

            else
            {
                return objcd_Cliente.Editar(obj, out Mensaje);
            }


        }

        public bool Eliminar(Cliente obj, out string Mensaje)
        {
            return objcd_Cliente.Eliminar(obj, out Mensaje);
        }

    }
}
