using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Infrastructure.Core;
using HomeCinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using AutoMapper;
using System.Net;
using System.Web.Http;
using HomeCinema.Services.Auth;

namespace HomeCinema.Controllers
{
    
    [RoutePrefix("api/genres")]
    public class GenresController : ApiControllerBase
    {

        private readonly IEntityBaseRepository<Genre> _genreRepository;

        public GenresController(IEntityBaseRepository<Genre> genreRepository, IEntityBaseRepository<Error> _errorRepository, IUnitOfWork _unitOfWork)
            : base (_errorRepository, _unitOfWork)
        {
            _genreRepository = genreRepository;
        }

        [AllowAnonymous]
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () => 
            {
                HttpResponseMessage response = null;
                var genre = _genreRepository.GetAll().ToList();
                IEnumerable<GenreViewModel> genreVM = Mapper.Map<IEnumerable<Genre>, IEnumerable<GenreViewModel>>(genre);
                response = request.CreateResponse(HttpStatusCode.OK, genreVM);
                return response;
            });
        }
    }
}