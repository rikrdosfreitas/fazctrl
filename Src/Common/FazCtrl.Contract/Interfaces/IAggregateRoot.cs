using System.Collections.Generic;

namespace FazCtrl.Contract.Interfaces
{
    public interface IAggregateRoot : IEntity
    {
        int Version { get; }

        IReadOnlyCollection<IDomainEvent> Events { get; }

        void ClearDomainEvents();

        void LoadFrom(IEnumerable<IDomainEvent> pastEvents);

        void Register<T>(System.Action<T> when) where T : IDomainEvent;
    }
}