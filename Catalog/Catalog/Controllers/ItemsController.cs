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


    }
}
