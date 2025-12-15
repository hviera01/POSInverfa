using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public Usuario oUsuario { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string DocumentoCliente { get; set; }
        public string NombreCliente { get; set; }
        public decimal MontoPago { get; set; }
        public decimal MontoCambio { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal SaldoPendiente { get; set; }
        public string MetodoPago { get; set; }
        public decimal Impuesto { get; set; }
        public string Condicion { get; set; }
    
        public DateTime? FechaVencimiento { get; set; }
        public List<Detalle_Venta> oDetalle_Venta { get; set; } = new List<Detalle_Venta>();

        public decimal PorcentajeTarjeta { get; set; }
        public string Estado { get; set; }
        public DateTime FechaRegistro { get; set; }

        public string Descripcion { get; set; }
        public int CodCliente { get; set; }

    }
}