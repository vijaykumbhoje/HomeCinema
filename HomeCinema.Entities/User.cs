using System;
using System.Collections.Generic;
using System.Text;

namespace HomeCinema.Entities
{
   public class User : IEntityBase
    {
        public User()
        {
            UserRoles = new List<UserRoles>();
        }
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
        public bool IsLocked { get; set; }
        public DateTime DateCreated { get; set; }
        public virtual ICollection<UserRoles> UserRoles { get; set; }
    }
}
