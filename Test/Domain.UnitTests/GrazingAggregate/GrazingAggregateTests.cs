using System;
using System.Collections.Generic;
using System.Linq;
using FazCtrl.Domain.GrazingAggregate;
using FluentAssertions;
using NUnit.Framework;

namespace Domain.UnitTests.GrazingAggregate
{
    public class GrazingAggregateTests
    {
        [Test]
        public void WhenTheGrazingIsCreatedTheBalanceIsZero()
        {
            var entity = Grazing.Create(id: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB4"), name: "Grazing 356", hectares: 158.4m);

            entity.Should().NotBeNull();
            entity.Events.Should().HaveCount(1);
            entity.Version.Should().Be(1);
            entity.Balance.Should().Be(0);
        }

        [Test]
        public void WhenAddingAnyAnimalInTheGrazingItMustBeAddedToTheBalance()
        {
            Grazing entity = Grazing.Create(id: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB4"), name: "Grazing 356", hectares: 158.4m);

            entity.Should().NotBeNull();
            entity.Events.Should().HaveCount(1);
            entity.Version.Should().Be(1);

            entity.AddAnimals(new List<GrazingAnimal> { GrazingAnimal.Create(animalId: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB3"), deal: 10) }
            );

            entity.Events.Should().HaveCount(2);
            entity.Version.Should().Be(2);
            entity.Balance.Should().Be(10);
            entity.Animals.First().Deal.Should().Be(10);
        }

        [Test]
        public void WhenRemoveAnyAnimalInTheGrazingItMustBeRemovedToTheBalance()
        {
            Grazing entity = Grazing.Create(id: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB4"), name: "Grazing 356", hectares: 158.4m);

            entity.AddAnimals(new List<GrazingAnimal> { GrazingAnimal.Create(animalId: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB3"), deal: 10) });
            entity.RemoveAnimals(new List<GrazingAnimal> { GrazingAnimal.Create(animalId: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB3"), deal: 5) });

            entity.Events.Should().HaveCount(3);
            entity.Version.Should().Be(3);

            entity.Balance.Should().Be(5);
            entity.Animals.First().Deal.Should().Be(5);
        }

        [Test]
        public void WhenRemoveAnyAnimalAndAnimalNotInGrazingMustBeThrow()
        {
            Grazing entity = Grazing.Create(id: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB4"), name: "Grazing 356", hectares: 158.4m);

            FluentActions.Invoking(() =>
                entity.RemoveAnimals(new List<GrazingAnimal>
                    {
                        GrazingAnimal.Create(animalId: Guid.Parse("C9F15999-5386-40C4-BCA6-FD920AC6BDB3"), deal: 5)
                    }
                )).Should().Throw<ApplicationException>().WithMessage("Animal C9F15999-5386-40C4-BCA6-FD920AC6BDB3 not found!");
        }
    }
}