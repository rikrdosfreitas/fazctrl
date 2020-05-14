using System;
using FazCtrl.Domain.Shared;
using Newtonsoft.Json;

namespace FazCtrl.Domain.GrazingAggregate.Events
{
    public class GrazingCreated : DomainEvent
    {
        [JsonConstructor]
        private GrazingCreated() { }

        public GrazingCreated(Guid id, string name, decimal hectares, int balance, int version) : base(id, version)
        {
            Id = id;
            Name = name;
            Hectares = hectares;
            Balance = balance;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public decimal Hectares { get; private set; }

        public int Balance { get; private set; }
    }
}