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
