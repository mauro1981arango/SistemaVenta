using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Categoria
    {
        private CD_Categoria objcd_Categoria = new CD_Categoria();
        public List<Categoria> Listar()
        {
            return objcd_Categoria.Listar();
        }
        //Llamamos el método Registrar desde la capa de datos
        public int Registrar(Categoria obj, out string Mensaje)
        {
            Mensaje = String.Empty;
            if (obj.Descripcion == "")
            {
                Mensaje += "Es necesario llenar el campo Descripcion\n";
            }
            
            if (Mensaje != String.Empty)
            {
                return 0;
            }
            else
            {
                return objcd_Categoria.Registrar(obj, out Mensaje);
            }

        }
        //Llamamos el método Editar desde la capa de datos
        public bool Editar(Categoria obj, out string Mensaje)
        {
            return objcd_Categoria.Editar(obj, out Mensaje);
        }
        //Llamamos el método Eliminar desde la capa de datos
        public bool Eliminar(Categoria obj, out string Mensaje)
        {
            return objcd_Categoria.Eliminar(obj, out Mensaje);
        }
    }
}
