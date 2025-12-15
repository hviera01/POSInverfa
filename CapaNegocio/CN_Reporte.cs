using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Reporte
    {
        private CD_Reporte objcd_reporte = new CD_Reporte();

        public List<ReporteCompra> Compra(string fechainicio, string fechafin, int idproveedor)
        {
            return objcd_reporte.Compra(fechainicio, fechafin, idproveedor);
        }


        public List<ReporteVenta> Venta(string fechainicio, string fechafin, out string productoMasVendido)
        {
           
            return objcd_reporte.Venta(fechainicio, fechafin, out productoMasVendido);
        }

        public ResultadoReporteVenta Venta(string fechainicio, string fechafin)
        {
            string productoMasVendido;
            var lista = objcd_reporte.Venta(fechainicio, fechafin, out productoMasVendido);

            return new ResultadoReporteVenta
            {
                Ventas = lista,
                ProductoMasVendido = productoMasVendido
            };
        }

        public List<ReporteUtilidadBruta> ObtenerUtilidadBruta(string fechainicio, string fechafin, string metodoPago, string usuario, string condicion, string estadoFactura)
        {
            return objcd_reporte.ObtenerUtilidadBruta(fechainicio, fechafin, metodoPago, usuario, condicion, estadoFactura);
        }

        public List<ReporteUtilidadNeta> ObtenerUtilidadNeta(string fechainicio, string fechafin)
        {
            return objcd_reporte.ObtenerUtilidadNeta(fechainicio, fechafin);
        }


        public List<ReporteGananciaProducto> ObtenerGananciaPorProducto(string fechainicio, string fechafin)
        {
            return objcd_reporte.GananciaPorProducto(fechainicio, fechafin);
        }


        public List<ReporteGananciaPorCliente> ObtenerGananciaPorCliente(string fechainicio, string fechafin)
        {
            return objcd_reporte.ObtenerGananciaPorCliente(fechainicio, fechafin);
        }

        public List<ReporteCierreCaja> ObtenerReporteCierresCaja(string fechainicio, string fechafin)
        {
            return objcd_reporte.ObtenerReporteCierresCaja(fechainicio, fechafin);
        }

    }
}
