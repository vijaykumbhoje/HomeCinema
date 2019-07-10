using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HomeCinema.Entities;
using HomeCinema.Infrastructure.Core;
using HomeCinema.Data.Repositories;
using HomeCinema.Data.Infrastructure;
using AutoMapper;
using HomeCinema.Models;
using HomeCinema.Infrastructure.Extensions;
using HomeCinema.Data.Extensions;

namespace HomeCinema.Controllers 
{
    [Authorize(Roles = "admin")]
    [RoutePrefix("api/customers")]   
    public class CustomerController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Customer> _customerRepositories;
        public CustomerController(IEntityBaseRepository<Customer> customerRepository, IEntityBaseRepository<Error> _errorRepository, IUnitOfWork _unitOfWork)
            : base (_errorRepository, _unitOfWork)
        {
            _customerRepositories = customerRepository;
        }

        [Route("{filter?}")]
        public HttpResponseMessage Get(HttpRequestMessage request, string filter)
        {
            filter = filter.ToLower().Trim();

            return CreateHttpResponse(request, () => {
                HttpResponseMessage response = null;
                var customer = _customerRepositories.GetAll()
                 .Where(c => c.Email.ToLower().Contains(filter) || 
                 c.FirstName.ToLower().Contains(filter) || 
                 c.LastName.ToLower().Contains(filter)).ToList();
                var customerVm = Mapper.Map<IEnumerable<Customer>, IEnumerable<CustomerViewModel>>(customer);
                response = request.CreateResponse(HttpStatusCode.OK, customerVm);
                return response;             
            }); 
        }

        [Route("details/{id:int}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () => {
                HttpResponseMessage response = null;
                var customer = _customerRepositories.GetSingle(id);
                var customerVm = Mapper.Map<Customer, CustomerViewModel>(customer);
                response = request.CreateResponse(HttpStatusCode.OK, customerVm);

                return response;
            });
        }

        [HttpPost]
        [Route("register")]
        public HttpResponseMessage Register(HttpRequestMessage request, CustomerViewModel customerVm)
        {
            return CreateHttpResponse(request, () => {

                HttpResponseMessage response = null;
                if(!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                        .Select(m => m.ErrorMessage).ToArray());
                }else
                {
                    if(_customerRepositories.UserExists(customerVm.Email, customerVm.IdentityCard))
                    {
                        ModelState.AddModelError("Invalid User", "User already exists");
                        response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState.Keys
                            .SelectMany(k => ModelState[k].Errors)
                            .Select(e => e.ErrorMessage).ToArray());
                    }
                    else
                    {
                        Customer newCustomer = new Customer();
                        customerVm.RegistrationDate = DateTime.Now;
                        newCustomer.UpdateCustomer(customerVm);
                        _customerRepositories.Add(newCustomer);
                        _unitOfWork.Commit();

                        customerVm = Mapper.Map<Customer, CustomerViewModel>(newCustomer);
                        response = request.CreateResponse(HttpStatusCode.Created, customerVm);
                    }
                }

                return response;
            });
        }



        [HttpGet]
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
                    Items = customerVM                
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