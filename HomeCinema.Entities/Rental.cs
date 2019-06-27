using System;
using System.Collections.Generic;
using System.Text;

namespace HomeCinema.Entities
{
   public class Rental:IEntityBase
    {
        public Rental()
        {

        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int StockId { get; set; }

        public DateTime RentalDate { get; set; }

        public DateTime ReturnDate { get; set; }

        public byte Status { get; set; }

    }
}
