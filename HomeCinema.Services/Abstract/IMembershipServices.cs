using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeCinema.Entities;
using HomeCinema.Data;
using HomeCinema.Services.Utilities;

namespace HomeCinema.Services.Abstract
{
    public interface IMembershipServices
    {
        MembershipContext ValidateUser(string username, string password);

        User CreateUser(string username, string email, string password, int[] roles);

        User GetUser(int userId);

        List<Role> GetUserRoles(string Username);

        bool CheckUser(string username, string password);
    }
}
