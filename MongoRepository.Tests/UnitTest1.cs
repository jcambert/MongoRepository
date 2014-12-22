using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MongoRepository.Tests
{
    [TestClass]
    public class UnitTest1
    {

        private static MongoRepository<string, Customer> Customers;

        [ClassInitialize]
        public static void Initialize(TestContext ctx)
        {
            // Dynamic.Audit = true;
           
            Customers = Repositories.GetCollection<Customer>();
            Customers.DeleteAll(true);
            Assert.IsTrue(Customers.Count() == 0);


        }

        [TestMethod]
        public void TestMethod1()
        {
            var rep=Repositories.GetCollection<Customer>();
            Assert.IsNotNull(rep);

            var customer= rep.New();
            Assert.IsNotNull(customer);

            customer.Add();
            Assert.IsTrue(!string.IsNullOrEmpty(customer.Id));

            customer.FirstName = "Ambert";
            customer.LastName = "jean-christophe";
            customer.Update();


            var order = new Order();
            order.Number = 1;

            customer.Orders = new System.Collections.Generic.List<Order>();
            customer.Orders.Add(order);

            customer.Update();

            var cus = Customers.GetById(customer.Id);
            Assert.IsNotNull(cus);
            Assert.AreEqual(cus, customer);
            Assert.IsTrue(cus.Orders.Count == 1);

            cus.LastName = "jc";

            cus.Update();
        }

    }
}
