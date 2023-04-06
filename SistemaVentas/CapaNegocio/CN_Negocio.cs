using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Negocio
    {
        private CD_Negocio objcd_Negocio = new CD_Negocio();
        public Negocio ObtenerDatos()
        {
            return objcd_Negocio.ObtenerDatos();
        }
        //Llamamos el método Registrar desde la capa de datos
        public bool GuardarDatos(Negocio obj_negocio, out string Mensaje)
        {
            Mensaje = String.Empty;
            if (obj_negocio.Nombre == "")
            {
                Mensaje += "Es necesario el Nombre del Negocio\n";
            }
            if (obj_negocio.RUC == "")
            {
                Mensaje += "Es necesario el RUC del Negocio\n";
            }
            if (obj_negocio.Direccion == "")
            {
                Mensaje += "Es necesario la dirccion Negocio\n";
            }
            
            if (Mensaje != String.Empty)
            {
                return false;
            }
            else
            {
                return objcd_Negocio.GuardarDatos(obj_negocio, out Mensaje);
            }

        }
        public byte[] ObtenerLogo(out bool obtenido)
        {
            return objcd_Negocio.ObtenerLogo(out obtenido);
        }
        public bool ActualizarLogo(byte[] imagen, out string mensaje)
        {
            return objcd_Negocio.ActualizarLogo(imagen, out mensaje);
        }
    }
}
