using FazCtrl.Contract.Interfaces;
using FazCtrl.Domain.Shared;
using MediatR;

namespace FazCtrl.Domain
{
    public interface ICommand : IRequest<bool>
    {
    }

    public interface ICommand<out T> : IRequest<T> where T : IEntity
    {
    }

    public interface ICommandHandler<in TRequest> : IRequestHandler<TRequest, bool> where TRequest : ICommand
    {
    }
}
