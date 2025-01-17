using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KantorLibrary.Exceptions
{
    public class InvalidEntityStateException : Exception
    {
        public string EntityName { get; }
        public string InvalidState { get; }

        public InvalidEntityStateException(string entityName, string invalidState)
            : base($"The entity '{entityName}' is in an invalid state: {invalidState}.")
        {
            EntityName = entityName;
            InvalidState = invalidState;
        }

        public InvalidEntityStateException(string entityName, string invalidState, string message)
            : base(message)
        {
            EntityName = entityName;
            InvalidState = invalidState;
        }

        public InvalidEntityStateException(string entityName, string invalidState, string message, Exception innerException)
            : base(message, innerException)
        {
            EntityName = entityName;
            InvalidState = invalidState;
        }
    }
}
