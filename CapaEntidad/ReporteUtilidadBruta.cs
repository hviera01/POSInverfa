using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class ReporteUtilidadBruta
    {
        public string Fecha { get; set; }
        public string Numero { get; set; }
        public string Cliente { get; set; }
        public string Vendedor { get; set; }
        public string MetodoPago { get; set; }
        public decimal TotalVenta { get; set; }
        public decimal CostoTotal { get; set; }
        public string Condicion { get; set; }
        public decimal SaldoPendiente { get; set; }
        public string EstadoFactura { get; set; }


        public decimal UtilidadBruta { get; set; }
    }
}
