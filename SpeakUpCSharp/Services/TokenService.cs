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
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration,UserManager<ApplicationUser> userManager, ILogger<TokenService> logger) {
            _configuration=configuration;
            _userManager=userManager;
            _logger=logger;
        }

        public async Task<string> GenerateToken(ApplicationUser user) {
            _logger.LogInformation("CALLED GENERATE TOKEN");
            var claims = new List<Claim> {
					new Claim(ClaimTypes.Name, user.UserName),
	                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
	                new Claim(ClaimTypes.Email, user.Email),
			};
            foreach (var x in claims) {
                _logger.LogInformation(x.ToString());
            }
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
