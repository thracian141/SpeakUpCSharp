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
		public async Task<IActionResult> NextCourseCard(int skipId) {
			var user = await _userManager.GetUserAsync(User);
			var dailyPerformance = await _daily.GetDailyPerformance(user.Id);

			int newWordsRemaining = user.DailyWordGoal - dailyPerformance.NewWords;
			bool thereAreNewCards = await _db.CardLinks
				.AnyAsync(l => l.UserId == user.Id && l.CourseCode == user.LastCourse && l.Level == 0 && l.NextReviewDate <= DateTime.UtcNow && l.Id != skipId);

			if (newWordsRemaining > 0 && thereAreNewCards) { // The User must learn today's new words
				var newCards = await _db.CardLinks
					.Where(l => l.UserId == user.Id && l.CourseCode == user.LastCourse && l.Level == 0 && l.Id != skipId)
					.Include(c => c.Card)
					.OrderBy(l => l.Card.Difficulty)
					.Take(newWordsRemaining).ToListAsync();
				if (!newCards.Any())
					return NoContent();
				var cardLink = newCards.FirstOrDefault();
				var card = cardLink.Card;
				var sentence = await _db.Sentences.Where(s => s.WordId == card.Id).OrderBy(s => Guid.NewGuid()).FirstOrDefaultAsync();
				if (sentence == null) sentence = new Sentence { Front = card.Front, Back = card.Back, Id = 0, WordId = card.Id };

				return new JsonResult(new { cardLink, card, sentence });
			} 
			else { // Already passed through the new words for today
				int reviewWordsRemaining = (user.DailyWordGoal * 5) - dailyPerformance.WordsGuessedCount;
				var reviewCards = await _db.CardLinks
					.Where(l => l.UserId == user.Id && l.CourseCode == user.LastCourse && l.NextReviewDate <= DateTime.UtcNow && l.Id != skipId)
					.Include(c => c.Card)
					.OrderBy(l => l.Card.Difficulty)
					.ThenBy(l => l.Level)
					.Take(reviewWordsRemaining).ToListAsync();
				if (!reviewCards.Any())
					return NoContent();
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
		[HttpGet("anycards")]
		public async Task<IActionResult> AnyCardsToStudy() {
			var user = await _userManager.GetUserAsync(User);
			bool anyCards = await _db.CardLinks
				.AnyAsync(l => l.UserId == user.Id && l.NextReviewDate <= DateTime.UtcNow && l.CourseCode == user.LastCourse);

			return new JsonResult(new { anyCards });
		}

		[HttpGet("nextdeckcard")]
		public async Task<IActionResult> NextDeckCard(int skipId) {
			var user = await _userManager.GetUserAsync(User);
			var rng = new Random();
			var chance = rng.Next(0, 100);
			bool thereAreNewCards = await _db.DeckCards
				.AnyAsync(c => c.DeckId == user.LastDeck && c.NextReviewDate <= DateTime.UtcNow && c.Level == 0 && c.Id != skipId);

			if (chance < 15 && thereAreNewCards) { // Random new card will be picked
				var randomCard = await _db.DeckCards
					.Where(c => c.DeckId == user.LastDeck && c.NextReviewDate <= DateTime.UtcNow && c.Level == 0 && c.Id != skipId)
					.OrderBy(c => Guid.NewGuid())
					.FirstOrDefaultAsync();
				return new JsonResult(new { randomCard });
			} else { // Card for review will be picked
				bool thereAreAnyCards = await _db.DeckCards
					.AnyAsync(c => c.DeckId == user.LastDeck && c.NextReviewDate <= DateTime.UtcNow && c.Level == 0 && c.Id != skipId);
				if (!thereAreAnyCards) return NoContent(); // Checking if there are any cards to study

				var randomCard = await _db.DeckCards
					.Where(c => c.DeckId == user.LastDeck && c.NextReviewDate <= DateTime.UtcNow && c.Level == 0 && c.Id != skipId)
					.OrderBy(c => c.Level)
					.ThenBy(c => c.Difficulty)
					.FirstOrDefaultAsync();
				return new JsonResult(new { randomCard });
			}
		}

		[HttpPost("leveldeckcard")]
		public async Task<IActionResult> LevelDeckCard([FromBody] LevelCardInputModel levelModel) {
			var card = await _db.DeckCards.FindAsync(levelModel.LinkId);
			if (card == null) return NotFound();
			if (levelModel.Correct) {
				int levelIncrement = 6 - (int)Math.Round((double)levelModel.Difficulty / 2, 1);
				card.Level += levelIncrement;
				card.NextReviewDate = _reviewDate.CorrectAnswerGetReviewDate(card.Level);
				card.LastReviewDate = DateTime.UtcNow;
			} else {
				card.NextReviewDate = _reviewDate.WrongAnswerGetReviewDate();
			}

			await _db.SaveChangesAsync();
			return Ok();
		}
	}
}
