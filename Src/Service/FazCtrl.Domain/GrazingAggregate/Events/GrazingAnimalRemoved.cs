using System;
using System.Collections.Generic;
using FazCtrl.Domain.Shared;
using Newtonsoft.Json;

namespace FazCtrl.Domain.GrazingAggregate.Events
{
    public class GrazingAnimalRemoved : DomainEvent
    {
        [JsonConstructor]
        private GrazingAnimalRemoved()
        {

        }

        public GrazingAnimalRemoved(Guid sourceId, int version, IEnumerable<GrazingAnimal> grazingAnimals) : base(sourceId, version)
        {
            GrazingAnimals = grazingAnimals;
        }

        public IEnumerable<GrazingAnimal> GrazingAnimals { get; set; }

       
    }
}