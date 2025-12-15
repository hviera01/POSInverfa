using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Abono
    {
        public int IdAbono { get; set; }
        public int IdVenta { get; set; }
        public decimal MontoAbono { get; set; }
        public DateTime FechaAbono { get; set; }
        public decimal NuevoSaldoPendiente { get; set; }
        public decimal InteresAplicado { get; set; }
        public string MetodoPago { get; set; }
        public DateTime? FechaUltimoInteres { get; set; }
        public string NoRecibo { get; set; }
        public decimal SaldoAnterior { get; set; }
    }
}
