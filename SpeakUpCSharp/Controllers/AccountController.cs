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
            _logger.LogInformation("GetUsername called");
            var user = await _userManager.GetUserAsync(User);
			_logger.LogInformation("User is: " + user.UserName);
			var username = user.UserName;

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
