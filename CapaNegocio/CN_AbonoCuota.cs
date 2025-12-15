using System.Data;
using CapaDatos;

namespace CapaNegocio
{
    public class CN_AbonoCuota
    {
        private readonly CD_AbonoCuota data = new CD_AbonoCuota();

        public void RegistrarDetalle(int idAbono, int idCuota, decimal montoAplicado)
            => data.RegistrarDetalle(idAbono, idCuota, montoAplicado);

        public DataTable ObtenerDetallePorAbono(int idAbono)
            => data.ObtenerDetallePorAbono(idAbono);

        public void EliminarPorAbono(int idAbono)
            => data.EliminarPorAbono(idAbono);
    }
}
