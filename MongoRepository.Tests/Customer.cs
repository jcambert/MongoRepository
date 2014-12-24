using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoRepository.Tests
{
    [CollectionName("Customers")]
    public class Customer//:Entity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        public string Id
        {
            get;
            set;
        }
        public string FirstName { get; set; }

        public string LastName{ get; set; }

        public List<Order> Orders { get; set; }
    }


    public class Order : Entity
    {
        public int Number { get; set; }
    }
}
