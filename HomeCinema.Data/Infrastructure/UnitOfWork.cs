using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;

namespace HomeCinema.Data.Infrastructure
{
   public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbFactory dbFactory;
        private HomeCinemaContext dbContext;
        private EntityBaseRepository<User> _userRepository;
        private EntityBaseRepository<Customer> _customerRepository;


        public UnitOfWork(IDbFactory dbFactory)
        {
            this.dbFactory = dbFactory;
           
        }

        public HomeCinemaContext DbContext
        {   
            get{ return dbContext ?? (dbContext = dbFactory.Init()); }
        }

        public EntityBaseRepository<User> userRepository
        {
            get
            {
                if(this._userRepository==null)
                       this._userRepository = new EntityBaseRepository<User>(dbFactory);
                return _userRepository;
                
            }
        }

        public EntityBaseRepository<Customer> customerRepository
        {
            get
            {
                if (this._customerRepository == null)
                    this._customerRepository = new EntityBaseRepository<Customer>(dbFactory);
                return _customerRepository;

            }
        }
        public void Commit()
        {
            DbContext.Commit(); 
        }


    }
}
