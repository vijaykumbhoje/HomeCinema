using System;
using System.Collections.Generic;
using System.Text;

namespace HomeCinema.Entities
{
    public class Role :IEntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
