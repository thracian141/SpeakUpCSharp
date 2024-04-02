using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeakUp.Models;
using SpeakUpCSharp.Data;
using SpeakUpCSharp.Models;
using SpeakUpCSharp.Models.InputModels;

namespace SpeakUpCSharp.Controllers {
	[ApiController]
	[Route("card")]
	public class CardController : ControllerBase {
		private readonly ApplicationDbContext _db;
		private readonly ILogger<CardController> _logger;
		private readonly UserManager<ApplicationUser> _userManager;
		public CardController(ApplicationDbContext db, ILogger<CardController> logger, UserManager<ApplicationUser> userManager) {
			_db = db;
			_logger = logger;
			_userManager = userManager;
		}

		[HttpGet("listByDeck")]
		public async Task<IActionResult> ListByDeck(int deckId) {
			var list = await _db.DeckCards.Where(c => c.DeckId == deckId).ToListAsync();

			return new JsonResult(new { list });
		}
		[HttpGet("listBySection")]
		public async Task<IActionResult> ListBySection(int sectionId) {
			var list = await _db.CourseCards.Where(c => c.SectionId == sectionId).ToListAsync();

			return new JsonResult(new { list });
		}

		[HttpPost("addToDeck")]
		public async Task<IActionResult> AddCardToDeck([FromBody] DeckCardInput cardInput) {
			Deck deck = await _db.Decks.FindAsync(cardInput.DeckId);
			ApplicationUser user = await _userManager.GetUserAsync(User);
			if (deck == null || user == null) {
				return NotFound();
			}

			DeckCard card = new DeckCard {
				Front = cardInput.Front,
				Back = cardInput.Back,
				Difficulty = cardInput.Difficulty,
				DateAdded = DateTime.UtcNow,
				NextReviewDate = DateTime.UtcNow,
				LastReviewDate = DateTime.UtcNow,
				FlaggedAsImportant = false,
				DeckId = cardInput.DeckId,
				Deck = deck,
				UserId = user.Id,
				User = user
			};

			await _db.DeckCards.AddAsync(card);
			await _db.SaveChangesAsync();

			return new JsonResult(new { card });
		}
		[HttpPost("addToCourseSection")]
		public async Task<IActionResult> AddCardToSection([FromBody] CourseCardInput cardInput) {
			var section = await _db.Sections.FindAsync(cardInput.SectionId);
			var user = await _userManager.GetUserAsync(User);
			if (section == null || user == null) {
				return NotFound();
			}

			CourseCard card = new CourseCard {
				Front = cardInput.Front,
				Back = cardInput.Back,
				Difficulty = cardInput.Difficulty,
				PartOfSpeech = cardInput.PartOfSpeech,
				DateAdded = DateTime.UtcNow,
				SectionId = cardInput.SectionId,
				Section = section,
				CourseCode = cardInput.CourseCode
			};

			await _db.CourseCards.AddAsync(card);
			await _db.SaveChangesAsync();

			_logger.LogInformation("CARD ADDED TO COURSE");

			var sectionLinks = await _db.SectionLinks.Where(l => l.SectionId == section.Id).Include(l => l.User).ToListAsync();
			foreach (var link in sectionLinks) {
				await _db.CardLinks.AddAsync(new CardLink {
					CardId = card.Id,
					Card = card,
					UserId = link.UserId,
					User = link.User,
					Level = 0,
					FlaggedAsImportant = false,
					CourseCode = section.CourseCode,
					LastReviewDate = DateTime.UtcNow,
					NextReviewDate = DateTime.UtcNow
				});
			}
			await _db.SaveChangesAsync();

			_logger.LogInformation("CARD LINKS ADDED TO USERS");

			return new JsonResult(new { card });

		}
		[HttpPost("deleteFromDeck")]
		public async Task<IActionResult> DeleteFromDeck(int id) {
			var card = await _db.DeckCards.FindAsync(id);
			if (card == null) { return NotFound(); }

			_db.Remove(card);
			await _db.SaveChangesAsync();

			return Ok();
		}
		[HttpPost("deleteFromCourse")]
		public async Task<IActionResult> DeleteFromCourse(int id) {
			var card = await _db.CourseCards.FindAsync(id);
			if (card == null) { return NotFound(); }

			var links = await _db.CardLinks.Where(l => l.CardId == card.Id).ToListAsync();
			_db.CardLinks.RemoveRange(links);
			await _db.SaveChangesAsync();

			_db.Remove(card);
			await _db.SaveChangesAsync();

			return Ok();
		}
		[HttpPost("editFromDeck")]
		public async Task<IActionResult> EditFromDeck([FromBody] EditDeckCardModel edit) {
			var oldCard = await _db.DeckCards.FindAsync(edit.Id);
			if (oldCard == null) { return BadRequest(); }

			oldCard.Front = edit.Front;
			oldCard.Back = edit.Back;
			oldCard.Difficulty = edit.Difficulty;
			oldCard.FlaggedAsImportant = edit.FlaggedAsImportant;

			await _db.SaveChangesAsync();

			return new JsonResult(new { oldCard });
		}
		[HttpPost("editFromCourse")]
		public async Task<IActionResult> EditFromCourse([FromBody] EditCourseCardModel edit) {
			var oldCard = await _db.CourseCards.FindAsync(edit.Id);
			if (oldCard == null) { return BadRequest(); }

			oldCard.Front = edit.Front;
			oldCard.Back = edit.Back;
			oldCard.Difficulty = edit.Difficulty;
			oldCard.PartOfSpeech = edit.PartOfSpeech;

			var userCardLinks = await _db.CardLinks.Where(c => c.CardId == oldCard.Id).ToListAsync();
			foreach (CardLink cardLink in userCardLinks) {
				cardLink.Level = 0;
			}

			await _db.SaveChangesAsync();

			return new JsonResult(new { oldCard });
		}

		[HttpGet("getCourseCardById")]
		public async Task<IActionResult> GetCourseCard(int id) {
			var card = await _db.CourseCards.FindAsync(id);
			if (card == null)
				return NotFound();

			return new JsonResult(new { card });
		}

		[HttpPost("levelDeckCard")]
		public async Task<IActionResult> LevelDeckCard(int cardId, int pchange, int nchange) {
			var card = await _db.DeckCards.FindAsync(cardId);
			if (card == null) { return BadRequest(); }

			card.Level += pchange;
			card.Level -= nchange;

			await _db.SaveChangesAsync();

			return Ok(card.Level);
		}

		[HttpPost("levelCourseCard")]
		public async Task<IActionResult> LevelCourseCard(int cardId, int pchange, int nchange) {
			var cardLink = await _db.CardLinks.Where(l => l.CardId == cardId).FirstOrDefaultAsync();
			if (cardLink == null) { return BadRequest(); }

			cardLink.Level += pchange;
			cardLink.Level -= nchange;

			await _db.SaveChangesAsync();

			return Ok(cardLink.Level);
		}
	}
}
