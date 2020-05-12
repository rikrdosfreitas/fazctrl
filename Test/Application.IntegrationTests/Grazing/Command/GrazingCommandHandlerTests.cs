using FazCtrl.Application.Command;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;

namespace Application.IntegrationTests.Grazing.Command
{
    using static Testing;

    [TestFixture()]
    public class GrazingCommandHandlerTests
    {
        [Test()]
        public async Task CreateGrazingCommandHandleTest()
        {
            Guid id = Guid.NewGuid();

            var command = new CreateGrazingCommand(id, "Test", 10.9m);

            await SendAsync(command);

            var item = await FindAsync<FazCtrl.Domain.GrazingAggregate.Grazing>(id);

            item.Should().NotBeNull();
            item.Name.Should().Be("Test");
            item.Id.Should().Be(id);
            item.Hectares.Should().Be(10.9m);
            item.Balance.Should().Be(0);
        }

        [Test()]
        public async Task AddAnimalInGrazingCommandHandleTest()
        {
            Guid id = Guid.NewGuid();

            var grazingId = new CreateGrazingCommand(id, "Test", 10.9m);

            await SendAsync(grazingId);


            var command = new AddAnimalInGrazingCommand(id, new List<GrazingAnimalViewModel> { new GrazingAnimalViewModel(id, 20) });

            await SendAsync(command);

            var item = await FindAsync<FazCtrl.Domain.GrazingAggregate.Grazing>(id);

            item.Should().NotBeNull();
            item.Id.Should().Be(id);
            item.Balance.Should().Be(20);
            item.Animals.First().Deal.Should().Be(20);
          
        }
    }
}