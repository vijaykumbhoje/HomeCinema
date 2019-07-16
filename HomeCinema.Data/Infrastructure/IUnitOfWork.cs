using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;

namespace HomeCinema.Data.Infrastructure
{
   public interface IUnitOfWork
    {
       EntityBaseRepository<User> userRepository { get; }
        EntityBaseRepository<Customer> customerRepository { get; }

        void Commit();
    }
}
