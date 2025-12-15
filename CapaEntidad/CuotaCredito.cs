using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class CuotaCredito
    {
        public int IdCuota { get; set; }
        public int IdVenta { get; set; }
        public int NumeroCuota { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal MontoCuota { get; set; }
        public decimal MontoPagado { get; set; }
        public string Estado { get; set; }
    }

    public class ResumenCuotas
    {
        public int NumeroCuotas { get; set; }
        public int CuotasPagadas { get; set; }
        public int CuotaActual { get; set; }
        public DateTime? ProximoPago { get; set; }
        public bool EnMora { get; set; }
        public decimal MontoCuota { get; set; }
        public decimal Prima { get; set; }   
    }
}
