using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;
using CapaDatos;

namespace CapaDatos
{
    public class CD_Negocio
    {
        //Creamos el método para obtener los datos del negocio desde la base de datos.
        public Negocio ObtenerDatos()
        {
            
            Negocio objnegocio = new Negocio();
            try
            {
                //Hacemos el llamado a la cadena de conexion y ejecutamos el query.
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    conexion.Open();
                    string query = "SELECT IdNegocio,Nombre,RUC,Direccion FROM NEGOCIO WHERE IdNegocio = 1";
                    SqlCommand cmd = new SqlCommand(query,conexion);
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        //Con un ciclo while recorremos el objeto negocio.
                        while (dr.Read())
                        {
                            objnegocio = new Negocio()
                            {
                                IdNegocio = int.Parse(dr["IdNegocio"].ToString()),
                                Nombre = dr["Nombre"].ToString(),
                                RUC = dr["RUC"].ToString(),
                                Direccion = dr["Direccion"].ToString()
                            };
                        }
                    }
                }
            }catch 
            {                
                objnegocio = new Negocio();
            }
            return objnegocio;
        }

        //Creamos el método que nos va servir para guardar los datos.
        public bool GuardarDatos(Negocio obj_negocio, out string mensaje)
        {
            mensaje = String.Empty;
            bool respuesta = true;
            try
            {
                //Hacemos el llamado a la cadena de conexion y ejecutamos el query.
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    conexion.Open();
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("UPDATE NEGOCIO SET Nombre = @nombre,");
                    query.AppendLine("RUC = @ruc,");
                    query.AppendLine("Direccion = @direccion");
                    query.AppendLine("WHERE IdNegocio = 1;");
                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.AddWithValue("@nombre",obj_negocio.Nombre);
                    cmd.Parameters.AddWithValue("@ruc", obj_negocio.RUC);
                    cmd.Parameters.AddWithValue("@direccion", obj_negocio.Direccion);
                    cmd.CommandType = CommandType.Text;
                    //Validamos si la ejecucion del query va ser correcta o no.
                    if (cmd.ExecuteNonQuery() < 1)
                    {
                        mensaje = "No se pudieron guardar los datos";
                        respuesta = false;  
                    }     
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                respuesta=false;
            }
            return respuesta;
        }
        //Creamos el metodo que nos va permitir obtener el logo desde el directorio
        //donde tengamos guardada la imagen. Esto facilitará actualizar el logo del negocio.
        //Creamos un array de byte.
        public byte[] ObtenerLogo(out bool obtenido)
        {
            obtenido = true;
            byte[] LogoBytes = new byte[0];
            try
            {
                //Hacemos el llamado a la cadena de conexion y ejecutamos el query.
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    conexion.Open();
                    string query = "SELECT Logo FROM NEGOCIO WHERE IdNegocio = 1";
                    SqlCommand cmd = new SqlCommand(query, conexion);                 
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        //Con un ciclo while recorremos el objeto negocio.
                        while (dr.Read())
                        {
                            LogoBytes = (byte[])dr["Logo"];
                            
                        }
                    }
                }
            }
            catch (Exception)
            {
                obtenido=false;
                LogoBytes = new byte[0];
            }
            return LogoBytes;
        } 
        //Creamos el método que nos permmite actualizar el logo del negocio.
        public bool ActualizarLogo(byte[] image, out string mensaje)
        {
            mensaje = String.Empty;
            bool respuesta = true;
            try
            {
                //Hacemos el llamado a la cadena de conexion y ejecutamos el query.
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    conexion.Open();
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("UPDATE NEGOCIO SET Logo = @imagen");
                    query.AppendLine("WHERE IdNegocio = 1");
                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.AddWithValue("@imagen", image);
                    cmd.CommandType = CommandType.Text;
                    //Validamos si la ejecucion del query va ser correcta o no.
                    if (cmd.ExecuteNonQuery() < 1)
                    {
                        mensaje = "No se pudo actualizar el logo";
                        respuesta = false;
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                respuesta = false;
            }
            return respuesta;
        }
    }
}
