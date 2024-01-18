using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpeakUp.Models;
using SpeakUpCSharp.Models;

namespace SpeakUpCSharp.Controllers {
	[ApiController]
	[Route("account")]
	public class AccountController : ControllerBase {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager, ILogger<AccountController> logger) {
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("getusername")]
        public async Task<IActionResult> GetUsername() {
			var token = HttpContext.Request.Headers["Authorization"]; // Check if the token is present in headers
			_logger.LogInformation("Received token: {0}",token);

			if (User?.Identity?.IsAuthenticated==true) { 
                _logger.LogInformation("User is authenticated");
				foreach (var claim in User.Claims) {
					_logger.LogInformation("Claim Type: {0}, Claim Value: {1}",claim.Type,claim.Value);
				}
			}

			var username = HttpContext.User.Identity.Name.ToString();
			_logger.LogInformation("User is {0}",username);
            if (username== null) {
                return BadRequest("User not found");
            }

            return new JsonResult(new { username });
        }

        [HttpGet("allInfo")]
        public async Task<IActionResult> GetAllInfo() {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) {
                return BadRequest("User not found");
            }

            return new JsonResult(new { user });
        }
    }
}
