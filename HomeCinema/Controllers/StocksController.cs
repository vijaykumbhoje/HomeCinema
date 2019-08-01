using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Data.Extensions;
using HomeCinema.Entities;
using HomeCinema.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AutoMapper;
using HomeCinema.Models;
using System.Net;
using HomeCinema.Services.Auth;

namespace HomeCinema.Controllers
{
    //[Authorize(Roles ="Admin")]

    [RoutePrefix("api/stocks")]
    public class StocksController : ApiControllerBase
    {
      
        private readonly IEntityBaseRepository<Stock> _stockRepository;

        public StocksController(IEntityBaseRepository<Stock> stockRepository, IEntityBaseRepository<Error> _errorRepository, IUnitOfWork _unitOfWork)
            : base (_errorRepository, _unitOfWork)
        {
            _stockRepository = stockRepository;
        }

        [Route("movie/{id:int}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int Id)
        {
            IEnumerable<Stock> stocks = null;
            return CreateHttpResponse(request, () => {
                HttpResponseMessage response = null;
                stocks = _stockRepository.GetAvailableItems(Id);
                IEnumerable<StockViewModel> stocksVm = Mapper.Map<IEnumerable<Stock>, IEnumerable < StockViewModel >> (stocks);
                response = request.CreateResponse(HttpStatusCode.OK, stocksVm);
                return response;
            });
        }       
    }
}