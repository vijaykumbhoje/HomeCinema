using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using HomeCinema.Entities;

namespace HomeCinema.Services.Utilities
{
    public class MembershipContext
    {
        public IPrincipal Principle { get; set; }

        public User User { get; set; }

        public bool IsValid()
        {
            return Principle != null;
        }
    }
}
