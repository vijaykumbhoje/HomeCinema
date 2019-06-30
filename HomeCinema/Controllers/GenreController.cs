using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Infrastructure.Core;
using HomeCinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using AutoMapper;
using System.Net;

namespace HomeCinema.Controllers
{
    [Authorize(Roles ="Admin")]
    [RoutePrefix("api/genres")]
    public class GenreController : ApiControllerBase
    {

        private readonly IEntityBaseRepository<Genre> _genreRepository;

        public GenreController(IEntityBaseRepository<Genre> genreRepository, IEntityBaseRepository<Error> _errorRepository, UnitOfWork _unitOfWork)
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
                response = request.CreateResponse<IEnumerable<GenreViewModel>>(HttpStatusCode.OK, genreVM);
                return response;
            });
        }
    }
}