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
			await _db.SaveChangesAsync();

			user.LastCourse = courseCode;
			await _db.SaveChangesAsync();

			return Ok(courseCode);
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
	}
}
