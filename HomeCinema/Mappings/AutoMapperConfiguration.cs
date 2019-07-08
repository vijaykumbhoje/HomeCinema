using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;

namespace HomeCinema.Mappings
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            Mapper.Initialize(x => {
                x.AddProfile<DomainToViewModelMappingProfile>();
            });
#pragma warning restore CS0618 // Type or member is obsolete
        }

    }
}