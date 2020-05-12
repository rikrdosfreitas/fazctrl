using System;
using System.Collections.Generic;
using FazCtrl.Contract.Interfaces;
using FazCtrl.Domain.GrazingAggregate;
using FazCtrl.Domain.GrazingAggregate.Events;
using FazCtrl.Domain.Shared;

namespace FazCtrl.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var id = Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB4");
            var created = new GrazingCreated(id, "Teste", 10.45m, 0, 0);
            var added = new GrazingAnimalAdded(id, 1, new List<GrazingAnimal>() { new GrazingAnimal(id, 20) });
            var removed = new GrazingAnimalRemoved(id, 2, new List<GrazingAnimal>() { new GrazingAnimal(id, 5) });

            IEnumerable<IDomainEvent> list = new List<IDomainEvent>() { created, added, removed };

            var gr = new Grazing(id, list);
        }
    }
}
