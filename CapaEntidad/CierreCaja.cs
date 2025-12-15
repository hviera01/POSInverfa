using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class CierreCaja
    {
        public int IdCierre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal IngresosEfectivo { get; set; }
        public decimal IngresosTarjeta { get; set; }
        public decimal IngresosTransferencia { get; set; }
        public decimal Egresos { get; set; }
        public decimal TotalCalculado { get; set; }
        public decimal TotalReal { get; set; }
        public decimal Diferencia { get; set; }
        public decimal EgresosTransferencia { get; set; }
        public decimal TotalTransferencia { get; set; }
        public decimal GranTotal { get; set; }

        public string UsuarioResponsable { get; set; }
        public string Observaciones { get; set; }
    }
}
