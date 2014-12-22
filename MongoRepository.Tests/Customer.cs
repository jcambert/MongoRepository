using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoRepository.Tests
{
    [CollectionName("Customers")]
    public class Customer:Entity
    {
        public string FirstName { get; set; }

        public string LastName{ get; set; }

        public List<Order> Orders { get; set; }
    }


    public class Order : Entity
    {
        public int Number { get; set; }
    }
}
