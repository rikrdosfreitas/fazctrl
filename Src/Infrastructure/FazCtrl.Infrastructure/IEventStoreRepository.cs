using System;
using System.Threading.Tasks;
using FazCtrl.Contract.Interfaces;

namespace FazCtrl.Infrastructure
{
    public interface IEventStoreRepository<T> where T : IAggregateRoot
    {
        Task<T> FindAsync(Guid requestId);

        Task SaveAsync(T eventSourced);
    }
}
