using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Detalle_Venta
    {
        public int IdDetalleVenta { get; set; }
        public Producto oProducto { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Cantidad { get; set; }
        public decimal SubTotal { get; set; }
        public string FechaRegistro { get; set; }
        public string NombreProducto { get; set; }
        public decimal PrecioCompraUsado { get; set; }
        public decimal PrimaProducto { get; set; }
        public int NumeroCuotasProducto { get; set; }
        public DateTime? FechaInicioCreditoProducto { get; set; }
    }
}
