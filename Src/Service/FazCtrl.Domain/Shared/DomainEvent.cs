using System;
using FazCtrl.Contract.Interfaces;
using Newtonsoft.Json;

namespace FazCtrl.Domain.Shared
{
    public abstract class DomainEvent : IDomainEvent
    {
        [JsonConstructor]
        protected DomainEvent()
        {
            
        }

        protected DomainEvent(Guid sourceId, int version)
        {
            SourceId = sourceId;
            Version = version;
        }

        public Guid SourceId { get; private set; }

        public int Version { get; set; }
    }
}