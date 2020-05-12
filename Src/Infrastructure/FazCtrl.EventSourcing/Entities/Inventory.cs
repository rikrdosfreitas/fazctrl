using System;
using System.Collections.Generic;
using FazCtrl.EventSourcing.Entities.Domain;
using FazCtrl.EventSourcing.Interfaces;

namespace FazCtrl.EventSourcing.Entities
{
    public class Inventory : EventSourced
    {
        protected Inventory(Guid id) : base(id)
        {
            base.Handles<InventoryStockAdded>(this.OnInventoryStockAdded);
            base.Handles<InventoryStockUpdated>(this.OnInventoryStockUpdated);
        }

        public Inventory(Guid id, Guid animalId,int qtdy) : base(id)
        {
            this.Update(new InventoryStockAdded(qtdy, animalId, id));
        }

        public Inventory(Guid id, IEnumerable<IVersionedEvent> history) 
            : this(id)
        {
            LoadFrom(history);
        }

        public void UpdateInventory(Guid id, int qtdy, Guid aminalId)
        {
            this.Update(new InventoryStockUpdated(qtdy, aminalId, id));
        }
        
        private void OnInventoryStockAdded(InventoryStockAdded obj)
        {
            //throw new NotImplementedException();
        }
        
        private void OnInventoryStockUpdated(InventoryStockUpdated obj)
        {

        }

    }

    public class InventoryStockUpdated : VersionedEvent
    {
        public InventoryStockUpdated(int qtdy, Guid aminalId, Guid id)
        {
            Qtdy = qtdy;
            AminalId = aminalId;
            Id = id;
        }

        public Guid Id { get; set; }

        public Guid AminalId { get; set; }

        public int Qtdy { get; set; }
    }
}
