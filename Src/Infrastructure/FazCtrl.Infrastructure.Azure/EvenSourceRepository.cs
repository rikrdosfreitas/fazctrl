using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FazCtrl.Contract.Interfaces;

namespace FazCtrl.Infrastructure.Azure
{
    public class EvenSourceRepository<T> : IEventStoreRepository<T> where T : IAggregateRoot
    {
        private readonly IRepository _repository;

        public EvenSourceRepository(IRepository repository)
        {
            _repository = repository;
        }


        public async Task<T> FindAsync(Guid id)
        {
            try
            {
                var list = (await _repository.GetAsync(id, typeof(T).Name)).ToList();

                if (!list.Any()) return default;

                List<IDomainEvent> events = new List<IDomainEvent>();

                list.ForEach(x =>
                {
                    var data = (EventData) x;

                    events.Add(data.Event);

                });

                var result = Compile(id, events);

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private T Compile(Guid id, IEnumerable<IDomainEvent> list)
        {
            var constructor = typeof(T).GetConstructor(new Type[] { typeof(Guid), typeof(IEnumerable<IDomainEvent>) });
            var parameterId = Expression.Parameter(typeof(Guid), "p");
            var parameterEvents = Expression.Parameter(typeof(IEnumerable<IDomainEvent>), "p2");
            var creatorExpression = Expression.Lambda<Func<Guid, IEnumerable<IDomainEvent>, T>>(
                Expression.New(constructor, new Expression[] { parameterId, parameterEvents }), parameterId, parameterEvents);

            var compile = creatorExpression.Compile();

            return compile(id, list);
        }


        public async Task SaveAsync(T eventSourced)
        {
            var events = eventSourced.Events.ToArray();

            await _repository.SaveAsync(eventSourced.Id, typeof(T).Name, events);
        }
    }
}
