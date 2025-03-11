using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class ResultadoReporteVenta
    {
        public List<ReporteVenta> Ventas { get; set; }
        public string ProductoMasVendido { get; set; }
    }
}
