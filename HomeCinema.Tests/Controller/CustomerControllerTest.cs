using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

using System.Threading.Tasks;
using HomeCinema.Controllers;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;

using Moq;
using NUnit.Framework;
using HomeCinema.Tests.Helper;
using HomeCinema.Data;
using HomeCinema.Data.Infrastructure;

using System.Web.Http;

using System.Net;
using HomeCinema.Models;
using AutoMapper;
using HomeCinema.Mappings;
using Newtonsoft.Json.Linq;


namespace HomeCinema.Tests.Controller
{
    [TestFixture]
    public class CustomerControllerTest
    {
        private EntityBaseRepository<Customer> _customerRepository;
        private IUnitOfWork _unitOfWork;

        private List<Customer> _customers;
        private HomeCinemaContext _dbEntities;

        [SetUp]
        public void Setup()
        {
            _customers = SetUpCustomers();
            //  _dbEntities = new Mock<HomeCinemaContext>().Object;
            var customerRepository = SetupCustomerRepository();
            var errorRepository = new Mock<IEntityBaseRepository<Error>>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(c => c.customerRepository).Returns(_customerRepository);
            _unitOfWork = unitOfWork.Object;
            var controller = new CustomerController(customerRepository, errorRepository.Object, _unitOfWork);
            Mapper.Reset();
#pragma warning disable CS0618 // Type or member is obsolete
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<DomainToViewModelMappingProfile>();
            });
#pragma warning restore CS0618 // Type or member is obsolete
        }

    
        [Test]
        public void Customer_Registration_ShouldGetRegistered()
        {
            //Arrange
            var customerRepository = SetupCustomerRepository();
            var errorRepository = new Mock<IEntityBaseRepository<Error>>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var controller = new CustomerController(customerRepository, errorRepository.Object, _unitOfWork);
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());
            CustomerViewModel customerVm = new CustomerViewModel
            {
                ID = 4,
                Email = "fourth@email.com",
                FirstName = "fourthName",
                LastName = "fourthLastName",
                Mobile = "4444444444",
                IdentityCard = "13234567898745"
            };

            //Act
            var response = controller.Register(controller.Request, customerVm);
            var responseString = GetResponseString(response);
            JObject obj = JObject.Parse(responseString.Result);

            //Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual("fourthName", obj["FirstName"].ToString());

        }

        [Test]
        public void Customer_GetCustomerByFilterString_ReturnCustomerWithMatchingFilter()
        {
            //Arrange           
            var customerRepository = SetupCustomerRepository(); //new Mock<IEntityBaseRepository<Customer>>();          
            var errorRepository = new Mock<IEntityBaseRepository<Error>>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var controller = new CustomerController(customerRepository, errorRepository.Object, _unitOfWork);
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());
            string filterString = "sec";

            //Act
            var response = controller.Get(controller.Request, filterString);
            var responseString = GetResponseString(response);
            JObject obj = returnJString(responseString.Result);

            //Assert                       
            Assert.AreEqual(2, (int)obj["ID"]);
            Assert.IsTrue(obj["FirstName"].ToString().ToLower().Contains(filterString));
        }

        [Test]
        public void Customer_GetCustomerByID_ShouldReturnCustomerBasedOnID()
        {
            //Arrange
            var customerRepository = SetupCustomerRepository();
            var errorRepository = new Mock<IEntityBaseRepository<Error>>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var controller = new CustomerController(customerRepository, errorRepository.Object, _unitOfWork);
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());

            //Act
            var response = controller.Get(controller.Request, 2);
            var responseString = GetResponseString(response);
            JObject obj = JObject.Parse(responseString.Result);

            //Assert            
            Assert.AreEqual(2, (int)obj["ID"]);
        }

        [Test]
        public void Customer_SearchCustomer_ShouldReturnSearchedCustomerOrAll()
        {

            //Arrange
            var customerRepository = SetupCustomerRepository();
            var errorRepository = new Mock<IEntityBaseRepository<Error>>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var controller = new CustomerController(customerRepository, errorRepository.Object, _unitOfWork);
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());
            int cnt = customerRepository.GetAll().Count();

            //Act
            var response = controller.Search(controller.Request, 0, 4, string.Empty);
            var responseString = GetResponseString(response);
            JObject obj = JObject.Parse(responseString.Result);

            //Assert
            Assert.AreEqual(cnt, (int)obj["Count"]);

        }

        [Test]
        public void Customer_Update_ShouldUpdateCustomer()
        {
            //Arrange
            var customerRepository = SetupCustomerRepository();
            var errorRepository = new Mock<IEntityBaseRepository<Error>>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var controller = new CustomerController(customerRepository, errorRepository.Object, _unitOfWork);
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());
            CustomerViewModel customerVm = new CustomerViewModel
            {
                ID = 1,
                Email = "first@email.com",
                FirstName = "FirstName",
                LastName = "FirstLastName",
                Mobile = "4444444444",
                IdentityCard = "13234567898745"
            };

            //Act
            var response = controller.Update(controller.Request, customerVm);
            var responseString = GetResponseString(response);
            JObject obj = JObject.Parse(responseString.Result);

            //Assert
            Assert.AreEqual("FirstLastName", obj["LastName"].ToString());
        }

        #region 'Private Methods'
        private EntityBaseRepository<Customer> SetupCustomerRepository()
        {
            var mockRepo = new Mock<EntityBaseRepository<Customer>>(MockBehavior.Default, _dbEntities);
            var customerlist = new List<Customer>();
            customerlist.Add(new Customer { Id = 1, Email = "first@email.com", FirstName = "FirstName", LastName = "LastName", Mobile = "9178959595", IdentityCard = "9178959595" });
            customerlist.Add(new Customer { Id = 2, Email = "second@Email.com", FirstName = "SecondName", LastName = "SecondLast", Mobile = "9178400544", IdentityCard = "9178912595" });
            customerlist.Add(new Customer { Id = 3, Email = "third@Email.com", FirstName = "ThirdName", LastName = "ThirdLast", Mobile = "9178400545", IdentityCard = "9178959524" });
            mockRepo.Setup(c => c.GetAll()).Returns(customerlist.AsQueryable());
            return mockRepo.Object;
        }

        private static List<Customer> SetUpCustomers()
        {
            var custId = new int();
            var customer = DataInitializer.GetAllCustomers();
            foreach (Customer cust in customer)
                cust.Id = ++custId;
            return customer;
        }

        public async Task<string> GetResponseString(HttpResponseMessage response)
        {
            return await response.Content.ReadAsStringAsync();
        }

        public JObject returnJString(string task)
        {

            JArray jArray = JArray.Parse(task);
            JObject obj = JObject.Parse(jArray[0].ToString());
            return obj;
        }
        #endregion

    }
}
