using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KantorLibrary.Utils
{
    public delegate void FileOperationCompletedHandler(string filePath, bool success);

    public class FileOperations
    {
        // Event wywoływany po zakończeniu operacji na pliku
        public event FileOperationCompletedHandler FileOperationCompleted;

        public void OnFileOperationCompleted(string filePath, bool success)
        {
            // Wywołanie eventu, jeśli został zarejestrowany
            FileOperationCompleted?.Invoke(filePath, success);
        }

        // Przykład metody, która będzie wywoływać event
        public void WriteToFile(string filePath, string data)
        {
            try
            {
                File.WriteAllText(filePath, data);
                OnFileOperationCompleted(filePath, true); // Powiadomienie o sukcesie
            }
            catch (Exception)
            {
                OnFileOperationCompleted(filePath, false); // Powiadomienie o błędzie
            }
        }
    }
}

