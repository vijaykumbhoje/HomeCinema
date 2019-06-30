using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeCinema.Entities;
using HomeCinema.Models;

namespace HomeCinema.Mappings
{
    public class DomainToViewModelProfile : Profile
    {

        public DomainToViewModelProfile()
        {
            CreateMap<Movie, MovieViewModel>();
        }
        public override string ProfileName
        {
            get { return "DomainToViewModelMappings"; }
        }

        protected void Configure()

        {
            CreateMap<Movie, MovieViewModel>()
                    .ForMember(vm => vm.Genre, map => map.MapFrom(m => m.Genre.Name))
                    .ForMember(vm => vm.GenreId, map => map.MapFrom(m => m.Genre.Id))
                    .ForMember(vm => vm.IsAvailable, map => map.MapFrom(m => m.Stocks.Any(s => s.isAvailble)))
                    .ForMember(vm => vm.NumberOfStocks, map => map.MapFrom(m => m.Stocks.Count))
                    .ForMember(vm => vm.Image, map => map.MapFrom(m => string.IsNullOrEmpty(m.Image) == true ? "unknown.jpg" : m.Image));

            CreateMap<Genre, GenreViewModel>()
                .ForMember(vm => vm.NumberOfMovies, map => map.MapFrom(g => g.Movies.Count()));
            
        }
    }
}