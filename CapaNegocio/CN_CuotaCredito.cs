using System;
using System.Collections.Generic;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_CuotaCredito
    {
        private readonly CD_CuotaCredito _data = new CD_CuotaCredito();

        public List<CuotaCredito> ObtenerCuotasPorVenta(int idVenta)
            => _data.ObtenerPorVenta(idVenta);

        public ResumenCuotas ObtenerResumen(int idVenta)
            => _data.ObtenerResumen(idVenta);

        public ResumenCuotas ObtenerResumenPorVenta(int idVenta)
            => _data.ObtenerResumen(idVenta);

        public void ActualizarCuota(CuotaCredito c)
            => _data.ActualizarCuota(c);

        public void EliminarCuotasPorVenta(int idVenta)
            => _data.EliminarPorVenta(idVenta);

        public DateTime CalcularFechaUltimoPago(DateTime fechaInicio, string frecuencia, int numeroCuotas)
        {
            if (numeroCuotas <= 0)
                return fechaInicio;

            frecuencia = (frecuencia ?? string.Empty).Trim().ToLower();
            DateTime inicio = fechaInicio.Date;

            switch (frecuencia)
            {
                case "semanal":
                    return inicio.AddDays(7 * (numeroCuotas - 1));
                case "quincenal":
                    return inicio.AddDays(15 * (numeroCuotas - 1));
                case "mensual":
                default:
                    return inicio.AddMonths(numeroCuotas - 1);
            }
        }

        public void GenerarPlanCuotas(
            int idVenta,
            decimal saldoPendiente,
            int numeroCuotas,
            string frecuencia,
            DateTime fechaInicio)
        {
            if (idVenta <= 0 || numeroCuotas <= 0 || saldoPendiente <= 0)
                return;

            frecuencia = (frecuencia ?? string.Empty).Trim().ToLower();

            decimal cuotaBase = Math.Round(saldoPendiente / numeroCuotas, 2);
            decimal acumulado = 0m;
            DateTime fechaCuota = fechaInicio.Date;

            for (int n = 1; n <= numeroCuotas; n++)
            {
                if (n > 1)
                {
                    switch (frecuencia)
                    {
                        case "semanal":
                            fechaCuota = fechaCuota.AddDays(7);
                            break;
                        case "quincenal":
                            fechaCuota = fechaCuota.AddDays(15);
                            break;
                        case "mensual":
                        default:
                            fechaCuota = fechaCuota.AddMonths(1);
                            break;
                    }
                }

                decimal montoCuota;
                if (n < numeroCuotas)
                {
                    montoCuota = cuotaBase;
                    acumulado += cuotaBase;
                }
                else
                {
                    montoCuota = saldoPendiente - acumulado;
                    montoCuota = Math.Round(montoCuota, 2);
                    if (montoCuota < 0) montoCuota = 0;
                }

                CuotaCredito cuota = new CuotaCredito
                {
                    IdVenta = idVenta,
                    NumeroCuota = n,
                    FechaPago = fechaCuota,
                    MontoCuota = montoCuota,
                    MontoPagado = 0m,
                    Estado = "Pendiente"
                };

                _data.InsertarCuota(cuota);
            }
        }
    }
}
