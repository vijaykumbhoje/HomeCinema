using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;

namespace HomeCinema.Data.Extensions
{
   public static class UserExtensions
    {
        public static User GetSingleByUsername(this IEntityBaseRepository<User> userRepository, string username)
        {
            return userRepository.GetAll().FirstOrDefault(u => u.UserName == username);
        }
    }
}
