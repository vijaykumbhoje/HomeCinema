using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace HomeCinema.Services.Auth
{
    public class jwtAuthentication : Attribute, IAuthenticationFilter
    {
        public string RealM { get; set; }

        public bool AllowMultiple => false;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = request.Headers.Authorization;
            if(authorization==null || authorization.Scheme!="Bearer" || string.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new AuthFailureResult("JWT Token is Missing", request);
                return;
            }

            var token = authorization.Parameter;
            var principal =await AuthJwtToken(token);

            if(principal==null)
            {
                context.ErrorResult = new AuthFailureResult("Invalid Token", request);
            }
            else
            {
                context.Principal = principal;
            }
        }

      

        private static bool validateToken(string token, out string username)
        {
            username = null;
            var simplePrincipal = jwtAuthManager.GetPrincipal(token);
            if(simplePrincipal==null)
                return false;            

            var identity = simplePrincipal.Identity as ClaimsIdentity;
            if(identity==null)            
                return false;
            
            if(!identity.IsAuthenticated)
                return false;

            var usernameClaim = identity.FindFirst(ClaimTypes.Name);
            username = usernameClaim?.Value;

            if (string.IsNullOrEmpty(username))
                return false;
            return true;            
        }

        protected Task<IPrincipal> AuthJwtToken(string token)
        {
            string username;
            if (validateToken(token, out username))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username)                    
                };
                var identity = new ClaimsIdentity(claims, "Jwt");
                IPrincipal user = new ClaimsPrincipal(identity);
                return Task.FromResult(user);
            }
            return Task.FromResult<IPrincipal>(null);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter = null;
            if(!string.IsNullOrEmpty(RealM))
                parameter="realm=\""+RealM+"\"";

            context.ChallengeWith("Bearer ", parameter);
                
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return Task.FromResult(0);
        }
    }
}