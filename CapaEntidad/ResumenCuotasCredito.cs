using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class ResumenCuotasCredito
    {
        public int NumeroCuotas { get; set; }
        public int CuotaActual { get; set; }
        public DateTime? ProximoPago { get; set; }
        public decimal MontoCuota { get; set; }
        public bool EnMora { get; set; }
    }

}
