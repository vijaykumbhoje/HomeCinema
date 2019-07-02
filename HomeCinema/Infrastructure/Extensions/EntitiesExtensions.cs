using HomeCinema.Entities;
using HomeCinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeCinema.Infrastructure.Extensions
{
    public static class EntitiesExtensions
    {
        public static void UpdateCustomer(this Customer customer, CustomerViewModel customerVM)
        {
            customer.FirstName = customerVM.FirstName;
            customer.LastName = customerVM.LastName;
            customer.IdentityCard = customerVM.IdentityCard;
            customer.Mobile = customerVM.Mobile;
            customer.Email = customerVM.Email;
            customer.DateofBirth = customerVM.DateOfBirth;
            customer.UniqueKey = (customerVM.UniqueKey == null || customerVM.UniqueKey == Guid.Empty)
                ? Guid.NewGuid() : customerVM.UniqueKey;
            customer.RegistrationDate = customerVM.RegistrationDate;
        }

        public static void UpdateMovie(this Movie movie, MovieViewModel movieVm)
        {
            movie.Title = movieVm.Title;
            movie.Director = movieVm.Director;
            movie.Writer = movieVm.Writer;
            movie.Description = movieVm.Description;
            movie.Rating = movieVm.Rating;
            movie.ReleaseDate = movieVm.ReleaseDate;
            movie.Producer = movieVm.Producer;
            movie.GenreId = movieVm.GenreId;
            movie.TrailerUrl = movieVm.TrailerURI;
        }
    }
}