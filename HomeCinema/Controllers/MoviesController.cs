using AutoMapper;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Infrastructure.Core;
using HomeCinema.Infrastructure.Extensions;
using HomeCinema.Models;
using HomeCinema.Services.Auth;
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

    [Authorize]
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
                response = request.CreateResponse<IEnumerable<MovieViewModel>>(HttpStatusCode.OK, moviesVM);
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
                    Items = movieVm
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
        [jwtAuthentication]
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
                    response = request.CreateResponse(HttpStatusCode.Created, movieVm);
                }
                return response;
            });
        }

        [HttpPost]
        [Route("update")]
        public HttpResponseMessage  Update(HttpRequestMessage request, MovieViewModel movieVm)
        {
            HttpResponseMessage response = null;
            if(!ModelState.IsValid)
            {
                response = request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                var movieDb = _moviesRepository.GetSingle(movieVm.ID);
                if(movieDb==null)
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
            if(movieOld==null)
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