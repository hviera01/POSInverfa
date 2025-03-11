using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class ReporteVenta
    {
        public string FechaRegistro { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string TotalAPagar { get; set; } // Total a pagar
        public int CantidadProductos { get; set; } // Número de productos en la venta
        public string MetodoPago { get; set; }
        public string UsuarioRegistro { get; set; }
        public string DocumentoCliente { get; set; }
        public string NombreCliente { get; set; }
        public string Impuesto { get; set; }
        public string Condicion { get; set; }
        public string FechaVencimiento { get; set; }
        public string Estado { get; set; }
        public string ProductoMasVendido { get; set; }

    }
}
