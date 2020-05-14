using System;
using System.Collections.Generic;
using FazCtrl.Contract.Interfaces;

namespace FazCtrl.Domain.Shared
{
    public abstract class AggregateRoot : IAggregateRoot
    {
        private readonly Dictionary<Type, Action<IDomainEvent>> _handlers = new Dictionary<Type, Action<IDomainEvent>>();
        private readonly List<IDomainEvent> _events = new List<IDomainEvent>();

        protected AggregateRoot(Guid id)
        {
            Id = id;
            Version = 0;
        }


        AggregateRoot(Guid id, IEnumerable<IDomainEvent> history)
        {
            throw new NotImplementedException();
        }

        public Guid Id { get; protected set; }

        public int Version { get; protected set; }

        public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

        public void LoadFrom(IEnumerable<IDomainEvent> pastEvents)
        {
            foreach (var e in pastEvents)
            {
                _handlers[e.GetType()].Invoke(e);
                Version = e.Version;
            }
        }

        public void Register<T>(Action<T> when) where T : IDomainEvent
        {
            _handlers.Add(typeof(T), e => when((T)e));
        }

        public void ClearDomainEvents()
        {
            _events.Clear();
        }

        protected void Raise(IDomainEvent domainEvent)
        {
            domainEvent.Version++;
            _events.Add(domainEvent);
            _handlers[domainEvent.GetType()](domainEvent);
            Version = domainEvent.Version;
        }

    }
}