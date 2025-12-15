using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class RegistroColores
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Cliente { get; set; }
        public string Descripcion { get; set; }
        public string UbicacionFisica { get; set; }
        public string Pagina { get; set; }
        public string FechaRegistro { get; set; }
        public string Observaciones { get; set; }
    }
}
