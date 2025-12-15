using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Abono
    {
        private readonly CD_Abono _cd = new CD_Abono();

        public bool Registrar(Abono a, out string mensaje, out int idAbono)
            => _cd.Registrar(a, out mensaje, out idAbono);

        public List<Abono> ObtenerHistorialAbonos(int idVenta)
            => _cd.ObtenerHistorialAbonos(idVenta);

        public bool Actualizar(Abono a, out string mensaje)
            => _cd.Actualizar(a, out mensaje);

        public bool Eliminar(int idAbono, out string mensaje)
            => _cd.Eliminar(idAbono, out mensaje);
    }
}
