using System;
using System.Collections.Generic;
using System.Text;

namespace HomeCinema.Entities
{
  public class Customer :IEntityBase
    {

        public Customer()
        {

        }
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string IdentityCard{ get; set; }

        public DateTime DateofBirth { get; set; }

        public Guid UniqueKey { get; set; }

        public string Mobile { get; set; }
        public string Email { get; set; }

        public DateTime RegistrationDate { get; set; }


    }
    
}
