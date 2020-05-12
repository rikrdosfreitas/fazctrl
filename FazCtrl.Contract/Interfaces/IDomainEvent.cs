using System;

namespace FazCtrl.Contract.Interfaces
{
    public interface IDomainEvent
    {
        Guid SourceId { get; }

        int Version { get; set; }
    }
}