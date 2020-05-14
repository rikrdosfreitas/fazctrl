using FazCtrl.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;
using FazCtrl.Infrastructure;

namespace FazCtrl.Application.Command.Entity
{
    public class CreateEntityCommand : ICommand
    {
        public CreateEntityCommand(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }
    }

    public class CreateEntityCommandHandle : ICommandHandler<CreateEntityCommand>
    {
        private readonly IEventStoreRepository<Domain.EntityAggregate.Entity> _storeRepository;

        public CreateEntityCommandHandle(IEventStoreRepository<Domain.EntityAggregate.Entity> storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<bool> Handle(CreateEntityCommand request, CancellationToken cancellationToken)
        {
            var entity = await _storeRepository.FindAsync(request.Id);
            if (entity != null)
                throw new ApplicationException("Entity already exists!");

            entity = Domain.EntityAggregate.Entity.Create(request.Id, request.Name);

            await _storeRepository.SaveAsync(entity);

            return true;
        }
    }

}
