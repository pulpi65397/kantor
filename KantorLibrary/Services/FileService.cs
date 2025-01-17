using KantorLibrary.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using KantorLibrary.Exceptions;
using KantorLibrary.Interfaces;

namespace KantorLibrary.Services
{
    public class FileService<T>
    {
        private readonly IFileManager _fileManager;

        public FileService(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public async Task<List<T>> LoadDataAsync()
        {
            try
            {
                return await Task.Run(() => _fileManager.ReadFromFile<List<T>>());
            }
            catch (FileException ex)
            {
                // Obsługa błędu
                throw new ServiceException("Błąd podczas odczytu danych.", ex);
            }
        }

        public async Task SaveDataAsync(List<T> data)
        {
            try
            {
                await Task.Run(() => _fileManager.WriteToFile(data));
            }
            catch (FileException ex)
            {
                // Obsługa błędu
                throw new ServiceException("Błąd podczas zapisu danych.", ex);
            }
        }
    }

    public class ServiceException : Exception
    {
        public ServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

