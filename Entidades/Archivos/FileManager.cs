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
                    throw new FileManagerException("Error al crear el directorio");
                }
            }
        }

        public static void Guardar(string data, string nombreArchivo, bool append)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(path, $"{nombreArchivo}.txt"), append))
            {
                sw.Write(data);
            }
        }

        public static bool Serializar<T>(T elemento, string nombreArchivo)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.WriteIndented = true;

                using (StreamWriter sw = new StreamWriter(Path.Combine(path, $"{nombreArchivo}.json")))
                {
                    string datosJson = JsonSerializer.Serialize(elemento, options);
                    sw.Write(datosJson);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new FileManagerException($"Error al serializar: {ex.Message}");
            }
        }
    }
}
