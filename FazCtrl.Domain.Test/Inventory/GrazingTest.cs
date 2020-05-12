using System;
using System.Collections.Generic;
using System.Linq;
using FazCtrl.Domain.GrazingAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FazCtrl.Domain.Test.Inventory
{
    [TestClass]
    public class GrazingTest
    {
        [TestMethod]
        public void WhenTheGrazingIsCreatedTheBalanceIsZero()
        {
            Grazing entity = Grazing.Create(id: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB4"), name: "Grazing 356", hectares: 158.4m);

            Assert.IsNotNull(entity);
            Assert.AreEqual(0, entity.Balance);
        }

        [TestMethod]
        public void WhenAddingAnyAnimalInTheGrazingItMustBeAddedToTheBalance()
        {
            Grazing entity = Grazing.Create(id: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB4"), name: "Grazing 356", hectares: 158.4m);

            entity.AddAnimals(new List<GrazingAnimal> { GrazingAnimal.Create(animalId: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB3"), deal: 10) }
            );

            Assert.AreEqual(10, entity.Balance);
            Assert.AreEqual(10, entity.Animals.First().Deal);
        }

        [TestMethod]
        public void WhenRemoveAnyAnimalInTheGrazingItMustBeRemovedToTheBalance()
        {
            Grazing entity = Grazing.Create(id: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB4"), name: "Grazing 356", hectares: 158.4m);
            
            entity.AddAnimals(new List<GrazingAnimal> { GrazingAnimal.Create(animalId: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB3"), deal: 10) });


            entity.RemoveAnimals(new List<GrazingAnimal> { GrazingAnimal.Create(animalId: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB3"), deal: 5) });
            
            Assert.AreEqual(5, entity.Balance);
            Assert.AreEqual(5, entity.Animals.First().Deal);
        }

        [TestMethod]
        public void WhenRemoveAnyAnimalAndAnimalNotInGrazingMustBeThrow()
        {
            Grazing entity = Grazing.Create(id: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB4"), name: "Grazing 356", hectares: 158.4m);

            Assert.ThrowsException<ApplicationException>(() =>
                entity.RemoveAnimals(new List<GrazingAnimal>
                { GrazingAnimal.Create(animalId: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB3"), deal: 5) }
            ), "Animal C9F15999-5386-40C4-BCA6-FD920AC6BDB3 not found!");
        }
    }
}
