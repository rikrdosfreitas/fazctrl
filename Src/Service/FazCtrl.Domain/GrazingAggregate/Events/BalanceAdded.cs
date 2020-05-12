using System;
using FazCtrl.Domain.Shared;

namespace FazCtrl.Domain.GrazingAggregate.Events
{
    public class BalanceAdded : DomainEvent
    {
        public BalanceAdded(Guid id, int version, Guid animalAnimalId, int animalDeal) : base(id, version)
        {
            Id = id;
            AnimalAnimalId = animalAnimalId;
            AnimalDeal = animalDeal;
        }

        public Guid Id { get; private set; }

        public Guid AnimalAnimalId { get; private set; }

        public int AnimalDeal { get; private set; }
    }
}