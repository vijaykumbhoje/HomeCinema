using System;
using System.Collections.Generic;
using System.Text;

namespace HomeCinema.Entities
{
    public class Stock : IEntityBase
    {
        public Stock()
        {
            Rentals = new List<Rental>();
        }
        public int Id { get; set; }

        public int MovieId { get; set; }

        public virtual Movie Movie { get; set; }

        public Guid UniqueKey { get; set; }

        public bool isAvailble { get; set; }

        public virtual ICollection<Rental> Rentals { get; set; }
    }
}
