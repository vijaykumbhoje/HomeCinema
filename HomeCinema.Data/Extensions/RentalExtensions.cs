using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCinema.Data.Extensions
{
    public static class RentalExtensions
    {
        public static IEnumerable<Rental> GetStockRentals(this IEntityBaseRepository<Rental> rentalRepository, IEnumerable<Stock> stocks)
        {
            IEnumerable<Rental> _rental = null;
            IEnumerable<int> _stockIds = stocks.Select(s => s.Id).Distinct();
            _rental = rentalRepository.GetAll().Where(r => _stockIds.Contains(r.StockId));
            return _rental;
        }
    }
}
