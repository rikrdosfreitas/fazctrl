using System;
using System.Threading.Tasks;
using FazCtrl.Application.Command.Entity;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.Command.Entity
{
    using static Testing;

    [TestFixture()]
    public class CreateEntityCommandHandleTests
    {
        [Test()]
        public async Task CreateEntityCommandHandleTest()
        {
            Guid id = Guid.NewGuid();

            var command = new CreateEntityCommand(id, "Test");

            await SendAsync(command);

            var item = await FindAsync<FazCtrl.Domain.EntityAggregate.Entity>(id);

            item.Should().NotBeNull();
            item.Id.Should().Be(id);
            item.Name.Should().Be("Test");
        }
    }
}