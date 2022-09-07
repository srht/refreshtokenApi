using Microsoft.IdentityModel.Tokens;
using refreshtokenApi.Models;
using refreshtokenApi.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace refreshtokenApi.Helpers
{
    public static class TokenHelper
    {
        public static TokenResponseViewModel GenerateToken(AppUser appUser, string role, IConfiguration _config)
        {
            DateTime expireDate = DateTime.Now.AddSeconds(50);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Application:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _config["Application:Audience"],
                Issuer = _config["Application:Issuer"],
                Subject = new ClaimsIdentity(new Claim[]
                {
                    //Claim tanımları yapılır. Burada en önemlisi Id ve emaildir.
                    //Id üzerinden, aktif kullanıcıyı buluyor olacağız.
                    new Claim(ClaimTypes.NameIdentifier, appUser.Id),
                    new Claim(ClaimTypes.Name, appUser.Name),
                    new Claim(ClaimTypes.Email, appUser.Email),
                    new Claim(ClaimTypes.Role, role)
                }),

                //ExpireDate
                Expires = expireDate,

                //Şifreleme türünü belirtiyoruz: HmacSha256Signature
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var tokenInfo = new TokenResponseViewModel();

            tokenInfo.AccessToken = tokenString;
            tokenInfo.AccessTokenExpireDate = expireDate;

            return tokenInfo;
        }

        public static string CreateRefreshToken()

        {
            var numberByte = new Byte[32];

            using var rnd = RandomNumberGenerator.Create();

            rnd.GetBytes(numberByte);

            return Convert.ToBase64String(numberByte);
        }

        public static string ipAddress(HttpRequest httpRequest, HttpContext httpContext)
        {
            // get source ip address for the current request
            if (httpRequest.Headers.ContainsKey("X-Forwarded-For"))
                return httpRequest.Headers["X-Forwarded-For"];
            else
                return httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
