using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class ReporteUtilidadNeta
    {
        public decimal TotalVentas { get; set; }
        public decimal TotalCostos { get; set; }
        public decimal TotalEgresos { get; set; }
        public decimal UtilidadNeta { get; set; }
    }
}
