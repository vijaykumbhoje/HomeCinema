using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using HomeCinema.Controllers;
using HomeCinema.Data;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Mappings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;

namespace HomeCinema.Tests.Controller
{
    [TestClass]
    public class RentalControllerTest
    {

        Mock<IEntityBaseRepository<Error>> _errorRepo = new Mock<IEntityBaseRepository<Error>>();
        Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork>();
        HomeCinemaContext _dbContext;
        List<Stock> stock = new List<Stock>();

        [TestMethod]
        public void ShouldGetMovieRentalHistory()
        {
            //Arrange
            var movieRepo = setupMoviesRepository();
            var customerRepo = SetupCustomerRepository();
            var stockRepo = setupStockRepository();
            var rentalRepo = SetupRentalRepository();
            var controller = new RentalsController(rentalRepo, customerRepo, stockRepo, movieRepo, _errorRepo.Object, _unitOfWork.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            //Act
            var response = controller.RentalHistory(controller.Request, 1);
            var responseString = GetResponseString(response);
            JObject obj = returnJString(responseString.Result);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void ShouldGetTotalMovieRentalHistory()
        {
            //Arrange
            var movieRepo = setupMoviesRepository();
            var customerRepo = SetupCustomerRepository();
            var stockRepo = setupStockRepository();
            var rentalRepo = SetupRentalRepository();
            var controller = new RentalsController(rentalRepo, customerRepo, stockRepo, movieRepo, _errorRepo.Object, _unitOfWork.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            //Act
            var response = controller.TotalRentalHistory(controller.Request);
            var responseString = GetResponseString(response);
            JObject obj = returnJString(responseString.Result);


            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(obj);


        }

        [TestMethod]
        public void ShouldReturnRentedMovie()
        {
            //Arrange
            var movieRepo = setupMoviesRepository();
            var customerRepo = SetupCustomerRepository();
            var stockRepo = setupStockRepository();
            var rentalRepo = SetupRentalRepository();
            var controller = new RentalsController(rentalRepo, customerRepo, stockRepo, movieRepo, _errorRepo.Object, _unitOfWork.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            //Act
            var response = controller.Return(controller.Request, 1);          

            //Assert
            Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode);
           
        }

        [TestMethod]
        public void ShouldRentMovie()
        {
            //Arrange
            var movieRepo = setupMoviesRepository();
            var customerRepo = SetupCustomerRepository();
            var stockRepo = setupStockRepository();
            var rentalRepo = SetupRentalRepository();
            var controller = new RentalsController(rentalRepo, customerRepo, stockRepo, movieRepo, _errorRepo.Object, _unitOfWork.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            //Act
            var response = controller.Rent(controller.Request, 1,2);
            var responseString = GetResponseString(response);
            JObject obj = JObject.Parse(responseString.Result);

            //Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual("Borrowed",obj["Status"].ToString());
        }

        #region Methods
        private EntityBaseRepository<Stock> setupStockRepository()
        {
            var stockRepo = new Mock<EntityBaseRepository<Stock>>(MockBehavior.Default, _dbContext);
            stock = new List<Stock>();
            stock.Add(new Stock { Id=1, MovieId = 1, UniqueKey = Guid.Parse("4EEAEC04-D90F-43C2-B996-BD0FE7750CF9"), isAvailble = true });
            stock.Add(new Stock { Id = 2, MovieId = 1, UniqueKey = Guid.Parse("7DCD83DF-52EB-49BE-A597-AA178198C76C"), isAvailble = true });
            stockRepo.Setup(s => s.GetAll()).Returns(stock.AsQueryable());
            return stockRepo.Object;
        }
        public EntityBaseRepository<Rental> SetupRentalRepository()
        {
           
           Stock stock1 = new Stock{Id=1, MovieId = 1, UniqueKey = Guid.Parse("4EEAEC04-D90F-43C2-B996-BD0FE7750CF9"), isAvailble = false };
            Stock stock2 = new Stock { Id = 2, MovieId = 1, UniqueKey = Guid.Parse("4EEAEC04-D90F-43C2-B996-BD0FE7750CF9"), isAvailble = true };


            var rentalRepo = new Mock<EntityBaseRepository<Rental>>(MockBehavior.Default, _dbContext);
            List<Rental> rentals = new List<Rental>();
            rentals.Add(new Rental { Id = 1, CustomerId = 1, StockId = 1, RentalDate = Convert.ToDateTime("2019-07-10 14:35:58.310"), ReturnDate = null, Status = "Borrowed", Stock = stock1 });
            rentals.Add(new Rental { Id = 2, CustomerId = 2, StockId = 1, RentalDate = Convert.ToDateTime("2019-08-11 14:35:58.310"), ReturnDate = Convert.ToDateTime("2019-08-12 14:35:58.310"), Status = "Returned", Stock = stock1 });
            rentals.Add(new Rental { Id = 3, CustomerId = 3, StockId = 2, RentalDate = Convert.ToDateTime("2019-09-12 14:35:58.310"), ReturnDate = Convert.ToDateTime("2019-08-13 14:35:58.310"), Status = "Returned", Stock = stock2 });
            rentals.Add(new Rental { Id = 1, CustomerId = 1, StockId = 1, RentalDate = Convert.ToDateTime("2019-10-14 14:35:58.310"), ReturnDate = null, Status = "Borrowed", Stock = stock1 });
            rentalRepo.Setup(r => r.GetAll()).Returns(rentals.AsQueryable());
#pragma warning disable CS0618 // Type or member is obsolete
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<DomainToViewModelMappingProfile>();
            });
#pragma warning restore CS0618 // Type or member is obsolete
            return rentalRepo.Object;
        }

        private EntityBaseRepository<Customer> SetupCustomerRepository()
        {
            var mockRepo = new Mock<EntityBaseRepository<Customer>>(MockBehavior.Default, _dbContext);
            var customerlist = new List<Customer>();
            customerlist.Add(new Customer { Id = 1, Email = "first@email.com", FirstName = "FirstName", LastName = "LastName", Mobile = "9178959595", IdentityCard = "9178959595" });
            customerlist.Add(new Customer { Id = 2, Email = "second@Email.com", FirstName = "SecondName", LastName = "SecondLast", Mobile = "9178400544", IdentityCard = "9178912595" });
            customerlist.Add(new Customer { Id = 3, Email = "third@Email.com", FirstName = "ThirdName", LastName = "ThirdLast", Mobile = "9178400545", IdentityCard = "9178959524" });
            mockRepo.Setup(c => c.GetAll()).Returns(customerlist.AsQueryable());
            return mockRepo.Object;
        }

        private EntityBaseRepository<Movie> setupMoviesRepository()
        {
            List<Rental> cust1Rental = new List<Rental>();
            cust1Rental.Add(new Rental { Id = 1, CustomerId = 1, StockId = 1, RentalDate = Convert.ToDateTime("2019-07-10 14:35:58.310"), ReturnDate = null, Status = "Borrowed" });
            cust1Rental.Add(new Rental { Id = 2, CustomerId = 1, StockId = 1, RentalDate = Convert.ToDateTime("2019-08-11 14:35:58.310"), ReturnDate = Convert.ToDateTime("2019-08-12 14:35:58.310"), Status = "Returned" });
            cust1Rental.Add(new Rental { Id = 3, CustomerId = 1, StockId = 2, RentalDate = Convert.ToDateTime("2019-07-10 14:35:58.310"), ReturnDate = null, Status = "Borrowed" });
            cust1Rental.Add(new Rental { Id = 4, CustomerId = 1, StockId = 2, RentalDate = Convert.ToDateTime("2018-03-04 14:35:58.310"), ReturnDate = Convert.ToDateTime("2018-03-10 14:35:58.310"), Status = "Returned" });


            List<Stock> minions = new List<Stock>();
            minions.Add(new Stock { Id = 1, MovieId = 1, UniqueKey = Guid.Parse("4EEAEC04-D90F-43C2-B996-BD0FE7750CF9"), isAvailble = true, Rentals = cust1Rental });
            minions.Add(new Stock { Id = 2, MovieId = 1, UniqueKey = Guid.Parse("7DCD83DF-52EB-49BE-A597-AA178198C76C"), isAvailble = false, Rentals = cust1Rental });



            var movieRepo = new Mock<EntityBaseRepository<Movie>>(MockBehavior.Default, _dbContext);
            List<Movie> movies = new List<Movie>();

            movies.Add(new Movie
            {
                Id = 1,
                Title = "Minions",
                Description = "hatches a plot to take over the world.,",
                Image = "minions.jpg",
                Writer = "Brian Lynch",
                Director = "Kyle Bald",
                Producer = "Janet Healy",
                GenreId = 1,
                ReleaseDate = Convert.ToDateTime("2015-07-10 00:00:00.000"),
                Rating = 3,
                TrailerUrl = "https://www.youtube.com/watch?v=Wfql_DoHRKc",
                Stocks = minions
            });

            movies.Add(new Movie
            {
                Id = 2,
                Title = "Ted 2",
                Description = "hatches a plot to take over the world.,",
                Image = "ted2.jpg",
                Writer = "Seth MacFarlane",
                Director = "Seth MacFarlane",
                Producer = "Jason Clark",
                GenreId = 1,
                ReleaseDate = Convert.ToDateTime("2015-06-27 00:00:00.000"),
                Rating = 4,
                TrailerUrl = "https://www.youtube.com/watch?v=S3AVcCggRnU",
                Stocks = stock
            });

            movies.Add(new Movie
            {
                Id = 3,
                Title = "Trainwreck",
                Description = "hatches a plot to take over the world.,",
                Image = "minions.jpg",
                Writer = "Brian Lynch",
                Director = "Kyle Bald",
                Producer = "Janet Healy",
                GenreId = 4,
                ReleaseDate = Convert.ToDateTime("2015-06-19 00:00:00.000"),
                Rating = 4,
                TrailerUrl = "https://www.youtube.com/watch?v=Wfql_DoHRKc",
                Stocks = stock
            });

            movies.Add(new Movie
            {
                Id = 4,
                Title = "Inside Out",
                Description = "hatches a plot to take over the world.,",
                Image = "minions.jpg",
                Writer = "Brian Lynch",
                Director = "Kyle Bald",
                Producer = "Janet Healy",
                GenreId = 1,
                ReleaseDate = Convert.ToDateTime("2015-05-27 00:00:00.000"),
                Rating = 3,
                TrailerUrl = "https://www.youtube.com/watch?v=Wfql_DoHRKc",
                Stocks = stock
            });

            movies.Add(new Movie
            {
                Id = 5,
                Title = "Home",
                Description = "hatches a plot to take over the world.,",
                Image = "minions.jpg",
                Writer = "Brian Lynch",
                Director = "Kyle Bald",
                Producer = "Janet Healy",
                GenreId = 1,
                ReleaseDate = Convert.ToDateTime("2015-09-21 00:00:00.000"),
                Rating = 3,
                TrailerUrl = "https://www.youtube.com/watch?v=Wfql_DoHRKc",
                Stocks = stock
            });

            movies.Add(new Movie
            {
                Id = 6,
                Title = "Batman v Superman: Dawn of Justice",
                Description = "hatches a plot to take over the world.,",
                Image = "minions.jpg",
                Writer = "Brian Lynch",
                Director = "Kyle Bald",
                Producer = "Janet Healy",
                GenreId = 1,
                ReleaseDate = Convert.ToDateTime("2015-07-17 00:00:00.000"),
                Rating = 3,
                TrailerUrl = "https://www.youtube.com/watch?v=Wfql_DoHRKc",
                Stocks = stock
            });

            movies.Add(new Movie
            {
                Id = 6,
                Title = "Ant-Man",
                Description = "hatches a plot to take over the world.,",
                Image = "minions.jpg",
                Writer = "Brian Lynch",
                Director = "Kyle Bald",
                Producer = "Janet Healy",
                GenreId = 1,
                ReleaseDate = Convert.ToDateTime("2015-07-17 00:00:00.000"),
                Rating = 3,
                TrailerUrl = "https://www.youtube.com/watch?v=Wfql_DoHRKc",
                Stocks = stock
            });

            movieRepo.Setup(m => m.GetAll()).Returns(movies.AsQueryable());



            return movieRepo.Object;
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
