using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCinema.Data.Extensions
{
    public static class CustomerExtension 
    {
        public static string GetCustomerFullName(this IEntityBaseRepository<Customer> customerRepository, int customerId)
        {
            var customer = customerRepository.GetSingle(customerId);
            return customer.FirstName + " " + customer.LastName;
        }
    }
}
