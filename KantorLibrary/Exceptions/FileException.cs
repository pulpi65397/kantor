using System;

namespace KantorLibrary.Exceptions
{
    // Klasa wyjątków dla operacji na plikach
    public class FileException : Exception
    {
        // Konstruktor bez parametrów
        public FileException()
        {
        }

        // Konstruktor z wiadomością błędu
        public FileException(string message)
            : base(message)
        {
        }

        // Konstruktor z wiadomością błędu i wewnętrznym wyjątkiem
        public FileException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
