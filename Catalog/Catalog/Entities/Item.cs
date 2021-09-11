using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Entities
{
    public record Item //Record type, makes object immutable
    {
        public Guid Id { get; init; } // init, only set the value during initialization. similar to a private set. property cannot be modified after initialization\
        public string Name { get; init; }
        public decimal Price { get; init; }
        public DateTimeOffset CreatedDate { get; init; }


    }



}
