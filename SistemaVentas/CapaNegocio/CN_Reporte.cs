using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Reporte
    {
        private CD_Reporte objcD_Reporte = new CD_Reporte();
        public List<ReporteCompras> Compras(string fechainicio, string fechafin, int idproveedor)
        {
            return objcD_Reporte.Compras(fechainicio, fechafin, idproveedor);
        }
        public List<ReporteVentas> Ventas(string fechainicio, string fechafin)
        {
            return objcD_Reporte.Ventas(fechainicio, fechafin);
        }
    }
}
