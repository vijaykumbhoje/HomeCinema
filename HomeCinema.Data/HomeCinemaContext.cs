using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using HomeCinema.Entities;

namespace HomeCinema.Data
{
    public class HomeCinemaContext : DbContext
    {
        public HomeCinemaContext() : base ("HomeCinema")
        {
            Database.SetInitializer<HomeCinemaContext>(null);
        }

        #region 'Entity Sets'
        public IDbSet<Customer> CustomerSet { get; set; }       

        public IDbSet<Rental> RentalSet { get; set; }    
        
        public IDbSet<Stock> StockSet { get; set; }

        public IDbSet<Movie> MovieSet { get; set; }

        public IDbSet<Genre> GenreSet { get; set; }

        public IDbSet<User> UserSet { get; set; }

        public IDbSet<Role> RoleSet { get; set; }

        public IDbSet<UserRole> UserRoleSet { get; set; }
       
        public IDbSet<Error> ErrorSet { get; set; }

        #endregion
    }
}
