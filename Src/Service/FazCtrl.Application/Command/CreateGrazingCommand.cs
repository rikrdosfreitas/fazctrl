using System;
using FazCtrl.Domain;

namespace FazCtrl.Application.Command
{
    public class CreateGrazingCommand : ICommand
    {
        public CreateGrazingCommand(Guid id, string name, decimal hectares)
        {
            Id = id;
            Name = name;
            Hectares = hectares;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public decimal Hectares { get; private set; }
    }
}