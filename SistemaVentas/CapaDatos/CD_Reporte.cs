using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Reporte
    {
        public List<ReporteCompras> Compras(string fechainicio, string fechafin, int idproveedor)
        {
            List<ReporteCompras> lista = new List<ReporteCompras>();
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    SqlCommand cmd = new SqlCommand("sp_ReporteCompras", oconexion);
                    cmd.Parameters.AddWithValue("fechainicio", fechainicio);
                    cmd.Parameters.AddWithValue("fechafin", fechafin);
                    cmd.Parameters.AddWithValue("idproveedor", idproveedor);
                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new ReporteCompras()
                            {
                                FechaRegistro = reader["FechaRegistro"].ToString(),
                                TipoDocumento = reader["TipoDocumento"].ToString(),
                                NumeroDocumento = reader["NumeroDocumento"].ToString(),
                                MontoTotal = reader["MontoTotal"].ToString(),
                                UsuarioRegistro = reader["UsuarioRegistro"].ToString(),
                                DocumentoProveedor = reader["DocumentoProveedor"].ToString(),
                                RazonSocial = reader["RazonSocial"].ToString(),
                                CodigoProducto = reader["CodigoProducto"].ToString(),
                                NombreProducto = reader["NombreProducto"].ToString(),
                                Categoria = reader["Categoria"].ToString(),
                                PrecioCompra = reader["PrecioCompra"].ToString(),
                                PrecioVenta = reader["PrecioVenta"].ToString(),
                                Cantidad = reader["Cantidad"].ToString(),
                                SubTotal = reader["SubTotal"].ToString(),
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    lista = new List<ReporteCompras>();
                }
            }
            return lista;
        }

        public List<ReporteVentas> Ventas(string fechainicio, string fechafin)
        {
             List<ReporteVentas> lista = new List<ReporteVentas>();
            using (SqlConnection con = new SqlConnection(Conexion.cadena))
            {
                //con.Open();
                StringBuilder sb = new StringBuilder();
                SqlCommand cmd = new SqlCommand("sp_ReporteVentas", con);
                cmd.Parameters.AddWithValue("fechainicio", fechainicio);
                cmd.Parameters.AddWithValue("fechafin", fechafin);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new ReporteVentas() 
                        {
                            FechaRegistro = reader["FechaRegistro"].ToString(),
                            TipoDocumento = reader["TipoDocumento"].ToString(),
                            NumeroDocumento = reader["NumeroDocumento"].ToString(),
                            MontoTotal = reader["MontoTotal"].ToString(),
                            UsuarioRegistro = reader["UsuarioRegistro"].ToString(),
                            DocumentoCliente = reader["DocumentoCliente"].ToString(),
                            NombreCliente = reader["NombreCliente"].ToString(),
                            CodigoProducto = reader["CodigoProducto"].ToString(),
                            NombreProducto = reader["NombreProducto"].ToString(),
                            Categoria = reader["Categoria"].ToString(),
                            PrecioVenta = reader["PrecioVenta"].ToString(),
                            Cantidad = reader["Cantidad"].ToString(),
                            SubTotal = reader["SubTotal"].ToString()
                        });
                    }
                }
                try
                {
                    lista = lista.OrderBy(x => x.FechaRegistro).ToList();
                    //lista = new List<ReporteVentas>();
                }
                catch (Exception ex)
                {
                    lista = new List<ReporteVentas>();
                    Console.Write(ex.Message);
                }
            }
            return lista;
        }
    }
}
