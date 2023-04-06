using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;


namespace CapaDatos
{
    public class CD_Permis
    {
        public List<Permiso> Listar(int idUsuario)
        {
            List<Permiso> lista = new List<Permiso>();
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("select p.IdRol, NombreMenu from PERMISOS p");
                    sb.AppendLine("INNER JOIN ROL r ON r.IdRol=p.IdRol");
                    sb.AppendLine("INNER JOIN USUARIO u ON U.IdRol=r.IdRol");
                    sb.AppendLine("WHERE u.IdUsuario = @idUsuario");

                    SqlCommand cmd = new SqlCommand(sb.ToString(), oconexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Permiso()
                            {
                                oRol = new Rol() { IdRol = Convert.ToInt32(dr["IdRol"]) },
                                NombreMenu = dr["NombreMenu"].ToString(),

                            });
                        }
                    }

                }
                catch (Exception )
                {
                    lista = new List<Permiso>();
                }
                return lista;
            }

        }
    }
}
