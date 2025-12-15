using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_CierreCaja
    {
        private CD_CierreCaja objcd_cierre = new CD_CierreCaja();

        public bool Registrar(CierreCaja cierre, out string mensaje)
        {
            return objcd_cierre.Registrar(cierre, out mensaje);
        }
        public (DateTime? UltimaFecha, decimal? MontoInicial) ObtenerUltimoCierre()
        {
            return new CD_CierreCaja().ObtenerUltimoCierre();
        }

        public bool GuardarMontoInicial(DateTime fechaInicio, decimal montoInicial, string usuario, out string mensaje)
        {
            return new CD_CierreCaja().GuardarMontoInicial(fechaInicio, montoInicial, usuario, out mensaje);
        }


    }
}
