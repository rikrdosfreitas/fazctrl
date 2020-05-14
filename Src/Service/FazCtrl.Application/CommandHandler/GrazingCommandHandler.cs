using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FazCtrl.Application.Command;
using FazCtrl.Application.Command.Grazing;
using FazCtrl.Domain;
using FazCtrl.Domain.GrazingAggregate;
using FazCtrl.Infrastructure;

namespace FazCtrl.Application.CommandHandler
{
    public class GrazingCommandHandler 
        : ICommandHandler<CreateGrazingCommand>,
        ICommandHandler<AddAnimalInGrazingCommand>
    {
        private readonly IEventStoreRepository<Grazing> _storeRepository;

        public GrazingCommandHandler(IEventStoreRepository<Grazing> storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<bool> Handle(CreateGrazingCommand request, CancellationToken cancellationToken)
        {
            var entity = await _storeRepository.FindAsync(request.Id);
            if (entity != null)
                throw new ApplicationException("Entity already exists!");

            entity = Grazing.Create(request.Id, request.Name, request.Hectares);

            await _storeRepository.SaveAsync(entity);

            return true;
        }

        public async Task<bool> Handle(AddAnimalInGrazingCommand request, CancellationToken cancellationToken)
        {
            var entity = await _storeRepository.FindAsync(request.Id);
            if (entity == null)
                throw new ApplicationException("Entity already exists!");

            entity.AddAnimals(request.Animals.Select(x=> new GrazingAnimal(x.AnimalId, x.Deal)).ToList());

            await _storeRepository.SaveAsync(entity);

            return true;
        }
    }

    
}
