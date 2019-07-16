using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HomeCinema.Controllers;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;

using Moq;
using NUnit.Framework;
using HomeCinema.Tests.Helper;
using HomeCinema.Data;
using HomeCinema.Data.Infrastructure;
using System.Net.Mail;
using System.Net.Sockets;
using System.Web.Http.Hosting;
using System.Web.Http;
using Newtonsoft.Json;
using System.Net;

namespace HomeCinema.Tests.Controller
{
    [TestFixture]
    public class CustomerControllerTest
    {
        private EntityBaseRepository<Customer> _customerRepository;
        private IUnitOfWork _unitOfWork;
        private List<Customer> _customers;


        [SetUp]
        public void Setup()
        {
            _customers = SetUpCustomers();
            var customerRepository = new Mock<IEntityBaseRepository<Customer>>();
                
        }


        private static List<Customer> SetUpCustomers()
        {
            var custId = new int();
            var customer = DataInitializer.GetAllCustomers();
            foreach(Customer cust in customer)
                  cust.Id = ++custId;
            return customer;
        }
        [Test]
        public void CustomerShouldGetRegistered()
        {
            Assert.Inconclusive();
        }
        [Test]
        public void ShouldReturnCustomerBasedOnFilter()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void ShouldReturnCustomerBasedOnID()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void ShouldReturnSearchedCustomer()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void ShouldUpdateCustomer()
        {
            Assert.Inconclusive();
        }
    }
}
