using System;
using FazCtrl.Domain.Shared;

namespace FazCtrl.Domain.EntityAggregate.Events
{
    public class EntityCreated : DomainEvent
    {
        public EntityCreated(Guid id, string name, int version) : base(id, version)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }
    }
}