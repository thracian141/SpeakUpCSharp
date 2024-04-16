using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeakUp.Models;
using SpeakUpCSharp.Data;
using SpeakUpCSharp.Models;
using static System.Net.Mime.MediaTypeNames;

namespace SpeakUpCSharp.Controllers {
	[ApiController]
	[Route("learn")]
	public class LearnController : Controller {
		private readonly ApplicationDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<AccountController> _logger;

		public LearnController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
		ILogger<AccountController> logger) {
			_db = db;
			_userManager = userManager;
			_logger = logger;
        }

        [HttpGet("updateCourse")]
		public async Task<IActionResult> UpdateCourse() {
			var user = await _userManager.GetUserAsync(User);
			if (user == null) { return Unauthorized(); }

			var activeSectionId = _db.SectionLinks.Where(sl => sl.CourseCode == user.LastCourse && sl.CurrentActive)
				.Select(sl => sl.SectionId).FirstOrDefault();
			var cardLinks = await _db.CardLinks.Where(l => l.CourseCode == user.LastCourse && l.Card.SectionId == activeSectionId && l.NextReviewDate <= DateTime.UtcNow)
				.Include(c => c.Card)
				.OrderByDescending(l => l.Card.Difficulty)
				.ThenByDescending(c => c.Level)
				.ThenByDescending(c => c.NextReviewDate)
				.Take(50).ToListAsync();
			var cards = cardLinks.Select(l => l.Card).ToList();

			List<Sentence> sentences = new List<Sentence>();
			//for each cardLink, find a random sentence that has the same sentence.CardId as the cardLink.CardId
			foreach (var cardLink in cardLinks) {
				_logger.LogInformation("Card: " + cardLink.Card.Front);
				var sentence = await _db.Sentences.Where(s => s.WordId == cardLink.CardId).OrderBy(r => Guid.NewGuid()).FirstOrDefaultAsync();
				if (sentence == null)
					sentence = new Sentence {
						Id = 0,
						Front = cardLink.Card.Front,
						Back = cardLink.Card.Back,
						WordId = cardLink.CardId,
						Word = cardLink.Card
					};
				sentences.Add(sentence);
			}

			return new JsonResult(new { cardLinks, sentences, cards });
		}
		[HttpGet("updateDeck")]
		public async Task<IActionResult> UpdateDeck() {
			var user = await _userManager.GetUserAsync(User);
			if (user == null) { return Unauthorized(); }

			var cards = await _db.DeckCards.Where(c => c.DeckId == user.LastDeck && c.NextReviewDate <= DateTime.UtcNow)
				.OrderByDescending(c => c.Difficulty)
				.ThenByDescending(c => c.Level)
				.ThenByDescending(c => c.NextReviewDate)
				.Take(50).ToListAsync();

			return new JsonResult(new { cards });
		}
		[HttpGet("learningDeck")]
		public async Task<IActionResult> LearningDeck() {
			bool[] learningDeckLearningCourse = [false, false];
			var user = await _userManager.GetUserAsync(User);

			if (user == null) return Unauthorized();

			if (user.LastDeck != null) {
				learningDeckLearningCourse[0] = true;
			}
			if (user.LastCourse != null) {
				learningDeckLearningCourse[1] = true;
			}

			return new JsonResult(new { learningDeckLearningCourse });
		}
	}
}
