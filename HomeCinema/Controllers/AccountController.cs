using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Infrastructure.Core;
using HomeCinema.Models;
using HomeCinema.Services;
using HomeCinema.Services.Abstract;
using HomeCinema.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace HomeCinema.Controllers
{
    [Authorize(Roles ="Admin")]
    [RoutePrefix("api/account")]
    public class AccountController : ApiControllerBase
    {
        private readonly IMembershipServices _membershipService;

        public AccountController(IMembershipServices membershipServices, IEntityBaseRepository<Error> _errorRepository, IUnitOfWork _unitOfWork)
            :base (_errorRepository, _unitOfWork)
        {
            _membershipService = membershipServices;
        }

        [AllowAnonymous]
        [Route("authenticate")]
        [HttpPost]
        public HttpResponseMessage Login(HttpRequestMessage request, LoginViewModel user)
        {
            return CreateHttpResponse(request, () => 
            {
                HttpResponseMessage response = null;
                if(ModelState.IsValid)
                {
                    MembershipContext _userContext = _membershipService.ValidateUser(user.Username, user.Password);
                    if(_userContext.User!=null)
                    {
                        response = request.CreateResponse(HttpStatusCode.OK, new { success = true });
                    }
                    else
                    {
                        response = request.CreateResponse(HttpStatusCode.NotFound, new { success = false });
                    }
                }
                else
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, new { success = false });
                }
                return response;
            });
        }
        
        [AllowAnonymous]
        [Route("register")]
        [HttpPost]
        public HttpResponseMessage Register(HttpRequestMessage request, RegistrationViewModel user)
        {
            return CreateHttpResponse(request, () => {
                HttpResponseMessage response = null;
                if(ModelState.IsValid)
                {
                    User _user = _membershipService.CreateUser(user.Username, user.Password, user.Email, new int[] { 1 });
                    if(_user !=null)
                    {
                        response = request.CreateResponse(HttpStatusCode.Created, new { success = true });
                    }else
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, new { success = false });
                    }
                }
                else
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, new { success = false });
                }
                return response;
            });
        }
    }
}