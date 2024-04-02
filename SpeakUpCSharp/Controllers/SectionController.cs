using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeakUp.Models;
using SpeakUpCSharp.Data;
using SpeakUpCSharp.Models;
using SpeakUpCSharp.Models.InputModels;

namespace SpeakUpCSharp.Controllers {
	[ApiController]
	[Route("section")]
	public class SectionController : ControllerBase {
		private readonly ApplicationDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<SectionController> _logger;

		public SectionController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, ILogger<SectionController> logger) {
			_db = db;
			_userManager = userManager;
			_logger = logger;
        }
		[HttpGet("getbyid")]
		public async Task<IActionResult> GetById(int id) {
			var section = await _db.Sections.FindAsync(id);
			if (section == null) { return NotFound("Section not found..."); }

			return new JsonResult(new { section });
		}

        [HttpGet("listbycourse")]
		public async Task<IActionResult> ListByCourse(string courseCode) {
			_logger.LogInformation("Method called");
			var sections = await _db.Sections.Where(s => s.CourseCode == courseCode).OrderBy(s => s.Order).ToListAsync();

			return new JsonResult( new { sections });
		}

		[HttpPost("create")]
		public async Task<IActionResult> CreateSection([FromBody] SectionInputModel sectionInput) {
			var user = await _userManager.GetUserAsync(User);
			if (user == null) { return Unauthorized("User error!"); }

			int? order = 1;

			int? lastOrder = await _db.Sections.Where(s => s.CourseCode == sectionInput.CourseCode)
				.OrderByDescending(s => s.Order).Select(s => s.Order)
				.FirstOrDefaultAsync();

			if (lastOrder != null)
				order = lastOrder + 1;

			Section section = new Section {
				Title = sectionInput.Title,
				Description = sectionInput.Description,
				LastEdited = DateTime.Now,
				LastEditorId = user.Id,
				LastEditor = user,CourseCode = sectionInput.CourseCode,
				Order = (int)order
			};

			await _db.Sections.AddAsync(section);
			await _db.SaveChangesAsync();


			var courseLinks = await _db.CourseLinks.Where(l => l.CourseCode == section.CourseCode).Include(l => l.User).ToListAsync();
			foreach (CourseLink link in courseLinks) {
				await _db.SectionLinks.AddAsync(new SectionLink {
					SectionId = section.Id,
					Section = section,
					UserId = link.UserId,
					User = link.User,
					CourseCode = link.CourseCode,
					Order = (int)order
				});
			}
			await _db.SaveChangesAsync();

			return Ok(section.Id);
		}

		[HttpPost("delete")]
		public async Task<IActionResult> Delete(int sectionId) {
			var section = await _db.Sections.FindAsync(sectionId);
			if (section == null) { return Unauthorized(); }

			var courseCards = await _db.CourseCards.Where(c => c.SectionId == section.Id).ToListAsync();
			foreach (var courseCard in courseCards) {
				var currentCardLinks = await _db.CardLinks.Where(l => l.CardId == courseCard.Id).ToListAsync();
				_db.CardLinks.RemoveRange(currentCardLinks);
			}
			await _db.SaveChangesAsync();

			_db.RemoveRange(courseCards);
			await _db.SaveChangesAsync();

			var sectionLinks = await _db.SectionLinks.Where(l => l.SectionId == section.Id).ToListAsync();
			_db.SectionLinks.RemoveRange(sectionLinks);

			_db.Remove(section);
			await _db.SaveChangesAsync();

			return Ok("Removed!");
		}

		[HttpPost("orderup")]
		public async Task<IActionResult> OrderUp(int sectionId) {
			_logger.LogInformation("UP CALLED");
			var thisSection = await _db.Sections.FindAsync(sectionId);
			if (thisSection == null) 
				return NotFound();
			if (thisSection.Order == 1) 
				return Ok("This is already the first section!");

			var sectionAbove = await _db.Sections.Where(s => s.CourseCode == thisSection.CourseCode && 
				s.Order == (thisSection.Order - 1)).FirstOrDefaultAsync();

			if (sectionAbove == null)
				return Ok("This is already the first section!");
			else {
				sectionAbove.Order = sectionAbove.Order + 1;
				thisSection.Order = thisSection.Order - 1;
				await _db.SaveChangesAsync();
			}

			return Ok("Ordered up!");
		}

		[HttpPost("orderdown")]
		public async Task<IActionResult> OrderDown(int sectionId) {
			_logger.LogInformation("DOWN CALLED");
			var thisSection = await _db.Sections.FindAsync(sectionId);
			if (thisSection == null)
				return NotFound();

			var sectionBelow = await _db.Sections.Where(s => s.CourseCode == thisSection.CourseCode &&
				s.Order == (thisSection.Order + 1)).FirstOrDefaultAsync();

			if (sectionBelow == null)
				return Ok("This is already the last section!");
			else {
				sectionBelow.Order = sectionBelow.Order - 1;
				thisSection.Order = thisSection.Order + 1;
				await _db.SaveChangesAsync();
			}

			return Ok("Ordered down!");
		}

		[HttpGet("linksPerCourse")]
		public async Task<IActionResult> LinksPerCourse(string courseCode) {
			var user = await _userManager.GetUserAsync(User);
			if (user == null) { return Unauthorized("User error!"); }

			var links = await _db.SectionLinks.Where(l => l.CourseCode == courseCode && l.UserId == user.Id).Include(l => l.Section).ToListAsync();
			if (!links.Any(l => l.CurrentActive))
				links.Where(l => l.Order == 1).First().CurrentActive = true;

			await _db.SaveChangesAsync();

			return new JsonResult(new { links });
		}
	}
}
