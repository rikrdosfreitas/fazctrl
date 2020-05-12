using System;
using System.Collections.Generic;
using System.Linq;
using FazCtrl.Domain.GrazingAggregate;
using FluentAssertions;
using NUnit.Framework;

namespace Domain.UnitTests.Grazing
{
    public class GrazingTests
    {
        [Test]
        public void WhenTheGrazingIsCreatedTheBalanceIsZero()
        {
            FazCtrl.Domain.GrazingAggregate.Grazing entity = FazCtrl.Domain.GrazingAggregate.Grazing.Create(id: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB4"), name: "Grazing 356", hectares: 158.4m);

            entity.Should().NotBeNull();
            entity.Balance.Should().Be(0);
        }

        [Test]
        public void WhenAddingAnyAnimalInTheGrazingItMustBeAddedToTheBalance()
        {
            FazCtrl.Domain.GrazingAggregate.Grazing entity = FazCtrl.Domain.GrazingAggregate.Grazing.Create(id: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB4"), name: "Grazing 356", hectares: 158.4m);

            entity.Should().NotBeNull();

            entity.AddAnimals(new List<GrazingAnimal> { GrazingAnimal.Create(animalId: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB3"), deal: 10) }
            );

            entity.Balance.Should().Be(10);
            entity.Animals.First().Deal.Should().Be(10);
        }

        [Test]
        public void WhenRemoveAnyAnimalInTheGrazingItMustBeRemovedToTheBalance()
        {
            FazCtrl.Domain.GrazingAggregate.Grazing entity = FazCtrl.Domain.GrazingAggregate.Grazing.Create(id: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB4"), name: "Grazing 356", hectares: 158.4m);

            entity.AddAnimals(new List<GrazingAnimal> { GrazingAnimal.Create(animalId: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB3"), deal: 10) });


            entity.RemoveAnimals(new List<GrazingAnimal> { GrazingAnimal.Create(animalId: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB3"), deal: 5) });

            entity.Balance.Should().Be(5);
            entity.Animals.First().Deal.Should().Be(5);
        }

        [Test]
        public void WhenRemoveAnyAnimalAndAnimalNotInGrazingMustBeThrow()
        {
            FazCtrl.Domain.GrazingAggregate.Grazing entity = FazCtrl.Domain.GrazingAggregate.Grazing.Create(id: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB4"), name: "Grazing 356", hectares: 158.4m);

            FluentActions.Invoking(() =>
                entity.RemoveAnimals(new List<GrazingAnimal>
                    {
                        GrazingAnimal.Create(animalId: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB3"), deal: 5)
                    }
                )).Should().Throw<ApplicationException>().WithMessage("Animal C9F15999-5386-40C4-BCA6-FD920AC6BDB3 not found!");
        }
    }
}