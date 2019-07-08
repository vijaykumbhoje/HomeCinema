using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using System.Data.Entity.Infrastructure;
using System.Net;

namespace HomeCinema.Infrastructure.Core
{
    public class ApiControllerBaseExtended :ApiController
    {
        protected List<Type> _requiredRepositories;
        protected readonly IDataRepositoryFactory _dataRepositoryFactory;
        protected IEntityBaseRepository<Movie> _moviesRepository;
        protected IEntityBaseRepository<Rental> _rentalsRepository;
        protected IEntityBaseRepository<Stock> _stocksRepository;
        protected IEntityBaseRepository<Customer> _customersRepository;
        protected IEntityBaseRepository<Error> _errorsRepository;
        protected IUnitOfWork _unitOfWork;

        private HttpRequestMessage RequestMessage;

        public ApiControllerBaseExtended(IDataRepositoryFactory dataRepositoryFactory, IUnitOfWork unitOfWork)
        {
            _dataRepositoryFactory = dataRepositoryFactory;
            _unitOfWork = unitOfWork;
        }

        protected HttpResponseMessage CreateHttpResponse(HttpRequestMessage request, List<Type> repo, Func<HttpResponseMessage> function)
        {
            HttpResponseMessage response = null;
            try
            {
                RequestMessage = request;
                InitiateRepositories(repo);
                response = function.Invoke();
            }
            catch(DbUpdateException ex)
            {
                logError(ex);
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.InnerException.Message);
            }catch(Exception ex)
            {
                logError(ex);
                response = request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }

        private void InitiateRepositories(List<Type> entities)
        {
            _errorsRepository = _dataRepositoryFactory.GetDataRepository<Error>(RequestMessage);

            if (entities.Any(e => e.FullName == typeof(Movie).FullName))
            {
                _moviesRepository = _dataRepositoryFactory.GetDataRepository<Movie>(RequestMessage);
            }

            if (entities.Any(e => e.FullName == typeof(Rental).FullName))
            {
                _rentalsRepository = _dataRepositoryFactory.GetDataRepository<Rental>(RequestMessage);
            }

            if (entities.Any(e => e.FullName == typeof(Customer).FullName))
            {
                _customersRepository = _dataRepositoryFactory.GetDataRepository<Customer>(RequestMessage);
            }

            if (entities.Any(e => e.FullName == typeof(Stock).FullName))
            {
                _stocksRepository = _dataRepositoryFactory.GetDataRepository<Stock>(RequestMessage);
            }

            if (entities.Any(e => e.FullName == typeof(User).FullName))
            {
                _stocksRepository = _dataRepositoryFactory.GetDataRepository<Stock>(RequestMessage);
            }
        }

        private void logError(Exception ex)
        {
            try
            {
                Error error = new Error()
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    DateCreated = DateTime.Now
                };
                _errorsRepository.Add(error);
                _unitOfWork.Commit();

            }
            catch
            { }
            
        }
    }
}