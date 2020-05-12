using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FazCtrl.Contract.Interfaces;

namespace FazCtrl.Infrastructure
{
    public interface IRepository
    {
        Task<IEnumerable<IEventData>> GetAsync(Guid id, string type);

        Task SaveAsync(Guid id, string type, IEnumerable<IDomainEvent> events);
    }
}
