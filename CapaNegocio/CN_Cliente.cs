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
