using AutoMapper;
using HomeCinema.Entities;
using HomeCinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeCinema.Mappings
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
           CreateMap<MovieViewModel, Movie>()             
              .ForMember(m => m.Genre, map => map.Ignore());
        }
        public override string ProfileName
        {
            get { return "ViewModelToDomainMappings"; }
        }
    }
}