using FazCtrl.Contract;
using FazCtrl.EventSourcing.Interfaces;
using System;
using System.Collections.Generic;

namespace FazCtrl.EventSourcing
{
    public abstract class EventSourced : IEventSourced
    {
        private readonly Dictionary<Type, Action<IVersionedEvent>> _handlers = new Dictionary<Type, Action<IVersionedEvent>>();
        private readonly List<IVersionedEvent> _pendingEvents = new List<IVersionedEvent>();
        private readonly Guid _id;
        private int _version = -1;

        protected EventSourced(Guid id)
        {
            _id = id;
        }
        
        public Guid Id => _id;

        public int Version => _version;
        
        public IEnumerable<IVersionedEvent> Events => _pendingEvents;

        protected void Handles<TEvent>(Action<TEvent> handler) where TEvent : IIntegrationEvent
        {
            _handlers.Add(typeof(TEvent), @event => handler((TEvent)@event));

        }
        protected void LoadFrom(IEnumerable<IVersionedEvent> pastEvents)
        {
            foreach (var e in pastEvents)
            {
                _handlers[e.GetType()].Invoke(e);
                _version = e.Version;
            }
        }

        protected void Update(VersionedEvent e)
        {
            e.SourceId = Id;
            e.Version = _version + 1;
            _handlers[e.GetType()].Invoke(e);
            _version = e.Version;
            _pendingEvents.Add(e);
        }
    }
}
