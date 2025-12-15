using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Venta
    {
        private CD_Venta objcd_venta = new CD_Venta();

        public int ObtenerCorrelativo()
        {
            return objcd_venta.ObtenerCorrelativo();
        }

        public bool RestarStock(int idproducto, decimal cantidad)
        {
            return objcd_venta.RestarStock(idproducto, cantidad);
        }
        public bool SumarStock(int idproducto, decimal cantidad)
        {
            return objcd_venta.SumarStock(idproducto, cantidad);
        }

        public int ObtenerCorrelativoCotizacion()
        {
            return objcd_venta.ObtenerCorrelativoCotizacion();
        }
        public int ObtenerCorrelativoVentaSinFacturar()
        {
            return objcd_venta.ObtenerCorrelativoVentaSinFacturar();
        }

        public int ObtenerCorrelativoCanje()
        {
            return objcd_venta.ObtenerCorrelativoCanje();
        }

        public List<VentaCreditoResumen> ObtenerFacturasCredito()
        {
            return objcd_venta.ObtenerFacturasCredito();
        }

        public int Registrar(Venta obj, DataTable DetalleVenta, out string Mensaje)
        {
            Mensaje = "";

            if (obj.MetodoPago == "Efectivo")
            {
                obj.MontoCambio = obj.MontoPago - obj.MontoTotal;
            }
            else if (obj.MetodoPago == "Tarjeta")
            {
                obj.MontoPago = 0;
                obj.MontoCambio = 0;
            }

            // retorna Id generado (int)
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


        public List<Venta> ObtenerVentasPorProducto(int idProducto)
        {
            return objcd_venta.ObtenerVentasPorProducto(idProducto);
        }
        public DataTable ObtenerPreparacionPorVenta(int idVenta)
        {
            return new CD_Venta().ObtenerPreparacionPorVenta(idVenta);
        }
        public int ObtenerIdVentaPorNumeroDocumento(string numero)
        {
            return new CD_Venta().ObtenerIdVentaPorNumeroDocumento(numero);
        }
        public bool EliminarVentaCredito(int idVenta, out string mensaje)
            => objcd_venta.EliminarVentaCredito(idVenta, out mensaje);

        public Venta ObtenerVentaCreditoPorId(int idVenta)
            => objcd_venta.ObtenerVentaCreditoPorId(idVenta);
        public bool EliminarCreditoCompleto(int idVenta, out string mensaje)
        {
            return new CD_Venta().EliminarCreditoCompleto(idVenta, out mensaje);
        }
        public bool ActualizarDatosCredito(Venta v, out string mensaje)
        {
            return new CD_Venta().ActualizarDatosCredito(v, out mensaje);
        }
        public bool ExisteNumeroDocumento(string numeroDocumento)
        {
            return objcd_venta.ExisteNumeroDocumento(numeroDocumento);
        }

    }
}
