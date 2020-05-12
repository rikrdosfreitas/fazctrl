using System;
using System.Collections.Generic;

namespace FazCtrl.EventSourcing.Interfaces
{
    public interface IEventSourced
    {
        Guid Id { get; }

        int Version { get; }

        IEnumerable<IVersionedEvent> Events { get; }
    }
}
