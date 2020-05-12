using System.Threading;
using System.Threading.Tasks;
using FazCtrl.Infrastructure.Sql.SqlEventStore.Utilities;
using MediatR;

namespace FazCtrl.Application.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {

        //private readonly IIntegrationEventService _integrationEventService;
        private readonly IDbContextManager _contextManager;
        //public readonly IssuanceDbContext Context;

        public TransactionBehavior(IDbContextManager contextManager)
        {
            _contextManager = contextManager;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (_contextManager.IsInTransaction)
            {
                return await next();
            }

            TResponse response = default;

            //await _contextManager.ExecuteInTransactionAsync(Context, async (token) =>
            //{
            //    response = await next();
            //}, cancellationToken);

            return response;
        }
    }
}
