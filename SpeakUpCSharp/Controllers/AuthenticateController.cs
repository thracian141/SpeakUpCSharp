using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpeakUp.Models;
using SpeakUp.Services;
using SpeakUpCSharp.Data;
using SpeakUpCSharp.Models;
using SpeakUpCSharp.Models.InputModels;

namespace SpeakUpCSharp.Controllers {
    [Route("authenticate")]
    [ApiController]
    public class AuthenticateController : ControllerBase {
        private readonly ApplicationDbContext _db;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthenticateController> _logger;
        private readonly ITokenService _tokenService;

        public AuthenticateController(SignInManager<ApplicationUser> signInManager,UserManager<ApplicationUser> userManager,ILogger<AuthenticateController> logger
        ,ITokenService tokenService, ApplicationDbContext db) {
            _tokenService=tokenService;
            _signInManager=signInManager;
            _userManager=userManager;
            _logger=logger;
            _db = db;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterInputModel model) {
            var user = new ApplicationUser {
                Id=0,
                UserName=model.UserName,
                Email=model.Email,
                DisplayName=model.DisplayName,
                ProfilePictureUrl=null,
                AccountCreatedDate=DateTime.UtcNow,
                LastDeck=null
            };
            var result = await _userManager.CreateAsync(user,model.Password);

            if (!result.Succeeded) {
                return BadRequest(result.Errors);
            }
            await _db.SaveChangesAsync();
            await _userManager.AddToRoleAsync(user,"User");

            var token = await _tokenService.GenerateToken(user);
            var cookieOptions = new CookieOptions {
                HttpOnly=true,
                Expires=DateTime.UtcNow.AddDays(1)
            };
            Response.Cookies.Append("token",token,cookieOptions);

            return new JsonResult(new { token });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginInputModel model) {

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user==null||!await _userManager.CheckPasswordAsync(user,model.Password)) {
                return Unauthorized();
            }

            var token = await _tokenService.GenerateToken(user);

            var cookieOptions = new CookieOptions {
                HttpOnly=true,
                Expires=DateTime.UtcNow.AddDays(1)
            };
            Response.Cookies.Append("token",token,cookieOptions);

            return new JsonResult(new { token });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            Response.Cookies.Delete("token");
            _logger.LogInformation("User logged out.");

            return Ok();
        }
    }
}
