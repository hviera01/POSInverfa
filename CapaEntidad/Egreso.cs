using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Egreso
    {
        public int IdEgreso { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
        public string Descripcion { get; set; }
        public string MetodoPago { get; set; }
        public string Categoria { get; set; }  // 'Casa' | 'Negocio'
        public bool EsPagado { get; set; }
        public DateTime? FechaPago { get; set; }
        public string Usuario { get; set; }
    }
}
