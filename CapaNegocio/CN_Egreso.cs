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
    public class CN_Egreso
    {
        private readonly CD_Egreso _repo = new CD_Egreso();

        public bool Registrar(Egreso eg, out string msg) => _repo.Registrar(eg, out msg);
        public bool Actualizar(Egreso eg, out string msg) => _repo.Actualizar(eg, out msg);
        public bool Eliminar(int id, out string msg) => _repo.Eliminar(id, out msg);

        public DataTable ObtenerLibroFinanciero(
            DateTime ini, DateTime fin, string vista,
            string tipo = null, string desc = null, string metodo = null,
            string categoria = null, bool? pagado = null)
            => _repo.ObtenerLibroFinanciero(ini, fin, vista, tipo, desc, metodo, categoria, pagado);

        public decimal ObtenerTotalVentasPorMetodo(string metodoPago, DateTime fechaInicio, DateTime fechaFin)
            => _repo.ObtenerTotalVentasPorMetodo(metodoPago, fechaInicio, fechaFin);

        public decimal ObtenerTotalEgresos(DateTime fechaInicio, DateTime fechaFin)
            => _repo.ObtenerTotalEgresos(fechaInicio, fechaFin);

        public decimal ObtenerTotalEgresosPorMetodo(string metodoPago, DateTime fechaInicio, DateTime fechaFin)
            => _repo.ObtenerTotalEgresosPorMetodo(metodoPago, fechaInicio, fechaFin);
    }
}
