using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace CapaNegocio
{
    public class CN_Compra
    {
        private CD_Compra objCD_Compra = new CD_Compra();
        public int obtenerCorrelativo()
        {
            return objCD_Compra.obtenerCorrelativo();
        }
        
        //Llamamos el método Registrar desde la capa de datos
        public bool Registrar(Compra obj, DataTable DetalleCompra, out string Mensaje)
        {    
           return objCD_Compra.Registrar(obj, DetalleCompra, out Mensaje);
        }
       public Compra ObtenerCompra(string numero)
        {
            Compra oCompra = objCD_Compra.ObtenerCompra(numero);
            if (oCompra.IdCompra != 0)
            {
                List<Detalle_Compra> oDetalleCompra = objCD_Compra.ObtenerDetalleCompra(oCompra.IdCompra);
                oCompra.oDetalleCompra = oDetalleCompra;
                
            }
            return oCompra;
        }
    }
}
