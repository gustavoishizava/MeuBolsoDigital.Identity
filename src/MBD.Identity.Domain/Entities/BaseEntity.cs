using System;

namespace MBD.Identity.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; private init; }
        public DateTime CreatedAt { get; private init; }
        public DateTime? UpdatedAt { get; private set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }
    }
}