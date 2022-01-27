using System;
using System.Threading.Tasks;
using Catalog.Api.Controllers;
using Catalog.Api.Dtos;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Catalog.UnitTests
{
    public class ItemsControllerTests
    {

        private readonly Mock<IInMemItemsRepository> repositoryStub = new();
        private readonly Mock<ILogger<ItemsController>> loggerStub = new();
        private readonly Random rand = new();

        [Fact]
        public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync((Item)null);
            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();

            //Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetItemAsync_WithExistingItem_ReturnsExpectedItem()
        {
            // Arrange
            var expectedItem = CreateRandomItem();
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync(expectedItem);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            // Assert
            result.Value.Should().BeEquivalentTo(
                expectedItem,
                options => options.ComparingByMembers<Item>()
            );

            // Substitui estas linhas por apenas a linha acima
            // Assert.IsType<ItemDto>(result.Value);
            // var dto = (result as ActionResult<ItemDto>).Value;
            // Assert.Equal(expectedItem.Id, dto.Id);
            // Assert.Equal(expectedItem.Name, dto.Name);

        }

        // [Fact]
        // public async Task GetItemByNameAsync_WithExistingItem_ReturnsExpectedItem()
        // {
        //     // Arrange
        //     var expectedItem = CreateRandomItem();

        //     var _expectedItem = new ItemDto()
        //     {
        //         Id = Guid.NewGuid(),
        //         Name = Guid.NewGuid().ToString(),
        //         Price = rand.Next(1000),
        //         CreatedDate = DateTimeOffset.UtcNow
        //     };

        //     //repositoryStub.Setup(repo => repo.GetItemByNameAsync(It.IsAny<Guid>().ToString())).ReturnsAsync(expectedItem);
        //     repositoryStub.Setup(r => r.GetItemByNameAsync(It.IsValueType<string>())).ReturnsAsync(_expectedItem);

        //     var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

        //     // Act
        //     var result = await controller.GetItemByName(Guid.NewGuid().ToString());

        //     // Assert
        //     result.Value.Should().BeEquivalentTo(
        //         expectedItem,
        //         options => options.ComparingByMembers<Item>()
        //     );

        //     // Substitui estas linhas por apenas a linha acima
        //     // Assert.IsType<ItemDto>(result.Value);
        //     // var dto = (result as ActionResult<ItemDto>).Value;
        //     // Assert.Equal(expectedItem.Id, dto.Id);
        //     // Assert.Equal(expectedItem.Name, dto.Name);

        // }

        [Fact]
        public async Task GetItemsAsync_WithExistingItems_ReturnsAllItems()
        {
            // Arrange
            var expectedItems = new[] { CreateRandomItem(), CreateRandomItem(), CreateRandomItem() };
            repositoryStub.Setup(repo => repo.GetItemsAsync()).ReturnsAsync(expectedItems);
            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Act
            var actualItems = await controller.GetItemsAsync();

            // Assert
            actualItems.Should().BeEquivalentTo(
                expectedItems,
                op => op.ComparingByMembers<Item>()
            );
        }

        [Fact]
        public async Task CreateItemAsync_WithItemToCreate_ReturnsCreatedItem()
        {
            // Arrange
            var itemToCreate = new CreateItemDto()
            {
                Name = Guid.NewGuid().ToString(),
                Price = rand.Next(1000)
            };

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Act
            var result = await controller.CreateItemAsync(itemToCreate);

            // Assert
            var createItem = (result.Result as CreatedAtActionResult).Value as ItemDto;
            itemToCreate.Should().BeEquivalentTo(
                createItem,
                op => op.ComparingByMembers<ItemDto>().ExcludingMissingMembers()
            );
            createItem.Id.Should().NotBeEmpty();
            TimeSpan span = TimeSpan.FromSeconds(1);
            createItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, span);
        }

        [Fact]
        public async Task UpdateItemAsync_WithExistingItem_ReturnsNoContent()
        {
            // Arrange
            var existingItem = CreateRandomItem();
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync(existingItem);

            var itemId = existingItem.Id;
            var itemToUpdate = new UpdateItemDto()
            {
                Name = Guid.NewGuid().ToString(),
                Price = existingItem.Price + 3
            };
            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Act
            var result = await controller.UpdateItemAsync(itemId, itemToUpdate);

            // Assert
            result.Should().BeOfType<NoContentResult>();

        }

        [Fact]
        public async Task UpdateItemAsync_WithNonExistingItem_ReturnsNotFound()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync((Item)null);
            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();

        }

        [Fact]
        public async Task DeleteItemAsync_WithExistingItem_ReturnsNoContent()
        {
            // Arrange
            var existingItem = CreateRandomItem();
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync(existingItem);

            var itemId = existingItem.Id;
            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Act
            var result = await controller.DeleteItemAsync(itemId);

            // Assert
            result.Should().BeOfType<NoContentResult>();

        }

        private Item CreateRandomItem()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Price = rand.Next(1000),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}
