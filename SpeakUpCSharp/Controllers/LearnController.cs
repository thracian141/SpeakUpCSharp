using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeakUp.Models;
using SpeakUpCSharp.Data;
using SpeakUpCSharp.Models;
using SpeakUpCSharp.Models.InputModels;
using SpeakUpCSharp.Services;

namespace SpeakUpCSharp.Controllers {
	[ApiController]
	[Route("learn")]
	public class LearnController : Controller {
		private readonly ApplicationDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<AccountController> _logger;
		private readonly IReviewDateService _reviewDate;
		private readonly IDailyPerformanceService _daily;

		public LearnController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
		ILogger<AccountController> logger, IReviewDateService reviewDate, IDailyPerformanceService daily) {
			_db = db;
			_userManager = userManager;
			_logger = logger;
			_reviewDate = reviewDate;
			_daily = daily;
		}
		[HttpGet("learningDeck")]
		public async Task<IActionResult> LearningDeck() {
			bool[] learningDeckLearningCourse = [false, false];
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
				return Unauthorized();

			if (user.LastDeck != null) {
				learningDeckLearningCourse[0] = true;
			}
			if (user.LastCourse != null) {
				learningDeckLearningCourse[1] = true;
			}

			return new JsonResult(new { learningDeckLearningCourse });
		}

		[HttpGet("nextcoursecard")] 
		public async Task<IActionResult> NextCourseCard() {
			_logger.LogCritical("NEXT COURSE CARD METHOD REACHED");
			var user = await _userManager.GetUserAsync(User);
			var dailyPerformance = await _daily.GetDailyPerformance(user.Id);

			int newWordsRemaining = user.DailyWordGoal - dailyPerformance.NewWords;

			if (newWordsRemaining > 0) { // The User must learn today's new words
				var newCards = await _db.CardLinks
					.Where(l => l.UserId == user.Id && l.CourseCode == user.LastCourse && l.Level == 0)
					.Include(c => c.Card)
					.OrderBy(l => l.Card.Difficulty)
					.Take(newWordsRemaining).ToListAsync();
				var cardLink = newCards.FirstOrDefault();
				var card = cardLink.Card;
				var sentence = await _db.Sentences.Where(s => s.WordId == card.Id).OrderBy(s => Guid.NewGuid()).FirstOrDefaultAsync();
				if (sentence == null) sentence = new Sentence { Front = card.Front, Back = card.Back, Id = 0, WordId = card.Id };

				return new JsonResult(new { cardLink, card, sentence });
			} 
			else { // Already passed through the new words for today
				int reviewWordsRemaining = (user.DailyWordGoal * 5) - dailyPerformance.WordsGuessedCount;
				var reviewCards = await _db.CardLinks
					.Where(l => l.UserId == user.Id && l.CourseCode == user.LastCourse && l.Level > 0 && l.NextReviewDate <= DateTime.UtcNow)
					.Include(c => c.Card)
					.OrderBy(l => l.Card.Difficulty)
					.ThenBy(l => l.Level)
					.Take(reviewWordsRemaining).ToListAsync();
				var cardLink = reviewCards.FirstOrDefault();
				var card = cardLink.Card;
				var sentence = await _db.Sentences.Where(s => s.WordId == card.Id).OrderBy(s => Guid.NewGuid()).FirstOrDefaultAsync();
				if (sentence == null) sentence = new Sentence { Front = card.Front, Back = card.Back, Id = 0, WordId = card.Id };

				return new JsonResult(new { cardLink, card, sentence });
			}
		}

		[HttpPost("levelcoursecard")]
		public async Task<IActionResult> LevelCourseCard([FromBody] LevelCardInputModel levelModel) {
			var cardLink = await _db.CardLinks.FindAsync(levelModel.LinkId);
			if (cardLink == null) return NotFound();
			if (levelModel.Correct) {
				if (cardLink.Level == 0) {
					await _daily.AddNewWord((int)cardLink.UserId);
				}
				int levelIncrement = 6 - (int)Math.Round((double)levelModel.Difficulty / 2, 1);
				cardLink.Level += levelIncrement;
				cardLink.NextReviewDate = _reviewDate.CorrectAnswerGetReviewDate(cardLink.Level);
				cardLink.LastReviewDate = DateTime.UtcNow;
				await _daily.AddGuessedWord((int)cardLink.UserId);
			} else {
				cardLink.NextReviewDate = _reviewDate.WrongAnswerGetReviewDate();
			}

			await _db.SaveChangesAsync();
			return Ok();
		}
	}
}
