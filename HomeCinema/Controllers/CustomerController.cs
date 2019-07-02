using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using HomeCinema.Entities;
using HomeCinema.Infrastructure.Core;
using HomeCinema.Data.Repositories;
using HomeCinema.Data.Infrastructure;
using AutoMapper;
using HomeCinema.Models;
using HomeCinema.Infrastructure.Extensions;
using System.Net;

namespace HomeCinema.Controllers 
{
    [Authorize(Roles = "admin")]
    [RoutePrefix("api/customers")]   
    public class CustomerController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Customer> _customerRepositories;
        public CustomerController(IEntityBaseRepository<Customer> cutomerBaseRepository, IEntityBaseRepository<Error> _errorRepository, IUnitOfWork _unitOfWork)
            : base (_errorRepository, _unitOfWork)
        {

        }

        [HttpPost]
        [Route("search/{page:int=0}/{pagesize=4}/{filter?}")]
        public HttpResponseMessage Search(HttpRequestMessage request, int? page, int? pageSize, string filter=null )
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () => {
                HttpResponseMessage response = null;
                List<Customer> customers = null;
                int totalMovies = new int();
                if(!String.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();
                    customers = _customerRepositories.GetAll().OrderBy(c => c.Id)
                    .Where(c => c.LastName.ToLower().Contains(filter) ||
                    c.FirstName.ToLower().Contains(filter) ||
                    c.IdentityCard.ToLower().Contains(filter)).ToList();
                }
                else
                {
                    customers = _customerRepositories.GetAll().ToList();
                }

                totalMovies = customers.Count();
                customers = customers.Skip(currentPage * currentPageSize).Take(currentPageSize).ToList();

                IEnumerable<CustomerViewModel> customerVM = Mapper.Map<IEnumerable<Customer>, IEnumerable<CustomerViewModel>>(customers);

                PaginationSet<CustomerViewModel> pagedSet = new PaginationSet<CustomerViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalMovies,
                    TotalPages = (int)Math.Ceiling((decimal)totalMovies / currentPageSize),
                    items = customerVM                
                };
                response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
                return response;
            });
        }

        [HttpPost]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, CustomerViewModel customer)
        {
            return CreateHttpResponse(request, () => {

                HttpResponseMessage response = null;
                if(ModelState.IsValid)
                {
                    Customer _customer = _customerRepositories.GetSingle(customer.ID);
                    _customer.UpdateCustomer(customer);
                    _unitOfWork.Commit();
                    response = request.CreateResponse(HttpStatusCode.OK);
                }
                return response;
            });
        }



    }
}