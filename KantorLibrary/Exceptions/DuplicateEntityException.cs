using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KantorLibrary.Exceptions
{
    public class DuplicateEntityException : Exception
    {
        public Guid EntityId { get; }

        public DuplicateEntityException(Guid entityId)
            : base($"Entity with ID {entityId} already exists.")
        {
            EntityId = entityId;
        }

        public DuplicateEntityException(Guid entityId, string message)
            : base(message)
        {
            EntityId = entityId;
        }

        public DuplicateEntityException(Guid entityId, string message, Exception innerException)
            : base(message, innerException)
        {
            EntityId = entityId;
        }
    }
}
