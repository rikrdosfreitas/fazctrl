using System;

namespace FazCtrl.Contract
{
    public interface IIntegrationEvent
    {
        Guid SourceId { get; }
    }
}
