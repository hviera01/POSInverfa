using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Producto
    {
        private CD_Producto objcd_Producto = new CD_Producto();
        public List<Producto> Listar()
        {
            return objcd_Producto.Listar();
        }

        public int Registrar(Producto obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.Codigo == "")
            {
                Mensaje += "Es necesario el codigo del Producto\n";
            }

            if (obj.Nombre == "")
            {
                Mensaje += "Es necesario el nombre del Producto\n";
            }

          

            if (Mensaje != string.Empty)
            {
                return 0;
            }

            else
            {
                return objcd_Producto.Registrar(obj, out Mensaje);
            }


        }

        public bool Editar(Producto obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.Codigo == "")
            {
                Mensaje += "Es necesario el codigo del Producto\n";
            }

            if (obj.Nombre == "")
            {
                Mensaje += "Es necesario el nombre del Producto\n";
            }

            if (Mensaje != string.Empty)
            {
                return false;
            }

            else
            {
                return objcd_Producto.Editar(obj, out Mensaje);
            }

        }

        public bool Eliminar(Producto obj, out string Mensaje)
        {
            return objcd_Producto.Eliminar(obj, out Mensaje);
        }

        public bool ActualizarStock(Producto obj, decimal stockAnterior, out string Mensaje)
        {
            Mensaje = string.Empty;
            return objcd_Producto.ActualizarStock(obj, stockAnterior, out Mensaje);
        }

        public bool ActualizarPrecio(Producto obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            return objcd_Producto.ActualizarPrecio(obj, out Mensaje);
        }

        public bool ActualizarPrecioCompra(Producto obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            return objcd_Producto.ActualizarPrecioCompra(obj, out Mensaje);
        }
        public Producto ObtenerPorCodigo(string codigo)
        {
            return new CD_Producto().ObtenerProductoPorCodigo(codigo);
        }

        public bool ActualizarDescripcion(int idProducto, string nuevaDescripcion, out string mensaje)
        {
            return new CD_Producto().ActualizarDescripcion(idProducto, nuevaDescripcion, out mensaje);
        }

        public int GuardarProductoCompleto(Producto obj, int idUsuario, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrWhiteSpace(obj.Codigo))
                Mensaje += "Es necesario el código del Producto\n";

            if (string.IsNullOrWhiteSpace(obj.Nombre))
                Mensaje += "Es necesario el nombre del Producto\n";

            if (obj.oCategoria == null || obj.oCategoria.IdCategoria == 0)
                Mensaje += "Seleccione una categoría\n";

            if (!string.IsNullOrEmpty(Mensaje))
                return 0;

            return objcd_Producto.GuardarProductoCompleto(obj, idUsuario, out Mensaje);
        }


    }
}
