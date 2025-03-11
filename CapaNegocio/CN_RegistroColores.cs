using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_RegistroColores
    {
        private CD_RegistroColores objcd_RegistroCoolores = new CD_RegistroColores();

        public List<RegistroColores> Listar()
        {
            return objcd_RegistroCoolores.Listar();
        }

        public int Registrar(RegistroColores obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            return objcd_RegistroCoolores.Registrar(obj, out Mensaje);
        }

        public bool Editar(RegistroColores obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            return objcd_RegistroCoolores.Editar(obj, out Mensaje);
        }

        public bool Eliminar(int idColor, out string Mensaje)
        {
            return objcd_RegistroCoolores.Eliminar(idColor, out Mensaje);
        }


    }
}
