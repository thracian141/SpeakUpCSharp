using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeakUpCSharp.Data;
using SpeakUpCSharp.Models;
using SpeakUpCSharp.Models.InputModels;
using SpeakUpCSharp.Utilities;

namespace SpeakUpCSharp.Controllers {
	[ApiController]
	[Route("bugs")]
	public class BugReportController : ControllerBase {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ApplicationDbContext _db;

		public BugReportController(UserManager<ApplicationUser> userManager, ApplicationDbContext db) {
			_userManager = userManager;
			_db = db;
		}

		[HttpGet("checkifdev")]
		public async Task<IActionResult> CheckIfDev() {
			var user = await _userManager.GetUserAsync(User);
			bool isDev = await _userManager.IsInRoleAsync(user, ApplicationRoles.Admin) ||
				await _userManager.IsInRoleAsync(user, ApplicationRoles.SysAdmin);
			if (!isDev) 
				isDev = await _userManager.IsInRoleAsync(user, ApplicationRoles.Dev);

			if (isDev)
				return Ok();
			else
				return Unauthorized();
		}

		[HttpPost("reportbug")]
		public async Task<IActionResult> ReportBug([FromBody] BugReportInputModel model) {
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return Unauthorized();

			var card = await _db.CourseCards.FindAsync(model.CardId);
			if (card == null)
				return BadRequest();

			var bugReport = new BugReport {
				Id = 0,
				Text = model.Text,
				CourseCode = model.CourseCode,
				ReporterId = user.Id,
				Reporter = user,
				CardId = model.CardId,
				Card = card
			};

			await _db.BugReports.AddAsync(bugReport);
			await _db.SaveChangesAsync();

			return Ok();
		}
		[Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.Dev},{ApplicationRoles.SysAdmin}")]
		[HttpGet("listbugs")]
		public async Task<IActionResult> ListBugs(string courseCode) {
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return BadRequest();

			if (courseCode == "all") {
				var bugs = await _db.BugReports.Include(b => b.Card).ToListAsync();
				return new JsonResult(new { bugs });
			} else {
				var bugs = await _db.BugReports.Include(b=>b.Card).Where(b => b.CourseCode == courseCode).ToListAsync();
				return new JsonResult(new { bugs });
			}
		}

		[Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.Dev},{ApplicationRoles.SysAdmin}")]
		[HttpPost("resolvebug")]
		public async Task<IActionResult> ResolveBug(int id) {
			var bug = await _db.BugReports.FindAsync(id);
			if (bug == null)
				return BadRequest();

			_db.BugReports.Remove(bug);
			await _db.SaveChangesAsync();

			return Ok();
		}
	}
}
