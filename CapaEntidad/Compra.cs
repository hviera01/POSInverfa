using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Compra
    {
        public int IdCompra { get; set; }
        public Usuario oUsuario { get; set; }
        public Proveedor oProveedor { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string NoFactura { get; set; }
        public decimal Impuesto { get; set; }
        public decimal MontoTotal { get; set; }
        public List<Detalle_Compra> oDetalleCompra { get; set; }
        public string FechaRegistro { get; set; } 
        public string MetodoPago { get; set; }
        public string Condicion { get; set; }
        public decimal SaldoPendiente { get; set; }
        public string FechaVencimiento { get; set; }
        public string Estado { get; set; }
    }
}
