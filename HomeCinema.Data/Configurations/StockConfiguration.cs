using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeCinema.Entities;

namespace HomeCinema.Data.Configurations
{
   public class StockConfiguration : EntityBaseConfiguration<Stock>
    {
        public StockConfiguration()
        {
            Property(s => s.MovieId).IsRequired();
            Property(s => s.isAvailble).IsRequired();
            Property(s => s.UniqueKey).IsRequired();
            HasMany(s => s.Rentals).WithRequired(r=>r.Stock).HasForeignKey(r => r.StockId);
        }
    }
}
