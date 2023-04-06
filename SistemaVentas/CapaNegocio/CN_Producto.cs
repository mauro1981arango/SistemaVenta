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
        private CD_Producto objcd_producto = new CD_Producto();
        public List<Producto> Listar()
        {
            return objcd_producto.Listar();
        }
        //Llamamos el método Registrar desde la capa de datos
        public int Registrar(Producto obj, out string Mensaje)
        {
            Mensaje = String.Empty;
            if (obj.Codigo == "")
            {
                Mensaje += "Es necesario insertar el código del Producto\n";
            }
            if (obj.Nombre == "")
            {
                Mensaje += "Es necesario el nombre del Producto\n";
            }
            if (obj.Descripcion == "")
            {
                Mensaje += "Es necesaria la descripcion del producto\n";
            }
            if (obj.oCategoria.IdCategoria == 0)
            {
                Mensaje += "Es necesario selecionar la categoría a la que pertenece el producto\n";
            }
            if (Mensaje != String.Empty)
            {
                return 0;
            }
            else
            {
                return objcd_producto.Registrar(obj, out Mensaje);
            }

        }
        //Llamamos el método Editar desde la capa de datos
        public bool Editar(Producto obj, out string Mensaje)
        {
            return objcd_producto.Editar(obj, out Mensaje);
        }
        //Llamamos el método Eliminar desde la capa de datos
        public bool Eliminar(Producto obj, out string Mensaje)
        {
            return objcd_producto.Eliminar(obj, out Mensaje);
        }
    }
}
