using System;
using FazCtrl.EventSourcing.Interfaces;

namespace FazCtrl.EventSourcing
{

    public abstract class VersionedEvent : IVersionedEvent
    {
        public Guid SourceId { get; set; }
        public int Version { get; set; }
    }
}
