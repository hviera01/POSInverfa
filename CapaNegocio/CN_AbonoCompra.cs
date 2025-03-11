using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_AbonoCompra
    {
        private CD_AbonoCompra objcd_abonoCompra = new CD_AbonoCompra();

        public bool Registrar(AbonoCompra abono, out string mensaje)
        {
            return objcd_abonoCompra.Registrar(abono, out mensaje);
        }

        public List<AbonoCompra> ObtenerHistorialAbonos(int idCompra)
        {
            return objcd_abonoCompra.ObtenerHistorialAbonos(idCompra);
        }
    }
}
