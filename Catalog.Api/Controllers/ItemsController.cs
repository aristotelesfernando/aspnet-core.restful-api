using System.Linq;
using System;
using System.Collections.Generic;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Catalog.Api.Dtos;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IInMemItemsRepository repository;
        private readonly ILogger<ItemsController> logger;

        public ItemsController(IInMemItemsRepository _repository, ILogger<ItemsController> logger)
        {
            this.repository = _repository;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetItemsAsync(string name = null)
        {
            var items = (await repository.GetItemsAsync()).Select(item => item.AsDto());

            if (!string.IsNullOrWhiteSpace(name))
            {
                items = items.Where(item => item.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            logger.LogInformation($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}: Retireve {items.Count()} items");

            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
        {
            var item = await repository.GetItemAsync(id);

            if (item is null)
            {
                return NotFound();
            }

            return item.AsDto();
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<ItemDto>> GetItemByName(string name)
        {
            var item = await repository.GetItemByNameAsync(name);

            if (item is null)
            {
                return NotFound();
            }

            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto)
        {
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Description = itemDto.Description,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await repository.CreateItemAsync(item);

            return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto itemDto)
        {
            var existingItem = await repository.GetItemAsync(id);
            if (existingItem is null)
            {
                return NotFound();
            }

            existingItem.Name = itemDto.Name;
            existingItem.Price = itemDto.Price;

            await repository.UpdateItemAsync(existingItem);
            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItemAsync(Guid id)
        {
            var existingItem = await repository.GetItemAsync(id);
            if (existingItem is null)
            {
                return NotFound();
            }

            await repository.DeleteItemAsync(id);
            return NoContent();

        }

    }
}