using NUnit.Framework;
using System;
using System.Linq;
using FazCtrl.Domain.EntityAggregate;
using FazCtrl.Domain.EntityAggregate.Events;
using FluentAssertions;

namespace Domain.UnitTests.EntityAggregate
{
    public class EntityAggregateTest
    {
        [Test]
        public void WhenEntityCreated()
        {
            Guid id = Guid.NewGuid();

            var entity = Entity.Create(id, "Unit Test");

            entity.Should().NotBeNull();
            entity.Id.Should().Be(id);
            entity.Name.Should().Be("Unit Test");

            entity.Events.Should().HaveCount(1);
            entity.Events.Should().ContainItemsAssignableTo<EntityCreated>();

            entity.Version.Should().Be(1);
        }

        [Test]
        public void WhenEntityUpdated()
        {
            Guid id = Guid.NewGuid();

            var entity = Entity.Create(id, "Unit Test");

            entity.Update("Test");

            entity.Should().NotBeNull();
            entity.Id.Should().Be(id);
            entity.Name.Should().Be("Test");

            entity.Events.Last().Should().BeOfType<EntityUpdated>();

            entity.Version.Should().Be(2);
        }
    }
}
