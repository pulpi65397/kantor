using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KantorLibrary.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public Guid EntityId { get; }

        public EntityNotFoundException(Guid entityId)
            : base($"Entity with ID {entityId} was not found.")
        {
            EntityId = entityId;
        }

        public EntityNotFoundException(Guid entityId, string message)
            : base(message)
        {
            EntityId = entityId;
        }

        public EntityNotFoundException(Guid entityId, string message, Exception innerException)
            : base(message, innerException)
        {
            EntityId = entityId;
        }

    }
}
