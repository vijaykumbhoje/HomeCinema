using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCinema.Data.Extensions
{
    public static class CustomerExtensions 
    {
        public static bool UserExists(this IEntityBaseRepository<Customer> customerRepository, string email, string identityCard)
        {
            bool _userExists = false;
            _userExists = customerRepository.GetAll().Any(c => c.Email.ToLower() == email || c.IdentityCard.ToLower() == identityCard);
            return _userExists;
        }
        public static string GetCustomerFullName(this IEntityBaseRepository<Customer> customerRepository, int customerId)
        {
            var customer = customerRepository.GetSingle(customerId);
            return customer.FirstName + " " + customer.LastName;
        }
    }
}
