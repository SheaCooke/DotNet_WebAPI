using Catalog.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Repositories
{
    public class MongoDbItemsRepository : IItemsRepository
    {
        private readonly IMongoCollection<Item> itemsCollection;//mongo db uses collections to associate items 
        private const string databaseName = "catalog";
        private const string collectionName = "items";
        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter; //filter items to be returned as their found in the collection. similar to using .Where()
        public MongoDbItemsRepository(IMongoClient mongoClient) // need to receive instance of mongo db client in constructor. nuget MongoDb.Driver
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName); // gets reference to database
            itemsCollection = database.GetCollection<Item>(collectionName); //gets reference to collection 
            
            
        }
        //run docker: docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db mongo 
        //docker normaly listens on port 27017. normal location for data inside container is /data/db
        

        public void CreateItem(Item item)
        {
            itemsCollection.InsertOne(item);
        }

        public void DeleteItem(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);//first, build filter
            itemsCollection.DeleteOne(filter);
        }

        public Item GetItem(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);//first, build filter
            return itemsCollection.Find(filter).SingleOrDefault(); 

            
        }

        public IEnumerable<Item> GetItems()
        {
            return itemsCollection.Find(new BsonDocument()).ToList();
        }

        public void UpdateItem(Item item)
        {
            var filter = filterBuilder.Eq(existingItem => existingItem.Id, item.Id); //first, build filter
            itemsCollection.ReplaceOne(filter, item);
        }
    }
}
