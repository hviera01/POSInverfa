using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class ReporteCierreCaja
    {
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public string MontoInicial { get; set; }
        public string IngresosEfectivo { get; set; }
        public string IngresosTarjeta { get; set; }
        public string IngresosTransferencia { get; set; }
        public string Egresos { get; set; }
        public string EgresosTransferencia { get; set; }
        public string TotalCalculado { get; set; }
        public string TotalTransferencia { get; set; }
        public string GranTotal { get; set; }
        public string TotalReal { get; set; }
        public string Diferencia { get; set; }
        public string UsuarioResponsable { get; set; }
        public string Observaciones { get; set; }
    }
}
