﻿using System;
using System.Collections.Generic;
using System.Text;


namespace HomeCinema.Entities
{
  public class Movie : IEntityBase
    {
        public Movie()
        {
            Stocks = new List<Stock>();          
        }
        public int Id { get; set; }        

        public string Title { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public string Writer { get; set; }
        public string Director { get; set; }

        public string Producer { get; set; }

        public virtual Genre Genre {get;set;}
                  
        public int GenreId { get; set; }

        public DateTime  ReleaseDate { get; set; }

        public byte Rating { get; set; }

        public string TrailerUrl { get; set; }

        public virtual ICollection<Stock> Stocks { get; set; }

    }
}
