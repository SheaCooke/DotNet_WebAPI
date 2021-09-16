using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Repositories;
using Catalog.Entities;
using Catalog.Dtos;

namespace Catalog.Controllers
{
    [ApiController] // brings in additional behaviors 
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository repository;

        public ItemsController(IItemsRepository repository)
        {
            this.repository = repository;
            //repository = new InMenuItemsRepository(); //needs to be replaced, explicit dependency. use dependency inversion
        }

        [HttpGet]
        public IEnumerable<ItemDto> GetItems()
        {
            var items = repository.GetItems().Select(item => item.AsDto());

            return items;
        }

        //GET/items/id
        [HttpGet("{Id}")] //The path parameter must match the controller parameter exactly!!!
        public ActionResult<ItemDto> GetItem(Guid Id) //ActionResult allows us to return different data types  
        {
            var item = repository.GetItem(Id);

            if (item is null) //code 204
            {
                return NotFound();
            }

            return item.AsDto();
        }

        /// <summary>
        // /items
        /// </summary>
        /// <param name="itemDto"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<ItemDto> CreateItem(CreateItemDto itemDto) //create the item, and then return the item you created
        {
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.Now
            };

            repository.CreateItem(item);
             
            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item.AsDto()); //can also use crate at route. 
            //get name of item passed to the GET action method, create a new item with the same id 
        }
           // The convention for a put (update to db) is to not return anything 
           // PUT /items/{id}
           [HttpPut("{id}")]
        public ActionResult UpdateItem(Guid id, UpdateItemDto itemDto)
        {
            var existingItem = repository.GetItem(id);

            if (existingItem == null)
            {
                return NotFound();
            }

            Item updatedItem = existingItem with // with kw used to create a copy of an object with the following properteies modified. Works with records which are immutable 
            {
                Name = itemDto.Name,
                Price = itemDto.Price
            };
            repository.UpdateItem(updatedItem); // Because the id stayed the same, this changes the origonal to the copy we just created 

            return NoContent();
        }


        //Just like with PUT (update) requests, the convention is to not return any content with deletes 
        //DELETE/items/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteItem(Guid id)
        {
            var existingItem = repository.GetItem(id);

            if (existingItem == null)
            {
                return NotFound();
            }

            repository.DeleteItem(id);

            return NoContent();
        }
    }
}
