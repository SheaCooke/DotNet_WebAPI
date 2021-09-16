using Catalog.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.Repositories
{
    public interface IItemsRepository
    {
        Task<Item> GetItemAsync(Guid id); //Task: task of item, not syncronous, you will not get the item right away, you will get a task that will eventually return an item. Turns method into Async method 
        Task<IEnumerable<Item>> GetItemsAsync();

        Task CreateItemAsync(Item item); //only receives an item 

        Task UpdateItemAsync(Item item);

        Task DeleteItemAsync(Guid id); // void -> Task for Async 
    }
}