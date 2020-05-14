using System;

namespace FazCtrl.Application.Command
{
    public class GrazingAnimalViewModel
    {
        public GrazingAnimalViewModel(Guid animalId, int deal)
        {
            AnimalId = animalId;
            Deal = deal;
        }

        public Guid AnimalId { get; set; }
        public int Deal { get; set; }
    }
}