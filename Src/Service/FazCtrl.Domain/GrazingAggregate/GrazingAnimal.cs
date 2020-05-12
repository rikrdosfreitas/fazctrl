using System;
using System.Collections.Generic;
using FazCtrl.Contract.Interfaces;
using FazCtrl.Domain.Shared;

namespace FazCtrl.Domain.GrazingAggregate
{
    public class GrazingAnimal : ValueObject, IAggregate
    {
        public GrazingAnimal(Guid animalId, int deal)
        {
            AnimalId = animalId;
            Deal = deal;
        }

        public Guid AnimalId { get; private set; }

        public int Deal { get; private set; }

        public static GrazingAnimal Create(Guid animalId, int deal)
        {
            return new GrazingAnimal(animalId, deal);
        }

        public void AddBalance(int deal)
        {
            Deal += deal;
        }

        public void RemoveBalance(int deal)
        {
            Deal -= deal;
        }

        protected override IEnumerable<object> GetEqualityValues()
        {
            yield return AnimalId;
        }

    }
}
