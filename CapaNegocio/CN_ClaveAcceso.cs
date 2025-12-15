using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;

namespace CapaNegocio
{
    public class CN_ClaveAcceso
    {
        private CD_ClaveAcceso objetoDatos = new CD_ClaveAcceso();
        public bool CambiarClave(string tipoAcceso, string claveActual, string claveNueva, out string mensaje)
        {
            return objetoDatos.CambiarClave(tipoAcceso, claveActual, claveNueva, out mensaje);
        }
        public string ObtenerClave(string tipoAcceso)
        {
            return objetoDatos.ObtenerClave(tipoAcceso);
        }
    }
}
