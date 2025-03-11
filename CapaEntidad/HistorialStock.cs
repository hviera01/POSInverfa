using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class HistorialStock
    {
        public int ID { get; set; }
        public int IdProducto { get; set; }
        public decimal Stock_Anterior { get; set; }
        public decimal Stock_Nuevo { get; set; }
        public DateTime Fecha { get; set; }
        public Usuario oUsuario { get; set; }
        public string Codigo { get; set; } // Nueva propiedad para el código
        public string Nombre { get; set; }
        public string NombreUsuario => oUsuario?.NombreCompleto;
    }
}
