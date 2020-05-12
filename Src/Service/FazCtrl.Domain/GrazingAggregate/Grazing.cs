using System;
using System.Collections.Generic;
using System.Linq;
using FazCtrl.Contract.Interfaces;
using FazCtrl.Domain.GrazingAggregate.Events;
using FazCtrl.Domain.Shared;

namespace FazCtrl.Domain.GrazingAggregate
{
    public class Grazing : AggregateRoot
    {
        private readonly List<GrazingAnimal> _animals = new List<GrazingAnimal>();

        private Grazing(Guid id) : base(id)
        {
            Register<GrazingCreated>(When);
            Register<GrazingAnimalAdded>(When);
            Register<GrazingAnimalRemoved>(When);
        }

        private Grazing(Guid id, string name, decimal hectares) : this(id)
        {
            Name = name;
            Hectares = hectares;
            Balance = 0;

            Raise(new GrazingCreated(Id, Name, Hectares, Balance, Version));
        }

        public Grazing(Guid id, IEnumerable<IDomainEvent> history) : this(id)
        {
            LoadFrom(history);
        }

        public string Name { get; private set; }

        public decimal Hectares { get; private set; }

        public int Balance { get; private set; }

        public IReadOnlyCollection<GrazingAnimal> Animals => _animals.AsReadOnly();

        #region Business
        public static Grazing Create(Guid id, string name, decimal hectares)
        {
            return new Grazing(id, name, hectares);
        }

        public void AddAnimals(IList<GrazingAnimal> grazingAnimals)
        {
            Raise(new GrazingAnimalAdded(Id, Version, grazingAnimals));
        }

        public void RemoveAnimals(List<GrazingAnimal> grazingAnimals)
        {
            Raise(new GrazingAnimalRemoved(Id, Version, grazingAnimals));
        }

        #endregion

        #region Handler
        public void When(GrazingCreated @event)
        {
            Id = @event.Id;
            Name = @event.Name;
            Hectares = @event.Hectares;
            Balance = 0;
        }

        public void When(GrazingAnimalAdded @event)
        {
            foreach (var item in @event.GrazingAnimals)
            {
                var animal = _animals.FirstOrDefault(x => x == item);
                if (animal == null)
                {
                    _animals.Add(item);
                }
                else
                {
                    animal.AddBalance(item.Deal);
                }

                Balance += item.Deal;
            }
        }

        private void When(GrazingAnimalRemoved @event)
        {
            foreach (var item in @event.GrazingAnimals)
            {
                var animal = _animals.FirstOrDefault(x => x == item);
                if (animal == null)
                {
                    throw new ApplicationException($"Animal {item.AnimalId} not found!");
                }

                animal.RemoveBalance(item.Deal);
                Balance -= item.Deal;
            }
        }

        #endregion
    }
}