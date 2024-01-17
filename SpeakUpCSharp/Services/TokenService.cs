using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SpeakUpCSharp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SpeakUp.Services {
    public class TokenService : ITokenService {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(IConfiguration configuration,UserManager<ApplicationUser> userManager) {
            _configuration=configuration;
            _userManager=userManager;
        }

        public async Task<string> GenerateToken(ApplicationUser user) {
            var claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Email", user.Email),
                new Claim("UserName", user.UserName),
                new Claim("DisplayName", user.DisplayName),
                new Claim("ProfilePictureUrl", user.ProfilePictureUrl),
                new Claim("AccountCreatedDate", user.AccountCreatedDate.ToString()),
                new Claim("LastDeck", user.LastDeck.ToString())
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role,role)));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
