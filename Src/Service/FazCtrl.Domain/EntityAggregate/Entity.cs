using System;
using System.Collections.Generic;
using FazCtrl.Contract.Interfaces;
using FazCtrl.Domain.EntityAggregate.Events;
using FazCtrl.Domain.Shared;

namespace FazCtrl.Domain.EntityAggregate
{
    public class Entity : AggregateRoot
    {
        private Entity(Guid id) : base(id)
        {
            Register<EntityCreated>(When);
            Register<EntityUpdated>(When);
        }

        private Entity(Guid id, string name) : this(id)
        {
            Raise(new EntityCreated(id, name, 0));
        }

        public Entity(Guid id, IEnumerable<IDomainEvent> history) : this(id)
        {
            LoadFrom(history);
        }

        public string Name { get; private set; }

        public static Entity Create(Guid id, string name)
        {
            return new Entity(id, name);
        }

        public void Update(string name)
        {
            Raise(new EntityUpdated(Id, name, Version));
        }

        private void When(EntityCreated @event)
        {
            Id = @event.Id;
            Name = @event.Name;
        }

        private void When(EntityUpdated @event)
        {
            Name = @event.Name;
        }

    }
}
