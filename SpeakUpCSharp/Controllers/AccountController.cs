using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpeakUp.Models;
using SpeakUpCSharp.Models;
using SpeakUpCSharp.Services;

namespace SpeakUpCSharp.Controllers {
	[ApiController]
	[Route("account")]
	public class AccountController : ControllerBase {
        private readonly UserManager<ApplicationUser> _userManager;
		private IImageService _img;
		private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager,
            ILogger<AccountController> logger, IImageService img) {
            _userManager = userManager;
            _logger = logger;
            _img = img;
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
        [HttpPost("setpfp")]
        public async Task<IActionResult> SetPfp([FromBody] IFormFile picture) {
            var user = await _userManager.GetUserAsync(User);

			if (user == null) return BadRequest("User not found");

            // saves "picture" to wwwroot/ProfilePictures and returns the generated url
            string pictureUrl = await _img.SaveProfilePictureReturnUrl(picture);
            user.ProfilePictureUrl = pictureUrl;
            await _userManager.UpdateAsync(user);

            return Ok();
		}

        [HttpGet("getpfp")]
        public async Task<IActionResult> GetPfp() {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) return BadRequest("User not found");

			var profilePicture = await _img.GetProfilePictureByUserId(user.Id);

			return File(profilePicture,"image/*");
		}
    }
}
