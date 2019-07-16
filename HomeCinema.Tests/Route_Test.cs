using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace HomeCinema.Tests
{
    [TestFixture]
    public class Route_Test
    {
        HttpConfiguration _config;

        public void Setup()
        {
            _config = new HttpConfiguration();
            _config.Routes.MapHttpRoute(name: "DefaultWebAPI", routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }

    }
}
