using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Negocio
    {
        public int IdNegocio { get; set; }
        public string Nombre { get; set; }
        public string RUC { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; } 
        public DateTime FechaLimiteEmision { get; set; } 
        public string CorreoElectronico { get; set; } 
        public string CAI { get; set; } 
        public string RangoAutorizado { get; set; }
        public string RangoInicial { get; set; } 
        public string Eslogan { get; set; } 
        public byte[] Logo { get; set; } 
        public byte[] LogoBlancoNegro { get; set; }
    }
}