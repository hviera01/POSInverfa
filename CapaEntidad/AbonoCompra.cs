using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class AbonoCompra
    {
        public int IdAbonoCompra { get; set; }
        public int IdCompra { get; set; }
        public decimal MontoAbono { get; set; }
        public DateTime FechaAbono { get; set; }
        public decimal NuevoSaldoPendiente { get; set; }
        public decimal InteresAplicado { get; set; }
        public string MetodoPago { get; set; }
    }
}
