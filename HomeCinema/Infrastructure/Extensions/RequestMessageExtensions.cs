using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Services;
using HomeCinema.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Dependencies;


namespace HomeCinema.Infrastructure.Extensions
{
    public static class RequestMessageExtensions
    {
        internal static IMembershipServices GetMembershipService(this HttpRequestMessage request)
        {
            return request.GetService<IMembershipServices>();
        }

        private static TService GetService<TService>(this HttpRequestMessage request)
        {
            IDependencyScope dependencyScope = request.GetDependencyScope();
            TService service = (TService)dependencyScope.GetService(typeof(TService));
            return service;
        }
    }
}