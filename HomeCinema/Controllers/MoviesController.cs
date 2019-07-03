using AutoMapper;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Infrastructure.Core;
using HomeCinema.Infrastructure.Extensions;
using HomeCinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace HomeCinema.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/movies")]
    public class MoviesController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Movie> _moviesRepository;

        public MoviesController(IEntityBaseRepository<Movie> movieRepository, IEntityBaseRepository<Error> _errorRepository, IUnitOfWork  _unitOfWork)
            :base (_errorRepository, _unitOfWork)
        {
            _moviesRepository = movieRepository;
        }

        [AllowAnonymous]
        [Route("latest")]
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var movies = _moviesRepository.GetAll().OrderByDescending(m => m.ReleaseDate).Take(6).ToList();
                IEnumerable<MovieViewModel> moviesVM = Mapper.Map<IEnumerable<Movie>, IEnumerable<MovieViewModel>>(movies);
                response = request.CreateResponse(HttpStatusCode.OK, moviesVM);
                return response;
            });
        }
        
        [AllowAnonymous]
        [Route("{pages:int=0}/{pagesize=3}/{filter?}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int? page, int? pageSize, string filter=null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () => {
                HttpResponseMessage response = null;
                List<Movie> movies = null;
                int totalMovies = new int();
                if (!String.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();
                    movies = _moviesRepository.GetAll().OrderBy(m => m.Id).Where(m => m.Title.ToLower()
                    .Contains(filter)).ToList();
                }
                else
                {
                    movies = _moviesRepository.GetAll().ToList();
                }

                totalMovies = movies.Count();
                movies = movies.Skip(currentPage * currentPageSize).Take(currentPageSize).ToList();

                IEnumerable<MovieViewModel> movieVm = Mapper.Map<IEnumerable<Movie>, IEnumerable<MovieViewModel>>(movies);

                PaginationSet<MovieViewModel> pagedSet = new PaginationSet<MovieViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalMovies,
                    TotalPages = (int)Math.Ceiling((decimal)totalMovies/currentPageSize),
                    items = movieVm
                };

                response = request.CreateResponse<PaginationSet<MovieViewModel>>(HttpStatusCode.OK, pagedSet);
                return response;    
            });
        }

        [Route("details/{id:int}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () => {
                HttpResponseMessage response = null;
                var movie = _moviesRepository.GetSingle(id);
                MovieViewModel movieVM = Mapper.Map<Movie, MovieViewModel>(movie);
                response = request.CreateResponse<MovieViewModel>(HttpStatusCode.OK, movieVM);
                return response;
            });
        }

        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Add(HttpRequestMessage request, MovieViewModel movieVm)
        {
           
            return CreateHttpResponse(request, () => {
                HttpResponseMessage response = null;
                if(!ModelState.IsValid)
                {
                    response = request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }else
                {
                    Movie newMovie = new Movie();
                    newMovie.UpdateMovie(movieVm);
                    for(int i =0; i<=movieVm.NumberOfStocks; i++)
                    {
                        Stock stock = new Stock()
                        {
                            isAvailble = true,
                            Movie = newMovie,
                            UniqueKey = Guid.NewGuid()
                        };
                        newMovie.Stocks.Add(stock);
                    }
                    _moviesRepository.Add(newMovie);
                    _unitOfWork.Commit();
                    movieVm = Mapper.Map<Movie, MovieViewModel>(newMovie);
                    response = request.CreateResponse(HttpStatusCode.OK, movieVm);
                }
                return response;
            });
        }
    }
}