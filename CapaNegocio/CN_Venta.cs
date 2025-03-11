using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Venta
    {
        private CD_Venta objcd_venta = new CD_Venta();

        public bool RestarStock(int idproducto, decimal cantidad)
        {
            return objcd_venta.RestarStock(idproducto, cantidad);
        }
        public bool SumarStock(int idproducto, decimal cantidad)
        {
            return objcd_venta.SumarStock(idproducto, cantidad);
        }

        public int ObtenerCorrelativo()
        {
            return objcd_venta.ObtenerCorrelativo();
        }
        public int ObtenerCorrelativoCotizacion()
        {
            return objcd_venta.ObtenerCorrelativoCotizacion();
        }
        public int ObtenerCorrelativoVentaSinFacturar()
        {
            return objcd_venta.ObtenerCorrelativoVentaSinFacturar();
        }

        public List<Venta> ObtenerFacturasCredito()
        {
            return objcd_venta.ObtenerFacturasCredito();
        }

        public bool Registrar(Venta obj, DataTable DetalleVenta, out string Mensaje)
        {
            // Verifica el método de pago y calcula el cambio si es efectivo
            if (obj.MetodoPago == "Efectivo")
            {
                // Calcula el cambio
                obj.MontoCambio = obj.MontoPago - obj.MontoTotal;
            }
            else if (obj.MetodoPago == "Tarjeta")
            {
                // No se requiere MontoPago ni MontoCambio
                obj.MontoPago = 0;
                obj.MontoCambio = 0;
            }

            // Llamada al método de registro en la capa de datos
            return objcd_venta.Registrar(obj, DetalleVenta, out Mensaje);
        }

        public Venta ObtenerVenta(string numero)
        {
            Venta oVenta = objcd_venta.ObtenerVenta(numero);

            if (oVenta.IdVenta != 0)
            {
                List<Detalle_Venta> oDetalleVenta = objcd_venta.ObtenerDetalleVenta(oVenta.IdVenta);
                oVenta.oDetalle_Venta = oDetalleVenta;
            }

            return oVenta;
        }
        public bool ActualizarEstadoFactura(int idVenta, string estado)
        {
            return objcd_venta.ActualizarEstadoFactura(idVenta, estado);
        }
        public bool ActualizarSaldoPendiente(int idVenta, decimal TotalPagar)
        {
            return objcd_venta.ActualizarSaldoPendiente(idVenta, TotalPagar);
        }

        public bool RegistrarCredito(Venta obj, out string mensaje)
        {
            return objcd_venta.RegistrarCredito(obj, out mensaje);
        }
    }
}
