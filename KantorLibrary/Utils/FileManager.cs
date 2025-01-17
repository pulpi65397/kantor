using KantorLibrary.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KantorLibrary.Utils
{
    public interface IFileManager
    {
        T ReadFromFile<T>();
        void WriteToFile<T>(T data);
        bool FileExists();
        void CreateFileIfNotExists();
    }
    public class FileManager : IFileManager
    {
        private readonly string _filePath;

        public FileManager(string filePath)
        {
            _filePath = filePath;
        }

        // Metoda do odczytu pliku JSON
        public T ReadFromFile<T>()
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"Plik '{_filePath}' nie został znaleziony.");
            }

            var jsonData = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<T>(jsonData);
        }

        // Metoda do zapisu danych do pliku JSON
        public void WriteToFile<T>(T data)
        {
            var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(_filePath, jsonData);
        }

        // Metoda do sprawdzenia czy plik istnieje
        public bool FileExists()
        {
            return File.Exists(_filePath);
        }

        // Metoda do tworzenia nowego pliku, jeśli nie istnieje
        public void CreateFileIfNotExists()
        {
            if (!FileExists())
            {
                File.Create(_filePath).Dispose(); // Utworzenie pustego pliku
            }
        }
    }
}
