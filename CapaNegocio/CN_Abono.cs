using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Abono
    {
        private CD_Abono objcd_abono = new CD_Abono();

        public bool Registrar(Abono abono, out string mensaje)
        {
            return objcd_abono.Registrar(abono, out mensaje);
        }

        public List<Abono> ObtenerHistorialAbonos(int idVenta)
        {
            return objcd_abono.ObtenerHistorialAbonos(idVenta);
        }

      
    }
}
