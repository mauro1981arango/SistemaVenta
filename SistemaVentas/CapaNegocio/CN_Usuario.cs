using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Usuario
    {
        private CD_Usuario objcd_usuario = new CD_Usuario();
        public List<Usuario> Listar()
        {
            return objcd_usuario.Listar();
        }
        //Llamamos el método Registrar desde la capa de datos
        public int Registrar(Usuario obj, out string Mensaje)
        {
            Mensaje = String.Empty;
            if(obj.Documento == "")
            {
                Mensaje += "Es necesario el número de documento del usuario\n";
            }
            if (obj.NombreCompleto == "")
            {
                Mensaje += "Es necesario el nombre completo usuario\n";
            }
            if (obj.Correo == "")
            {
                Mensaje += "Es necesario el correo del usuario\n";
            }
            if (obj.Clave == "")
            {
                Mensaje += "Es necesario la clave del usuario\n";
            }
            if(Mensaje != String.Empty)
            {
                return 0;
            }
            else
            {
                return objcd_usuario.Registrar(obj, out Mensaje);
            }
            
        }
        //Llamamos el método Editar desde la capa de datos
        public bool Editar(Usuario obj, out string Mensaje)
        {
            return objcd_usuario.Editar(obj, out Mensaje);
        }
        //Llamamos el método Eliminar desde la capa de datos
        public bool Eliminar(Usuario obj, out string Mensaje)
        {
            return objcd_usuario.Eliminar(obj, out Mensaje);
        }
    }
}
