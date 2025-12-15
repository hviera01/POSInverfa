using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Negocio
    {


        private CD_Negocio objcd_negocio = new CD_Negocio();


        public Negocio ObtenerDatos()
        {
            return objcd_negocio.ObtenerDatos();
        }

        public bool GuardarDatos(Negocio obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.Nombre))
            {
                Mensaje += "Es necesario el nombre\n";
            }

            if (string.IsNullOrEmpty(obj.RUC))
            {
                Mensaje += "Es necesario el número de RUC\n";
            }

            if (string.IsNullOrEmpty(obj.Direccion))
            {
                Mensaje += "Es necesario la dirección\n";
            }

            if (string.IsNullOrEmpty(obj.Telefono))
            {
                Mensaje += "Es necesario el teléfono\n";
            }

            if (obj.FechaLimiteEmision == DateTime.MinValue)
            {
                Mensaje += "Es necesario la fecha límite de emisión\n";
            }

            if (string.IsNullOrEmpty(obj.CorreoElectronico))
            {
                Mensaje += "Es necesario el correo electrónico\n";
            }

            if (string.IsNullOrEmpty(obj.CAI))
            {
                Mensaje += "Es necesario el CAI\n";
            }

            if (string.IsNullOrEmpty(obj.RangoAutorizado))
            {
                Mensaje += "Es necesario el rango autorizado\n";
            }

            if (string.IsNullOrEmpty(obj.Eslogan))
            {
                Mensaje += "Es necesario el eslogan\n";
            }

            if (!string.IsNullOrEmpty(Mensaje))
            {
                return false;
            }
            else
            {
                return objcd_negocio.GuardarDatos(obj, out Mensaje);
            }

        }

        public byte[] ObtenerLogo(out bool obtenido)
        {
            return objcd_negocio.ObtenerLogo(out obtenido);
        }

        public byte[] ObtenerLogoBlancoNegro(out bool obtenido)
        {
            return objcd_negocio.ObtenerLogoBlancoNegro(out obtenido);
        }


        public bool ActualizarLogo(byte[] imagen, out string mensaje)
        {
            return objcd_negocio.ActualizarLogo(imagen, out mensaje);
        }

        public bool ActualizarLogoBlancoNegro(byte[] imagen, out string mensaje)
        {
            return objcd_negocio.ActualizarLogoBlancoNegro(imagen, out mensaje);
        }

    }
}