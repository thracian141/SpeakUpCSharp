using Microsoft.EntityFrameworkCore;
using SpeakUpCSharp.Data;
using SpeakUpCSharp.Models;

namespace SpeakUpCSharp.Services {
	public class CourseService : ICourseService {
		private readonly ApplicationDbContext _db;
		private readonly ILogger<CourseService> _logger;
		public CourseService(ApplicationDbContext db, ILogger<CourseService> logger) {
			_db = db;
			_logger = logger;
		}

		public async Task StartLearningCourse(int userId, string courseCode) {
			var user = await _db.ApplicationUsers.FindAsync(userId);
			user.LastCourse = courseCode;

			var courseLink = new CourseLink {
				CourseCode = courseCode,
				UserId = userId,
				User = user
			};
			await _db.CourseLinks.AddAsync(courseLink);
			await _db.SaveChangesAsync();

			var sections = await _db.Sections.Where(s => s.CourseCode == courseCode).ToListAsync();
			List<SectionLink> sectionLinks = new List<SectionLink>();
			foreach (var section in sections) {
				sectionLinks.Add(new SectionLink {
					SectionId = section.Id,
					Section = section,
					UserId = userId,
					User = user,
					CourseCode = courseCode,
					Order = section.Order
				});		
			}
			await _db.SectionLinks.AddRangeAsync(sectionLinks);
			await _db.SaveChangesAsync();

			var courseCards = await _db.CourseCards.Where(c => c.CourseCode == courseCode).ToListAsync();
			List<CardLink> cardLinks = new List<CardLink>();
			foreach (var card in courseCards) {
				cardLinks.Add(new CardLink {
					CardId = card.Id,
					Card = card,
					UserId = userId,
					User = user,
					Level = 0,
					LastReviewDate = DateTime.UtcNow,
					NextReviewDate = DateTime.UtcNow,
					FlaggedAsImportant = false,
					CourseCode = courseCode
				});
			}
			await _db.CardLinks.AddRangeAsync(cardLinks);
			await _db.SaveChangesAsync();
		}
	}
}
