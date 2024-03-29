﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using HomeCinema.Entities;
using HomeCinema.Data.Configurations;

namespace HomeCinema.Data
{
    public class HomeCinemaContext : DbContext
    {
        public HomeCinemaContext() : base ("DefaultConnection")
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

        public virtual void Commit()
        {
            base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new CustomerConfiguration());
            modelBuilder.Configurations.Add(new MovieConfiguration());
            modelBuilder.Configurations.Add(new GenreConfiguration());
            modelBuilder.Configurations.Add(new StockConfiguration());
            modelBuilder.Configurations.Add(new RentalConfiguration());
            modelBuilder.Configurations.Add(new RoleConfiguration());
            modelBuilder.Configurations.Add(new UserRoleConfiguration());
            modelBuilder.Configurations.Add(new UserConfiguration());   
        }
    }
}
