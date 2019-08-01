using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace HomeCinema.Services.Auth
{
    public static class jwtAuthManager
    {
        //https://code-adda.com/2019/01/jwt-authentication-with-asp-net-web-api/
        public const string secretkey = "B56173EDC8121F7B58712F11A66A815FE9389252999A779231ED697A900B8358";
        
        public static string GenerateJwtToken(string Username, int expiryInMinutes=10)
        {
            var Symmetric_Key = Convert.FromBase64String(secretkey);
            var token_Handler = new JwtSecurityTokenHandler();
            var now = DateTime.Now;
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, Username),
                    new Claim(ClaimTypes.Role, "Admin")
                }),
                Expires = now.AddMinutes(Convert.ToInt32(expiryInMinutes)),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Symmetric_Key), SecurityAlgorithms.HmacSha256Signature)

            };

            var stoken = token_Handler.CreateToken(securityTokenDescriptor);
            var token = token_Handler.WriteToken(stoken);
            return token;
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null) 
                    return null;

                var symmetricKey = Convert.FromBase64String(secretkey);
                var validationParameters = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)

                };

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                return principal;

            }catch(Exception ex)
            {
                return null;
            }
        }

        public static string GenerateSecretKey()
        {
            var hmac = new HMACSHA256();
            var key = Convert.ToBase64String(hmac.Key);
            return key;
        }

    }
}