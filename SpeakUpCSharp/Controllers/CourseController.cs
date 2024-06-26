﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeakUpCSharp.Data;
using SpeakUpCSharp.Models;

namespace SpeakUpCSharp.Controllers {
	[ApiController]
	[Route("course")]
	public class CourseController : ControllerBase {
		private readonly ApplicationDbContext _db;
		private readonly ILogger<CourseController> _logger;
		private readonly UserManager<ApplicationUser> _userManager;
        public CourseController(ApplicationDbContext db, ILogger<CourseController> logger, UserManager<ApplicationUser> userManager) {
			_db = db;
			_logger = logger;
			_userManager = userManager;
        }

        [HttpPost("setactive")]
		public async Task<IActionResult> SetActiveCourse([FromBody] string courseCode) {
			var user = await _userManager.GetUserAsync(User);
			if (user == null) { return NotFound("No user found"); }

			CourseLink courseLink = new CourseLink {
				CourseCode = courseCode,
				UserId = user.Id,
				User = user
			};
			await _db.CourseLinks.AddAsync(courseLink);
			user.LastCourse = courseCode;

			var sections = await _db.Sections.Where(s => s.CourseCode == courseCode).ToListAsync();
			foreach (var section in sections) {
				await _db.SectionLinks.AddAsync(new SectionLink {
					SectionId = section.Id,
					Section = section,
					UserId = user.Id,
					User = user,
					CourseCode = courseCode,
					Order = section.Order
				});
			}

			var courseCards = await _db.CourseCards.Where(c => c.CourseCode == courseCode).ToListAsync();
			foreach (var card in courseCards) {
				await _db.CardLinks.AddAsync(new CardLink {
					CardId = card.Id,
					Card = card,
					UserId = user.Id,
					User = user,
					Level = 0,
					LastReviewDate = DateTime.UtcNow,
					NextReviewDate = DateTime.UtcNow,
					FlaggedAsImportant = false,
					CourseCode = courseCode
				});
			}

			await _db.SaveChangesAsync();

			return Ok(courseCode);
		}

		[HttpPost("changeactive")]
		public async Task<IActionResult> ChangeActive([FromBody] string newCourse) {
			var user = await _userManager.GetUserAsync(User);
			if (user == null) { return NotFound("No user found"); }

			user.LastDeck = null;

			if (user.LastCourse == newCourse) {
				await _db.SaveChangesAsync();
				return Ok("Already studying this course");
			} else {
				user.LastCourse = newCourse;
				await _db.SaveChangesAsync();
				return Ok(newCourse);
			}	
		}

		[HttpGet("getlast")]
		public async Task<IActionResult> GetLastActive() {
			var user = await _userManager.GetUserAsync(User);
			if (user == null) { return Unauthorized(); }

			var lastCourse = user.LastCourse;
			if (lastCourse == null) {
				var attemptToFind = await _db.CourseLinks.Where(l => l.UserId == user.Id).ToListAsync();
				if (attemptToFind.Count > 0) {
					user.LastCourse = attemptToFind.FirstOrDefault().CourseCode;
					await _db.SaveChangesAsync();
				} else {
					return NotFound("No active deck");
				}
			}

			return Ok(lastCourse);
		}

		[HttpGet("getactive")]
		public async Task<IActionResult> GetAllActiveCourses() {
			var user = await _userManager.GetUserAsync(User);
			if (user == null) { return NotFound("No user!");}

			var activeCourses = await _db.CourseLinks.Where(l => l.UserId == user.Id)
				.Select(l => l.CourseCode).ToListAsync();

			if (activeCourses.Count == 0) { return NotFound("No active courses!"); }

			return new JsonResult(new { activeCourses });
		}
		[HttpPost("removeactive")]
		public async Task<IActionResult> RemoveActiveCourse(string courseCode) {
			var user = await _userManager.GetUserAsync(User);
			if (user == null) { return Unauthorized("No user!"); }

			var cardLinks = await _db.CardLinks
				.Where(l => l.UserId == user.Id && l.CourseCode == courseCode)
				.ToListAsync();
			_db.CardLinks.RemoveRange(cardLinks);

			var sectionLinks = await _db.SectionLinks
				.Where(l => l.UserId == user.Id && l.CourseCode == courseCode)
				.ToListAsync();
			_db.SectionLinks.RemoveRange(sectionLinks);

			CourseLink? courseLink = await _db.CourseLinks.Where(l => l.UserId == user.Id && l.CourseCode == courseCode).FirstOrDefaultAsync();
			if (courseLink == null) { return NotFound("Not studying this course!"); }

			_db.CourseLinks.Remove(courseLink);
			await _db.SaveChangesAsync();

			return Ok($"Removed {courseLink}");
		}
		[HttpGet("getlastedit")]
		public async Task<IActionResult> GetLastEdit(string courseCode) {
			var lastEditedSection = await _db.Sections.Where(s => s.CourseCode == courseCode).Include(s => s.LastEditor)
				.OrderByDescending(s => s.LastEdited).FirstOrDefaultAsync();

			if (lastEditedSection == null) { return NotFound(); }

			var date = lastEditedSection.LastEdited;
			var username = lastEditedSection.LastEditor.UserName;

			return new JsonResult(new { date, username });
		}
		[HttpGet("getLastCourseCode")]
		public async Task<IActionResult> GetLastCourseCode() {
			var user = await _userManager.GetUserAsync(User);
			if (user == null || user.LastCourse == null) { return Unauthorized(); }

			return Ok(user.LastCourse);
		}

		[HttpGet("listactivecoursecodes")]
		public async Task<IActionResult> ListActiveCourses() {
			var user = await _userManager.GetUserAsync(User);
			if (user == null) { return Unauthorized(); }

			List<string> courses = await _db.CourseLinks
				.Where(l => l.UserId == user.Id)
				.Select(l => l.CourseCode)
				.ToListAsync();
			if (courses.Count == 0) { return Unauthorized(); }

			return new JsonResult(new { courses });
		}
		[HttpGet("getCoursePerformance")]
		public async Task<IActionResult> GetCoursePerformance(string courseCode) {
			var user = await _userManager.GetUserAsync(User);
			if (user == null) { return Unauthorized(); }

			int learnedWords = await _db.CardLinks
				.Where(l => l.UserId == user.Id && l.CourseCode == courseCode && l.Level > 0)
				.CountAsync();
			int totalWords = await _db.CourseCards
				.Where(c => c.CourseCode == courseCode)
				.CountAsync();
			int totalLeft = totalWords - learnedWords;
			int percentageTotalLeft = (int)Math.Round((double)learnedWords / totalWords * 100);
			int goalWords = user.DailyWordGoal * 200;
			int goalLeft = goalWords - learnedWords;
			int percentageGoalLeft = (int)Math.Round((double)learnedWords / goalWords * 100);

			return new JsonResult(new { learnedWords, totalWords, totalLeft, percentageTotalLeft, goalWords, goalLeft, percentageGoalLeft });
		}
	}
}
