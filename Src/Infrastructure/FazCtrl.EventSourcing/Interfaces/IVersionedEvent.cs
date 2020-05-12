using System;
using FazCtrl.Contract;

namespace FazCtrl.EventSourcing.Interfaces
{
    public interface IVersionedEvent : IIntegrationEvent
    {
        int Version { get; }
    }
}
