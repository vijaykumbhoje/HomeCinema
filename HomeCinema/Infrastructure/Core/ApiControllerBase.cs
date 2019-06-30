using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using HomeCinema.Data.Repositories;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Entities;
using System.Net.Http;
using System.Data.Entity.Infrastructure;
using System.Net;

namespace HomeCinema.Infrastructure.Core
{
    public class ApiControllerBase :ApiController
    {
        protected readonly IEntityBaseRepository<Error> _errorsRepository;
        protected readonly IUnitOfWork _unitOfWork;

        public ApiControllerBase(IEntityBaseRepository<Error> entityBaseRepository, IUnitOfWork unitOfWork)
        {
            _errorsRepository = entityBaseRepository;
            _unitOfWork = unitOfWork;
        }

        protected HttpResponseMessage CreateHttpResponse(HttpRequestMessage request, Func<HttpResponseMessage> function)
        {
            HttpResponseMessage response = null;
            try
            {
                response = function.Invoke();
            }catch(DbUpdateException ex)
            {
                LogError(ex);
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.InnerException.Message);
            }catch(Exception ex)
            {
                LogError(ex);
                response = request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            return response;
        }

        private void LogError(Exception ex)
        {
            try
            {
                Error _error = new Error()
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    DateCreated = DateTime.Now
                };

                _errorsRepository.Add(_error);
                _unitOfWork.Commit();

            }catch
            {

            }
        }

    }
}