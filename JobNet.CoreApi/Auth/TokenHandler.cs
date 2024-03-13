using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JobNet.CoreApi.Data.Entities;
using Microsoft.IdentityModel.Tokens;

namespace JobNet.CoreApi.Auth;

public static class TokenHandler
{
    public static Token CreateToken(IConfiguration configuration, User user)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:SecurityKey"]));

            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            DateTime expiration = DateTime.Now.AddMinutes(Convert.ToInt32(configuration["Token:Expiration"]));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            };

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: configuration["Token:Issuer"],
                audience: configuration["Token:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiration,
                signingCredentials: credentials
            );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string accessToken = tokenHandler.WriteToken(jwtSecurityToken);

            byte[] refreshTokenBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(refreshTokenBytes);
            }
            string refreshToken = Convert.ToBase64String(refreshTokenBytes);

            return new Token
            {
                AccessToken = accessToken,
                Expiration = expiration,
                RefreshToken = refreshToken,
                Claims = claims,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                UserId = user.UserId
            };
        }
}