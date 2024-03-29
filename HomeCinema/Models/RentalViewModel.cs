﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeCinema.Models
{
    public class RentalViewModel
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int StockId { get; set; }

        public DateTime RentalDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public string Status { get; set; }
    }
}