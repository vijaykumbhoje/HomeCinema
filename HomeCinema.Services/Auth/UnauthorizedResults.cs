using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HomeCinema.Services.Auth
{
    public class UnauthorizedResults : IHttpActionResult
    {
        public UnauthorizedResults(AuthenticationHeaderValue authHeaderValue, IHttpActionResult innerResult)
        {
            AuthHeaderValue = authHeaderValue;
            InnerResult = innerResult;
        }
        
        public AuthenticationHeaderValue AuthHeaderValue { get; }
        public IHttpActionResult InnerResult { get; }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await InnerResult.ExecuteAsync(cancellationToken);
            if(response.StatusCode==System.Net.HttpStatusCode.Unauthorized)
            {
                if(response.Headers.WwwAuthenticate.All(h=>h.Scheme != AuthHeaderValue.Scheme))
                {
                    response.Headers.WwwAuthenticate.Add(AuthHeaderValue);
                }
            }
            return response;
        }
    }
}