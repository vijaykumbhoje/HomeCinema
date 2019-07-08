using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace HomeCinema.Infrastructure.Core
{
    public class DataRepositoryFactory : IDataRepositoryFactory
    {
        public IEntityBaseRepository<T> GetDataRepository<T>(HttpRequestMessage request) where T : class, IEntityBase, new()
        {
            return request.GetDataRepository<T>();
        }
    }

    public interface IDataRepositoryFactory
    {
        IEntityBaseRepository<T> GetDataRepository<T>(HttpRequestMessage request) where T : class, IEntityBase, new();
    }
}