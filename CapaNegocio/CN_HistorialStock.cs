using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_HistorialStock
    {
        private CD_HistorialStock objcd_HistorialStock = new CD_HistorialStock();

        public List<HistorialStock> Listar(int idProducto)
        {
            return new CD_HistorialStock().Listar(idProducto);
        }
    }
}
