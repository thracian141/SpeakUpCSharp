﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpeakUp.Models;
using SpeakUpCSharp.Data;
using SpeakUpCSharp.Models;
using SpeakUpCSharp.Models.InputModels;
using SpeakUpCSharp.Services;

namespace SpeakUpCSharp.Controllers {
	[ApiController]
	[Route("account")]
	public class AccountController : ControllerBase {
        private readonly UserManager<ApplicationUser> _userManager;
		private IImageService _img;
		private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _db;

        public AccountController(UserManager<ApplicationUser> userManager,
            ILogger<AccountController> logger, IImageService img, ApplicationDbContext db) {
            _userManager = userManager;
            _logger = logger;
            _img = img;
            _db = db;
        }

        [HttpGet("getname")]
        public async Task<IActionResult> GetName() {
            var user = await _userManager.GetUserAsync(User);
            string name;

            if (user.DisplayName != null)
                name = user.DisplayName;
            else
                name = user.UserName;

            return new JsonResult(new { name });
        }

        [HttpGet("getusername")]
        public async Task<IActionResult> GetUsername() {
            var user = await _userManager.GetUserAsync(User);
			var username = user.UserName;

            return new JsonResult(new { username });
        }
        [HttpGet("getdisplayname")]
        public async Task<IActionResult> GetDisplayName() {
            var user = await _userManager.GetUserAsync(User);
            var displayname = user.DisplayName;

            return new JsonResult(new { displayname });
        }
        [HttpGet("checkifadmin")]
        public async Task<IActionResult> CheckIfAdmin() {
            var user = await _userManager.GetUserAsync(User);
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
                return Ok();
            else
                return Unauthorized();
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

        [HttpPost("editusername")]
        public async Task<IActionResult> EditUsername([FromBody] EditUserInputModel model) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return BadRequest();

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, model.currentPassword);
            if (isPasswordCorrect == false)
                return Unauthorized();

            user.UserName = model.newInput;
            user.NormalizedUserName = model.newInput.Normalize();
            await _db.SaveChangesAsync();

            return Ok(user.UserName);
        }

		[HttpPost("editemail")]
		public async Task<IActionResult> EditEmail([FromBody] EditUserInputModel model) {
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return BadRequest();

			var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, model.currentPassword);
			if (isPasswordCorrect == false)
				return Unauthorized();

			user.Email = model.newInput;
            user.NormalizedEmail = model.newInput.Normalize();
			await _db.SaveChangesAsync();

			return Ok(user.Email);
		}

		[HttpPost("editdisplayname")]
		public async Task<IActionResult> EditDisplayName([FromBody] EditUserInputModel model) {
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return BadRequest();

			var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, model.currentPassword);
			if (isPasswordCorrect == false)
				return Unauthorized();

			user.DisplayName = model.newInput;
			await _db.SaveChangesAsync();

			return Ok(user.DisplayName);
		}

		[HttpPost("editpassword")]
		public async Task<IActionResult> EditPassword([FromBody] EditUserInputModel model) {
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return BadRequest();

			var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, model.currentPassword);
			if (isPasswordCorrect == false)
				return Unauthorized();

            await _userManager.ChangePasswordAsync(user, model.currentPassword, model.newInput);

            return Ok();
		}
	}
}
