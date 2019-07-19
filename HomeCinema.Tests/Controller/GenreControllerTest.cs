using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HomeCinema.Controllers;
using HomeCinema.Data;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AutoMapper;
using HomeCinema.Mappings;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace HomeCinema.Tests.Controller
{
    [TestClass]
    public class GenreControllerTest
    {         

           
        Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork>();
        Mock<IEntityBaseRepository<Error>> _errorRepo = new Mock<IEntityBaseRepository<Error>>();
        HomeCinemaContext _dbContext;

      
        [TestMethod]
        public void ShouldGetGenre()
        {

            //Arrange
            var GenreRepo = SetupGenreRepo();
            var controller = new GenresController(GenreRepo, _errorRepo.Object, _unitOfWork.Object);
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());

            //Act
            var response = controller.Get(controller.Request);            

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        #region 'Methods'
        private EntityBaseRepository<Genre> SetupGenreRepo()
        {
#pragma warning disable CS0618 // Type or member is obsolete

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<DomainToViewModelMappingProfile>();
            });
#pragma warning restore CS0618 // Type or member is obsolete
            var genreRepo = new Mock<EntityBaseRepository<Genre>>(MockBehavior.Default, _dbContext);
            List<Genre> genre = new List<Genre>();
            genre.Add(new Genre { Id = 1, Name = "Comedy" });
            genre.Add(new Genre { Id = 1, Name = "Sci-fi" });
            genre.Add(new Genre { Id = 1, Name = "Action" });
            genre.Add(new Genre { Id = 1, Name = "Horror" });
            genre.Add(new Genre { Id = 1, Name = "Romance" });
            genre.Add(new Genre { Id = 1, Name = "Crime" });
           
            genreRepo.Setup(g => g.GetAll()).Returns(genre.AsQueryable());

            return genreRepo.Object;
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
