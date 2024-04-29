using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeakUp.Models;
using SpeakUpCSharp.Data;
using SpeakUpCSharp.Models;
using SpeakUpCSharp.Models.InputModels;
using SpeakUpCSharp.Services;
using SpeakUpCSharp.Utilities;

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
            bool isAdmin = await _userManager.IsInRoleAsync(user, ApplicationRoles.Admin) ||
                await _userManager.IsInRoleAsync(user, ApplicationRoles.SysAdmin);
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

		[Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.SysAdmin}")]
		[HttpGet("searchAccounts")]
        public async Task<IActionResult> SearchAccounts(string search) {
            var thisuser = await _userManager.GetUserAsync(User);
            var sysadmin = await _db.Users.Where(u => u.UserName == "sysadmin").FirstOrDefaultAsync();
            var searchNormalized = search.ToLower();

			var users = await _db.Users
                .Where(u => 
                    u.UserName.ToLower().Contains(searchNormalized) || 
                    u.DisplayName.Contains(searchNormalized) ||
                    u.Email.Contains(searchNormalized))
                .ToListAsync();
            users.Remove(thisuser);
            users.Remove(sysadmin);

			var userRoles = new List<string>();
			foreach (var u in users) {
                var roles = await _userManager.GetRolesAsync(u);
                userRoles.Add(roles.FirstOrDefault());
			}

			return new JsonResult(new { users, userRoles });
		}

		[Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.SysAdmin}")]
		[HttpPost("deleteaccount")]
        public async Task<IActionResult> DeleteAccount(int userId) {
			var user = await _db.ApplicationUsers.FindAsync(userId);
			if (user == null)
				return BadRequest();

            var dailyPerformances = await _db.DailyPerformances.Where(dp => dp.UserId == userId).ToListAsync();
            _db.DailyPerformances.RemoveRange(dailyPerformances);
			var cardLinks = await _db.CardLinks.Where(cl => cl.UserId == userId).ToListAsync();
            _db.CardLinks.RemoveRange(cardLinks);
			var sectionLinks = await _db.SectionLinks.Where(sl => sl.UserId == userId).ToListAsync();
            _db.SectionLinks.RemoveRange(sectionLinks);
			var courseLinks = await _db.CourseLinks.Where(cl => cl.UserId == userId).ToListAsync();
            _db.CourseLinks.RemoveRange(courseLinks);
            var bugReports = await _db.BugReports.Where(br => br.ReporterId == userId).ToListAsync();
            _db.BugReports.RemoveRange(bugReports);
            var deckCards = await _db.DeckCards.Where(dc => dc.UserId == userId).ToListAsync();
            _db.DeckCards.RemoveRange(deckCards);
            var decks = await _db.Decks.Where(d => d.OwnerId == userId).ToListAsync();
            _db.Decks.RemoveRange(decks);

			await _userManager.DeleteAsync(user);
            await _db.SaveChangesAsync();

			return Ok();
		}
        [HttpGet("amhigherrole")]
        public async Task<IActionResult> AmHigherRole(int than) {
            var thisuser = await _userManager.GetUserAsync(User);
            var otheruser = await _db.ApplicationUsers.FindAsync(than);
            if (thisuser == null || otheruser == null)
				return BadRequest();

            var thisRole = await _userManager.GetRolesAsync(thisuser);
			var otherRole = await _userManager.GetRolesAsync(otheruser);
			switch (thisRole.LastOrDefault(), otherRole.LastOrDefault()) {
                case ("Admin", "User"):
					return Ok();
                case ("Admin", "Dev"):
                    return Ok();
                case ("SysAdmin", "Admin"):
                    return Ok();
                case ("SysAdmin", "Dev"):
					return Ok();
                case ("SysAdmin", "User"):
                    return Ok();
                case ("Dev", "User"):
					return Ok();
                default:
                    return Unauthorized();
            }
        }
		[Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.Dev},{ApplicationRoles.SysAdmin}")]
		[HttpGet("getrole")]
        public async Task<IActionResult> GetRole(int userId) {
            var user = await _db.ApplicationUsers.FindAsync(userId);
			if (user == null)
				return BadRequest();

			var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            return Ok($"{role}");
        }
        [HttpGet("getownrole")]
        public async Task<IActionResult> GetOwnRole() {
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return BadRequest();

			var roles = await _userManager.GetRolesAsync(user);
			var role = roles.LastOrDefault();
			return Ok($"{role}");
		}

		[Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.SysAdmin}")]
		[HttpPost("makedev")]
        public async Task<IActionResult> MakeDev(int userId) {
			var user = await _db.ApplicationUsers.FindAsync(userId);
            if (user == null)
                return BadRequest();

            await _userManager.AddToRoleAsync(user, ApplicationRoles.Dev);
            await _userManager.RemoveFromRoleAsync(user, ApplicationRoles.User);

            return Ok();
        }

        [Authorize(Roles = "SysAdmin")]
        [HttpPost("makeadmin")]
        public async Task<IActionResult> MakeAdmin(int userId) {
            var user = await _db.ApplicationUsers.FindAsync(userId);
            if (user == null)
				return BadRequest();

            await _userManager.AddToRoleAsync(user, ApplicationRoles.Admin);
            await _userManager.RemoveFromRoleAsync(user, ApplicationRoles.Dev);
            await _userManager.RemoveFromRoleAsync(user, ApplicationRoles.User);

            return Ok();
        }
	}
}
