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
   public class CD_Compra
    {
        //Creamos un método que nos permita obtener un correlativo para cada una de las compras que se registren en la base de datos, ejemplo 00001, 00002.
        public int obtenerCorrelativo()
        {
            int idcorrelativo = 0;
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT count(*) + 1 from COMPRA");
                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);                  
                    cmd.CommandType = CommandType.Text;
                   
                    oconexion.Open();
                    idcorrelativo = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                }
                catch (Exception)
                {
                    idcorrelativo = 0;
                }
            }
            return idcorrelativo;
        }
        //Creamos un método Registrar Compra de tipo bool que recibe los siguientes parametoros (Compra obj_compra, DataTable DetalleCompra, out string Mensaje)
        public bool Registrar(Compra obj, DataTable DetalleCompra, out string Mensaje)
        {
            bool Respuesta = false;
            Mensaje = string.Empty;
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    //Le pasamos al sqlcomand el procedimento almacenado de la base de datos para registrar una compra.
                    SqlCommand cmd = new SqlCommand("SP_Registrar_Compra", oconexion);
                    cmd.Parameters.AddWithValue("IdUsuario", obj.oUsuario.IdUsuario);
                    cmd.Parameters.AddWithValue("IdProveedor", obj.oProveedor.IdProveedor);
                    cmd.Parameters.AddWithValue("TipoDocumento", obj.TipoDocumento);
                    cmd.Parameters.AddWithValue("NumeroDocumento", obj.NumeroDocumento);
                    cmd.Parameters.AddWithValue("MontoTotal", obj.MontoTotal);
                    cmd.Parameters.AddWithValue("DetalleCompra", DetalleCompra);
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                    Respuesta = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                }
                catch (Exception ex)
                {
                    Mensaje = ex.Message;
                    Respuesta = false;
                }
            }
            return Respuesta;
        }
        //Creamos el método para obtener compra.
        public Compra ObtenerCompra(string numero)
        {
            //Creamos un bojeto Compra.
            Compra obj_compra = new Compra();
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select c.IdCompra,");
                    query.AppendLine("u.NombreCompleto,");
                    query.AppendLine("pro.Documento, pro.RazonSocial,");
                    query.AppendLine("c.TipoDocumento, c.NumeroDocumento, c.MontoTotal, convert (char (10), c.FechaRegistro,103)[FechaRegistro]");
                    query.AppendLine("from COMPRA c");
                    query.AppendLine("inner join USUARIO u ON u.IdUsuario = c.IdUsuario");
                    query.AppendLine("inner join PROVEEDOR pro ON  pro.IdProveedor = c.IdProveedor");
                    query.AppendLine("where c.NumeroDocumento = @numero");

                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);
                    //oconexion.Open();
                    //Especificamos el parámetro que vamos a recibir numero.
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            obj_compra = new Compra()
                            {
                                IdCompra = Convert.ToInt32(reader["IdCompra"]),
                                oUsuario = new Usuario() { NombreCompleto = reader["NombreCompleto"].ToString()},
                                oProveedor = new Proveedor() { Documento = reader["Documento"].ToString(), RazonSocial = reader["RazonSocial"].ToString() },
                                TipoDocumento = reader["TipoDocumento"].ToString(),
                                NumeroDocumento = reader["NumeroDocumento"].ToString(),
                                MontoTotal = Convert.ToDecimal( reader["MontoTotal"].ToString()),
                                FechaRegistro = reader["FechaRegistro"].ToString()
                            };
                        }
                    }
                }
                catch (Exception)
                {
                    obj_compra = new Compra();
                }
            }
            return obj_compra;
        }
        //Creamos el método detalle Compra.
        public List<Detalle_Compra> ObtenerDetalleCompra(int idcompra)
        {
            List<Detalle_Compra> olista = new List<Detalle_Compra>();
            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select p.Nombre, dc.PrecioCompra, dc.Cantidad, dc.MontoTotal from DETALLE_COMPRA dc");
                    query.AppendLine("inner join PRODUCTO p ON p.IdProducto = dc.IdProducto");
                    query.AppendLine("where dc.IdCompra = @idcompra");
                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.AddWithValue("@idcompra", idcompra);
                    cmd.CommandType = System.Data.CommandType.Text;
                    conexion.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            olista.Add(new Detalle_Compra()
                            {
                                oProducto = new Producto() { Nombre = reader["Nombre"].ToString()  },
                                PrecioCompra = Convert.ToDecimal(reader["PrecioCompra"]),
                                Cantidad = Convert.ToInt32(reader["Cantidad"]),
                                MontoTotal = Convert.ToDecimal(reader["MontoTotal"]),
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    olista = new List<Detalle_Compra>();
                }
            }
            return olista;
        }
    }
}
