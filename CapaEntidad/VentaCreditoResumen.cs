using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class VentaCreditoResumen
    {
        public int IdVenta { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string NumeroDocumento { get; set; }
        public int CodCliente { get; set; }
        public string NombreCliente { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal SaldoPendiente { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string Estado { get; set; }
        public string Vencida { get; set; }
        public string Descripcion { get; set; }
        public int NumeroCuotas { get; set; }
        public DateTime? ProximoPago { get; set; }
        public string Mora { get; set; }
    }
}
