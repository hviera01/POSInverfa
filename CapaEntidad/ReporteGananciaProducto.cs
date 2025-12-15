using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class ReporteGananciaProducto
    {
        public string NombreProducto { get; set; }
        public int CantidadVendida { get; set; }
        public string CostoTotal { get; set; }
        public string TotalVentas { get; set; }
        public string Ganancia { get; set; }
    }
}
