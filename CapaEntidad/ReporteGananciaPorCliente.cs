using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class ReporteGananciaPorCliente
    {
        public string NombreCliente { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal TotalCostos { get; set; }
        public decimal Ganancia { get; set; }
    }
}
