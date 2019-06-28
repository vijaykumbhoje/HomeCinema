using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeCinema.Entities;
namespace HomeCinema.Data.Configurations
{
   public class MovieConfiguration : EntityBaseConfiguration<Movie>
    {
        public MovieConfiguration()
        {
            Property(m => m.Title).IsRequired().HasMaxLength(100);
            Property(m => m.GenreId).IsRequired();
            Property(m => m.Producer).IsRequired().HasMaxLength(50);
            Property(m => m.Writer).IsRequired().HasMaxLength(50);
            Property(m => m.Director).IsRequired().HasMaxLength(50);
            Property(m => m.Description).IsRequired().HasMaxLength(2000);
            Property(m => m.Rating).IsRequired();
            Property(m => m.TrailerUrl).IsRequired().HasMaxLength(255);
            HasMany(m => m.Stocks).WithRequired().HasForeignKey(s => s.MovieId);


        }
    }
}
