using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        }

        #region Methods
        private EntityBaseRepository<Stock> setupStockRepository()
        {
            var stockRepo = new Mock<EntityBaseRepository<Stock>>(MockBehavior.Default, _dbContext);
            stock = new List<Stock>();
            stock.Add(new Stock { Id = 1, MovieId = 1, UniqueKey = Guid.Parse("4EEAEC04-D90F-43C2-B996-BD0FE7750CF9"), isAvailble = true });
            stock.Add(new Stock { Id = 2, MovieId = 1, UniqueKey = Guid.Parse("7DCD83DF-52EB-49BE-A597-AA178198C76C"), isAvailble = false });
            stock.Add(new Stock { Id = 3, MovieId = 2, UniqueKey = Guid.Parse("C723CC9E-3DC2-4534-8918-C186D16DE948"), isAvailble = true });
            stock.Add(new Stock { Id = 4, MovieId = 3, UniqueKey = Guid.Parse("A7C2FA62-FB64-479E-ABA9-31FD2E9769BD"), isAvailble = true });
            stock.Add(new Stock { Id = 5, MovieId = 3, UniqueKey = Guid.Parse("C8022BA4-55E5-4717-B20B-45A6D55E9E7B"), isAvailble = false });
            stock.Add(new Stock { Id = 6, MovieId = 4, UniqueKey = Guid.Parse("A46E05AA-7458-4EC7-A070-8B104D5ECB79"), isAvailble = true });
            stock.Add(new Stock { Id = 7, MovieId = 4, UniqueKey = Guid.Parse("82707365-FE94-47F7-98ED-C07DFF306D21"), isAvailble = false });
            stockRepo.Setup(s => s.GetAll()).Returns(stock.AsQueryable());
            return stockRepo.Object;
        }
        public EntityBaseRepository<Rental> SetupRentalRepository()
        {
            var rentalRepo = new Mock<EntityBaseRepository<Rental>>(MockBehavior.Default, _dbContext);
            List<Rental> rentals = new List<Rental>();
            rentals.Add(new Rental {Id=1, CustomerId=1, StockId= 1, RentalDate= Convert.ToDateTime("2019-07-10 14:35:58.310"), ReturnDate=null, Status="Borrowed" });
            rentals.Add(new Rental { Id = 2, CustomerId = 2, StockId = 2, RentalDate = Convert.ToDateTime("2019-08-11 14:35:58.310"), ReturnDate = Convert.ToDateTime("2019-08-12 14:35:58.310"), Status = "Returned" });
            rentals.Add(new Rental { Id = 3, CustomerId = 3, StockId = 3, RentalDate = Convert.ToDateTime("2019-09-12 14:35:58.310"), ReturnDate = Convert.ToDateTime("2019-08-13 14:35:58.310"), Status = "Returned" });
            rentals.Add(new Rental { Id = 1, CustomerId = 1, StockId = 1, RentalDate = Convert.ToDateTime("2019-10-14 14:35:58.310"), ReturnDate = null, Status = "Borrowed" });
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
                Stocks = stock
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

        #endregion
    }
}
