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
    public class CN_Compra
    {
        private CD_Compra objcd_compra = new CD_Compra();

        public int ObtenerCorrelativo()
        {
            return objcd_compra.ObtenerCorrelativo();
        }

        public bool Registrar(Compra obj, DataTable DetalleCompra, out string Mensaje)
        {
            return objcd_compra.Registrar(obj, DetalleCompra, out Mensaje);
        }

        public Compra ObtenerCompra(string numero)
        {
            Compra oCompra = objcd_compra.ObtenerCompra(numero);

            if (oCompra.IdCompra != 0)
            {
                List<Detalle_Compra> oDetalleCompra = objcd_compra.ObtenerDetalleCompra(oCompra.IdCompra);
                oCompra.oDetalleCompra = oDetalleCompra;
            }
            return oCompra;
        }

        public bool ActualizarSaldoPendiente(int idCompra, decimal nuevoSaldoPendiente, out string mensaje)
        {
            return objcd_compra.ActualizarSaldoPendiente(idCompra, nuevoSaldoPendiente, out mensaje);
        }

        public List<Compra> ObtenerComprasCredito()
        {
            return objcd_compra.ObtenerComprasCredito();
        }

        public bool RegistrarCredito(Compra compra, out string mensaje)
        {
            return objcd_compra.RegistrarCredito(compra, out mensaje);
        }


    }
}
