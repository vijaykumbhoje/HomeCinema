﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace HomeCinema.Infrastructure.Core
{
    public class MimeMultipart : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
           if(!actionContext.Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(
                    new HttpResponseMessage(System.Net.HttpStatusCode.UnsupportedMediaType));
            }
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
          
        }
    }
}