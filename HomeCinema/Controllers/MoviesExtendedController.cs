using AutoMapper;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Entities;
using HomeCinema.Infrastructure.Core;
using HomeCinema.Infrastructure.Extensions;
using HomeCinema.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HomeCinema.Controllers
{
    [Authorize(Roles="Admin")]
    [RoutePrefix("apimoviesextended")]
    public class MoviesExtendedController : ApiControllerBaseExtended
    {
        public MoviesExtendedController(IDataRepositoryFactory dataRepository, IUnitOfWork unitOfWork)
            :base(dataRepository, unitOfWork){ }

        [AllowAnonymous]
        [Route("latest")]
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            _requiredRepositories = new List<Type>() { typeof(Movie) };
            return CreateHttpResponse(request,_requiredRepositories, () => {
                HttpResponseMessage response = null;
                var movies = _moviesRepository.GetAll().OrderByDescending(m => m.ReleaseDate).Take(6).ToList();
                IEnumerable<MovieViewModel> movieVm = Mapper.Map<IEnumerable<Movie>, IEnumerable<MovieViewModel>> (movies);
                response = request.CreateResponse(HttpStatusCode.OK, movieVm);
                return response;
            });

        }

        [Route("details/{id:int}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int id)
        {
            _requiredRepositories = new List<Type>() { typeof(Movie) };
            return CreateHttpResponse(request, _requiredRepositories, () => {
                HttpResponseMessage response = null;
                var movie = _moviesRepository.GetSingle(id);

                MovieViewModel movieVm = Mapper.Map<Movie, MovieViewModel>(movie);
                response = request.CreateResponse(HttpStatusCode.OK, movieVm);
                return response;
            });
        }

        [AllowAnonymous]
        [Route("{page:int=0}/{pageSize=3}/{filter?}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int? page, int? pageSize, string filter=null)
        {
            _requiredRepositories = new List<Type>() { typeof(Movie) };
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, _requiredRepositories, () => {
                HttpResponseMessage response = null;
                List<Movie> movies = null;
                int totalMovies = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    movies = _moviesRepository.GetAll().OrderBy(m => m.Id)
                                .Where(m => m.Title.ToLower()
                                .Contains(filter.ToLower().Trim())).ToList();
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
                    TotalPages = (int)Math.Ceiling((decimal)totalMovies / currentPageSize),
                    Items = movieVm                
                };
                response = request.CreateResponse(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }

        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Add(HttpRequestMessage request, MovieViewModel movie)
        {
            _requiredRepositories = new List<Type>() { typeof(Movie), typeof(Stock) };
            return CreateHttpResponse(request, _requiredRepositories, () => {

                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    Movie newMovie = new Movie();
                    newMovie.UpdateMovie(movie);
                    for(int i=0; i<=movie.NumberOfStocks; i++)
                    {
                        Stock stocks = new Stock()
                        {
                            isAvailble = true,
                            Movie = newMovie,
                            UniqueKey = Guid.NewGuid()
                        };
                        newMovie.Stocks.Add(stocks);
                    }
                    _moviesRepository.Add(newMovie);
                    _unitOfWork.Commit();

                    movie = Mapper.Map<Movie, MovieViewModel>(newMovie);

                    response = request.CreateResponse(HttpStatusCode.Created, movie);
                    
                }

                return response;
            });
        }

        [HttpPost]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, MovieViewModel movieVm)
        {
            HttpResponseMessage response = null;
            if (!ModelState.IsValid)
            {
                response = request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                var movieDb = _moviesRepository.GetSingle(movieVm.ID);
                if (movieDb == null)
                {
                    response = request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid Movie");
                }
                else
                {
                    movieDb.UpdateMovie(movieVm);
                    movieVm.Image = movieDb.Image;
                    _moviesRepository.Edit(movieDb);
                    _unitOfWork.Commit();

                    response = request.CreateResponse(HttpStatusCode.OK, movieVm);
                }
            }
            return response;
        }

        [MimeMultipart]
        [Route("images/upload")]
        public async Task<HttpResponseMessage> PostAsync(HttpRequestMessage request, int movieId)
        {
            HttpResponseMessage response = null;

            var movieOld = _moviesRepository.GetSingle(movieId);
            if (movieOld == null)
            {
                response = request.CreateResponse(HttpStatusCode.NotFound, "Movie Not Found");
            }
            else
            {
                var uploadPath = HttpContext.Current.Server.MapPath("~/Content/images/movies");
                var multiPartFormDataStreamProvider = new UploadMultipartFormProvider(uploadPath);
                await Request.Content.ReadAsMultipartAsync(multiPartFormDataStreamProvider);

                string _localFileName = multiPartFormDataStreamProvider
                    .FileData.Select(multipartData => multipartData.LocalFileName).FirstOrDefault();

                FileUploadResult fileUploadResult = new FileUploadResult()
                {
                    LocalFilePath = _localFileName,
                    FileName = Path.GetFileName(_localFileName),
                    FileLength = new FileInfo(_localFileName).Length
                };

                movieOld.Image = fileUploadResult.FileName;
                _moviesRepository.Edit(movieOld);
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.OK, fileUploadResult);

            }
            return response;
        }
    }
}