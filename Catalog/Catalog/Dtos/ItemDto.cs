using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Dtos
{ // DTO: Object that determines the shape of the data you send to the client. Needs to fit into their front end. 
    public class ItemDto
    {
        public Guid Id { get; init; } // init, only set the value during initialization. similar to a private set. property cannot be modified after initialization\
        public string Name { get; init; }
        public decimal Price { get; init; }
        public DateTimeOffset CreatedDate { get; init; }

    }
}
