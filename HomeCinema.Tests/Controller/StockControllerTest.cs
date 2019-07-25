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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace HomeCinema.Tests.Controller
{
    [TestClass]
   public class StockControllerTest
    {
        Mock<IEntityBaseRepository<Error>> _errorRepo = new Mock<IEntityBaseRepository<Error>>();
        Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork>();
        HomeCinemaContext _dbContext;

        [TestMethod]
        public void Stock_GetAvailableStock_ShouldGetAvailableStock()
        {

            //Arrange 
            Mapper.Reset();
#pragma warning disable CS0618 // Type or member is obsolete
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<DomainToViewModelMappingProfile>();
            });
#pragma warning restore CS0618 // Type or member is obsolete
            var stockRepo = new Mock<EntityBaseRepository<Stock>>(MockBehavior.Default, _dbContext);

            List<Stock> stock = new List<Stock>();
            stock.Add(new Stock { Id = 1, MovieId = 1, UniqueKey = Guid.Parse("4EEAEC04-D90F-43C2-B996-BD0FE7750CF9"), isAvailble = true });
            stock.Add(new Stock { Id = 2, MovieId = 1, UniqueKey = Guid.Parse("7DCD83DF-52EB-49BE-A597-AA178198C76C"), isAvailble = true });
            stock.Add(new Stock { Id = 3, MovieId = 2, UniqueKey = Guid.Parse("C723CC9E-3DC2-4534-8918-C186D16DE948"), isAvailble = true });
            stock.Add(new Stock { Id = 4, MovieId = 3, UniqueKey = Guid.Parse("A7C2FA62-FB64-479E-ABA9-31FD2E9769BD"), isAvailble = true });
            stockRepo.Setup(s => s.GetAll()).Returns(stock.AsQueryable());

            var controller = new StocksController( stockRepo.Object,  _errorRepo.Object, _unitOfWork.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            //Act
            var response = controller.Get(controller.Request, 1);

            var responseString = GetResponseString(response);
            JObject obj = returnJString(responseString.Result);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(obj);
        }


        #region Methods
        

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
