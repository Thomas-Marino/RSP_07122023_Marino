using System.Data.SqlClient;
using Entidades.Excepciones;
using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Interfaces;

namespace Entidades.DataBase
{
    public static class DataBaseManager
    {
        private static string stringConnection;
        private static SqlConnection connection;

        static DataBaseManager()
        {
            stringConnection = "Server=.;Database=20230622SP;Trusted_Connection=True;";
        }

        #region "Métodos"
        /// <summary>
        /// Método encargado de consultar la base de datos para obtener la ruta de la imagen 
        /// asociada a un tipo de comida dentro de la BD.
        /// </summary>
        /// <param name="tipo">Tipo de comida a buscar en la base de datos.</param>
        /// <returns>
        /// Si el tipo de comida ingresado coincide con un tipo de comida dentro de la base de datos
        /// el método retornará la url de la imagen de ese tipo de comida como string.
        /// </returns>
        /// <exception cref="DataBaseManagerException"></exception>
        public static string GetImagenComida(string tipo)
        {
            try
            {
                using (connection = new SqlConnection(DataBaseManager.stringConnection))
                {
                    string query = "SELECT imagen FROM comidas WHERE tipo_comida=@tipo_comida";
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("tipo_comida", tipo);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        return reader.GetString(0);
                    }
                    else
                    {
                        throw new ComidaInvalidaExeption("No existe el tipo de comida ingresado.");
                    }
                }
            }
            catch (ComidaInvalidaExeption ex)
            {
                FileManager.Guardar(ex.Message, "logs", true);
                throw ex;
            }
            catch (Exception ex)
            {
                FileManager.Guardar(ex.Message, "logs", true);
                throw new DataBaseManagerException("Error al leer la base de datos intentando obtener la imagen de la comida.");
            }
        }
        /// <summary>
        /// Método encargado de guardar en la base de datos la información de la comida preparada y del empleado que la preparó.
        /// </summary>
        /// <typeparam name="T">Tipo de objeto que implementa la interfaz IComestible y tiene un constructor sin parámetros.</typeparam>
        /// <param name="nombreEmpleado">Nombre del empleado encargado de preparar la comida.</param>
        /// <param name="comida">Comida a ser preparada.</param>
        /// <returns>
        /// True si el ticket pudo ser guardado en la base de datos.
        /// Caso contrario el método arrojará una excepción.
        /// </returns>
        /// <exception cref="DataBaseManagerException"></exception>
        public static bool GuardarTicket<T>(string nombreEmpleado, T comida) where T : IComestible, new()
        {
            try
            {
                using (connection = new SqlConnection(DataBaseManager.stringConnection))
                {
                    string query = "INSERT INTO tickets (empleado,ticket)" +
                            "values (@empleado,@ticket)";
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("empleado", nombreEmpleado);
                    command.Parameters.AddWithValue("ticket", comida.Ticket);

                    connection.Open();
                    command.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception ex)
            {
                FileManager.Guardar(ex.Message, "logs", true);
                throw new DataBaseManagerException("Error al modificar la base de datos intentando guardar un ticket.");
            }
        }
        #endregion
    }
}
