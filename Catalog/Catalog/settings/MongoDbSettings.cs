using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.settings
{
    public class MongoDbSettings
    {
        //declare settings from appsettings.json as properties 
        public string Host { get; set; }
        public int Port { get; set; }

        public string ConnectionString 
        {get
            {
                return $"mongodb://{Host}:{Port}";
            }
        
        }




    }
}
