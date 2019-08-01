using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using HomeCinema.Infrastructure.Extensions;
using System.Security.Principal;
using System.Net;
using HomeCinema.Services.Auth;
using System.Security.Claims;

namespace HomeCinema.Infrastructure.MessageHandlers
{
    public class HomeCinemaAuthHandler :DelegatingHandler
    {
        IEnumerable<string> authHeaderValues = null;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                request.Headers.TryGetValues("Authorization", out authHeaderValues);
                if(authHeaderValues==null)
                   return base.SendAsync(request, cancellationToken);

                var tokens = authHeaderValues.FirstOrDefault();
                tokens = tokens.Replace("Bearer ", "").Trim();
                if (!string.IsNullOrEmpty(tokens))
                {
                    var membershipCtx = jwtAuthManager.GetPrincipal(tokens);

                    if (membershipCtx.Claims != null)
                    {

                        IPrincipal principle = membershipCtx;
                        Thread.CurrentPrincipal = principle;                       

                        HttpContext.Current.User = principle;
                    }
                    else //for unauthorized access
                    {
                        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        var tsc = new TaskCompletionSource<HttpResponseMessage>();
                        tsc.SetResult(response);
                        return tsc.Task;
                    }
                }
                else
                {
                    var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                    var tsc = new TaskCompletionSource<HttpResponseMessage>();
                    tsc.SetResult(response);
                    return tsc.Task;
                }
                return base.SendAsync(request, cancellationToken);
            }catch
            {
                var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(response);
                return tsc.Task;
            }
        }
    }
}