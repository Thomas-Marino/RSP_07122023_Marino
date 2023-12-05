using Entidades.Exceptions;
using Entidades.Interfaces;
using Entidades.Modelos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Entidades.Files
{
    
    public static class FileManager
    {
        private static string path;

        static FileManager()
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "20231207_Marino");
            ValidaExistenciaDeDirectorio();
        }

        #region "Metodos"
        /// <summary>
        /// Método encargado de verificar que exista un directorio vital en el cual se almacenaran los archivos
        /// que creará el programa. En el caso de no existir, los creará.
        /// </summary>
        /// <exception cref="FileManagerException"></exception>
        private static void ValidaExistenciaDeDirectorio()
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    Guardar(ex.Message, "logs", true);
                    throw new FileManagerException("Error al crear el directorio");
                }
            }
        }
        /// <summary>
        /// Método encargado de guardar los datos indicados por el usuario dentro de un archivo .txt.
        /// </summary>
        /// <param name="data">Datos a guardar.</param>
        /// <param name="nombreArchivo">Nombre del archivo en el que se guardaran los datos.</param>
        /// <param name="append">True para añadir los datos sin sobreescribir el archivo, false para añadir los datos sobreescribiendo el arhivo.</param>
        /// <exception cref="FileManagerException"></exception>
        public static void Guardar(string data, string nombreArchivo, bool append)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(path, $"{nombreArchivo}.txt"), append))
                {
                    sw.Write($"{data}\n");
                }
            }
            catch (Exception ex) 
            {
                Guardar(ex.Message, "logs", true);
                throw new FileManagerException("Errror al guardar los datos");
            }
        }
        /// <summary>
        /// Método encargado de serializar y almacenar los datos de un elemento generico 
        /// que solo aceptara tipos por referencia.
        /// </summary>
        /// <typeparam name="T">Tipo de objeto a serializar</typeparam>
        /// <param name="elemento">Elemento a serializar.</param>
        /// <param name="nombreArchivo">Nombre del archivo en el que se almacenaran los datos del elemento.</param>
        /// <returns>True si pudo serializar los datos del elemento correctamente.
        /// Caso contrario lanza una excepción.</returns>
        /// <exception cref="FileManagerException"></exception>
        public static bool Serializar<T>(T elemento, string nombreArchivo) where T : class
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions();
                int numeroArchivo = 1;
                string pathArchivo = Path.Combine(path, $"{nombreArchivo}.json");

                options.WriteIndented = true;

                while (File.Exists(pathArchivo))
                {
                    numeroArchivo += 1;
                    pathArchivo = Path.Combine(path, $"{nombreArchivo}_{numeroArchivo}.json");
                }

                using (StreamWriter sw = new StreamWriter(pathArchivo))
                {
                    string datosJson = JsonSerializer.Serialize(elemento, options);
                    sw.Write(datosJson);
                }
                return true;
            }
            catch (Exception ex)
            {
                Guardar(ex.Message, "logs", true);
                throw new FileManagerException($"Error al serializar.");
            }
        }
        #endregion
    }
}
