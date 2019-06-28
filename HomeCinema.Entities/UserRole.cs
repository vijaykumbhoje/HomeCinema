using System;
using System.Collections.Generic;
using System.Text;

namespace HomeCinema.Entities
{
    public class UserRole :IEntityBase
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role{ get; set; }

    }
}
