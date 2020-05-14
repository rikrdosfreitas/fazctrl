using System;
using FazCtrl.Domain.Shared;

namespace FazCtrl.Domain.EntityAggregate.Events
{
    public class EntityUpdated : DomainEvent
    {
        public EntityUpdated(Guid id, string name, int version) : base(id, version)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}