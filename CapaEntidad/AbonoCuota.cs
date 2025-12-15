using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class AbonoCuota
    {
        public int IdAbonoCuota { get; set; }
        public int IdAbono { get; set; }
        public int IdCuota { get; set; }
        public decimal MontoAplicado { get; set; }
    }
}
