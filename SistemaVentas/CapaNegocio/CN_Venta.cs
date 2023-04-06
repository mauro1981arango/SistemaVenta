using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Venta
    {
        private CD_Venta objCD_Venta = new CD_Venta();
        public bool RestarStock(int idproducto, int cantidad)
        {
            return objCD_Venta.RestarStock(idproducto, cantidad);
        }
        public bool SumarStock(int idproducto, int cantidad)
        {
            return objCD_Venta.SumarStock(idproducto, cantidad);
        }
        public int obtenerCorrelativo()
        {
            return objCD_Venta.obtenerCorrelativo();
        }

        //Llamamos el método Registrar desde la capa de datos
        public bool Registrar(Venta obj, DataTable DetalleVenta, out string Mensaje)
        {
            return objCD_Venta.Registrar(obj, DetalleVenta, out Mensaje);
        }
        public Venta ObtenerVenta(string numero)
        {
            Venta oVenta = objCD_Venta.ObtenerVenta(numero);
            if(oVenta.IdVenta != 0)
            {
                List<Detalle_Venta> detalle_Venta = objCD_Venta.ObtenerDetalleVenta(oVenta.IdVenta);
                oVenta.oDetalleVenta = detalle_Venta;
            }
            return oVenta;
        }
    }
}
