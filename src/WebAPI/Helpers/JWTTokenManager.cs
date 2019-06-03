using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace GenesisTest.WebAPI.Helpers
{
    public class JWTTokenManager:ITokenManager
    {
        private readonly AppSettings appSettings;
        public JWTTokenManager(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        /// <summary>
        /// Generate JWT Token with User Id as claim
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public string GenerateTokenForUser(string Id)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                      new Claim(ClaimTypes.Name, Id)
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
