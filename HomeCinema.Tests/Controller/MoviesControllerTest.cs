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
using HomeCinema.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;

namespace HomeCinema.Tests.Controller
{
    [TestClass]
    public class MoviesControllerTest
    {

        HomeCinemaContext _dbContext;
        Mock<IEntityBaseRepository<Error>> _errorRepo = new Mock<IEntityBaseRepository<Error>>();
        Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork>();

        [TestMethod]
        public void Movies_GetLatestMovies_ShouldGetLatestMovies()
        {
            //Arrange
            var movieRepo = setupMoviesRepository();            
            var controller = new MoviesController(movieRepo,_errorRepo.Object, _unitOfWork.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            //Act
            var response = controller.Get(controller.Request);          

            //Assert
            Assert.IsNotNull(response.Content);            
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void Movies_GetAllMovies_ShouldReturnAllMovies()
        {
            //Arrange
            var movieRepo = setupMoviesRepository();           
            var controller = new MoviesController(movieRepo, _errorRepo.Object, _unitOfWork.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();
            int cnt = movieRepo.GetAll().Count();
            //Act
            var response = controller.Get(controller.Request, 0, 4, string.Empty);
                      
            var responseString = GetResponseString(response);
            JObject obj = JObject.Parse(responseString.Result);

            //Assert
            Assert.IsNotNull(response.Content);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(cnt, (int)obj["TotalCount"]);

        }

        [TestMethod]
        public void Movie_GetwithParameterID_ShouldGetMovieByID()

        {
            //Arrange
            var movieRepo = setupMoviesRepository();            
            var controller = new MoviesController(movieRepo, _errorRepo.Object, _unitOfWork.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();
           
            //Act
            var response = controller.Get(controller.Request, 4);
            var responseString = GetResponseString(response);
            JObject obj = JObject.Parse(responseString.Result);

            //Assert
            Assert.IsNotNull(response.Content);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(4, (int)obj["ID"]);
        }

        [TestMethod]
        public void Movie_Add_ShouldAddMovie()
        {
            //Arrange
            var movieRepo = setupMoviesRepository();
            var controller = new MoviesController(movieRepo, _errorRepo.Object, _unitOfWork.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();
            MovieViewModel movieVm = new MovieViewModel
            {
                ID=8,
                Title="Dark Knight",
                Description = "The country needs better criminals.",
                Image = "DarkKight.jpg",
                Writer = "Brian Lynch",
                Director = "Kyle Bald",
                Producer = "Janet Healy",
                GenreId = 2,
                ReleaseDate = Convert.ToDateTime("2012-07-12 00:00:00.000"),
                Rating = 5,               
            };

            //Act
            var response = controller.Add(controller.Request, movieVm);
            var responseString = GetResponseString(response);
            JObject obj = JObject.Parse(responseString.Result);

            //Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual("Dark Knight", obj["Title"].ToString());       
        }

        [TestMethod]
        public void Movie_Update_ShouldUpdateMovie()
        {
            //Arrange
            var movieRepo = setupMoviesRepository();
            var controller = new MoviesController(movieRepo, _errorRepo.Object, _unitOfWork.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();
            MovieViewModel movieVm = new MovieViewModel
            {
                ID = 7,
                Title = "Dark Knight",
                Description = "The country needs better criminals.",
                Image = "DarkKight.jpg",
                Writer = "Brian Lynch",
                Director = "Kyle Bald",
                Producer = "Janet Healy",
                GenreId = 2,
                ReleaseDate = Convert.ToDateTime("2012-07-12 00:00:00.000"),
                Rating = 5,
            };

            //Act
            var response = controller.Update(controller.Request, movieVm);
            var responseString = GetResponseString(response);
            JObject obj = JObject.Parse(responseString.Result);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
           
            Assert.AreEqual("Dark Knight", obj["Title"].ToString());
        }

        #region
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
                TrailerUrl= "https://www.youtube.com/watch?v=Wfql_DoHRKc"
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
                TrailerUrl = "https://www.youtube.com/watch?v=S3AVcCggRnU"
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
                TrailerUrl = "https://www.youtube.com/watch?v=Wfql_DoHRKc"
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
                TrailerUrl = "https://www.youtube.com/watch?v=Wfql_DoHRKc"
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
                TrailerUrl = "https://www.youtube.com/watch?v=Wfql_DoHRKc"
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
                TrailerUrl = "https://www.youtube.com/watch?v=Wfql_DoHRKc"
            });

            movies.Add(new Movie
            {
                Id = 7,
                Title = "Ant-Man",
                Description = "hatches a plot to take over the world.,",
                Image = "minions.jpg",
                Writer = "Brian Lynch",
                Director = "Kyle Bald",
                Producer = "Janet Healy",
                GenreId = 1,
                ReleaseDate = Convert.ToDateTime("2015-07-17 00:00:00.000"),
                Rating = 3,
                TrailerUrl = "https://www.youtube.com/watch?v=Wfql_DoHRKc"
            });

            movieRepo.Setup(m => m.GetAll()).Returns(movies.AsQueryable());
            Mapper.Reset();
            #pragma warning disable CS0618 // Type or member is obsolete

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<DomainToViewModelMappingProfile>();
            });
           #pragma warning restore CS0618 // Type or member is obsolete
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
