using System;
using FazCtrl.Contract;
using MediatR;

namespace FazCtrl.EventSourcing.Entities.Domain
{
    public class InventoryStockAdded : VersionedEvent, INotification
    {
        private InventoryStockAdded()
        {
            
        }

        public InventoryStockAdded(int quantity, Guid animalId, Guid inventoryId)
        {
            Quantity = quantity;
            AnimalId = animalId;
            InventoryId = inventoryId;
        }

        public int Quantity { get; set; }

        public Guid AnimalId { get; set; }

        public Guid InventoryId { get; set; }
    }
}
