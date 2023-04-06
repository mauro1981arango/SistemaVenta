using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Cliente
    {
        private CD_Cliente objcd_Cliente = new CD_Cliente();
        public List<Cliente> Listar()
        {
            return objcd_Cliente.Listar();
        }
        //Llamamos el método Registrar desde la capa de datos
        public int Registrar(Cliente obj, out string Mensaje)
        {
            Mensaje = String.Empty;
            if (obj.Documento == "")
            {
                Mensaje += "Es necesario el número de documento del Cliente\n";
            }
            if (obj.NombreCompleto == "")
            {
                Mensaje += "Es necesario el nombre completo Cliente\n";
            }
            if (obj.Correo == "")
            {
                Mensaje += "Es necesario el correo del Cliente\n";
            }
            if (obj.Telefono == "")
            {
                Mensaje += "Es necesario el Telefono del Cliente\n";
            }
            if (Mensaje != String.Empty)
            {
                return 0;
            }
            else
            {
                return objcd_Cliente.Registrar(obj, out Mensaje);
            }

        }
        //Llamamos el método Editar desde la capa de datos
        public bool Editar(Cliente obj, out string Mensaje)
        {
            return objcd_Cliente.Editar(obj, out Mensaje);
        }
        //Llamamos el método Eliminar desde la capa de datos
        public bool Eliminar(Cliente obj, out string Mensaje)
        {
            return objcd_Cliente.Eliminar(obj, out Mensaje);
        }
    }
}
