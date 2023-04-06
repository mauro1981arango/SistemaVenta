using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Proveedor
    {
        private CD_Proveedor objcd_Proveedor = new CD_Proveedor();
        public List<Proveedor> Listar()
        {
            return objcd_Proveedor.Listar();
        }
        //Llamamos el método Registrar desde la capa de datos
        public int Registrar(Proveedor obj, out string Mensaje)
        {
            Mensaje = String.Empty;
            if (obj.Documento == "")
            {
                Mensaje += "Es necesario el número de documento del Proveedor\n";
            }
            if (obj.RazonSocial == "")
            {
                Mensaje += "Es necesario el nombre completo Proveedor\n";
            }
            if (obj.Correo == "")
            {
                Mensaje += "Es necesario el correo del Proveedor\n";
            }
            if (obj.Telefono== "")
            {
                Mensaje += "Es necesario la Telefono del Proveedor\n";
            }
            if (Mensaje != String.Empty)
            {
                return 0;
            }
            else
            {
                return objcd_Proveedor.Registrar(obj, out Mensaje);
            }

        }
        //Llamamos el método Editar desde la capa de datos
        public bool Editar(Proveedor obj, out string Mensaje)
        {
            return objcd_Proveedor.Editar(obj, out Mensaje);
        }
        //Llamamos el método Eliminar desde la capa de datos
        public bool Eliminar(Proveedor obj, out string Mensaje)
        {
            return objcd_Proveedor.Eliminar(obj, out Mensaje);
        }
    }
}
