using System;
using System.Collections.Generic;
using FazCtrl.Domain;

namespace FazCtrl.Application.Command.Grazing
{
    public class AddAnimalInGrazingCommand : ICommand
    {
        public AddAnimalInGrazingCommand(Guid id, IEnumerable<GrazingAnimalViewModel> animals)
        {
            Id = id;
            Animals = animals;
        }

        public Guid Id { get; private set; }

        public IEnumerable<GrazingAnimalViewModel> Animals { get; private set; }
    }
}