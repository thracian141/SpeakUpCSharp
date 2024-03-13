using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeakUp.Models;
using SpeakUpCSharp.Data;
using SpeakUpCSharp.Models.InputModels;

namespace SpeakUpCSharp.Controllers {
	[ApiController]
	[Route("card")]
	public class CardController : ControllerBase {
		private readonly ApplicationDbContext _db;
		private readonly ILogger<CardController> _logger;
		public CardController(ApplicationDbContext db, ILogger<CardController> logger) {
			_db = db;
			_logger = logger;
		}

		[HttpGet("listByDeck")]
		public async Task<IActionResult> ListByDeck(int deckId) {
			var list = await _db.Cards.Where(c => c.DeckId == deckId).ToListAsync();

			return new JsonResult(new { list });
		}
		[HttpGet("listBySection")]
		public async Task<IActionResult> ListBySection(int sectionId) {
			var list = await _db.Cards.Where(c => c.SectionId == sectionId).ToListAsync();

			return new JsonResult(new { list });
		}

		[HttpPost("add")]
		public async Task<IActionResult> AddCard([FromBody] CardInputModel cardInput) {
			Card card = new Card {
				Front = cardInput.Front,
				Back = cardInput.Back,
				Level = 0,
				Difficulty = cardInput.Difficulty,
				DateAdded = DateTime.Now,
				LastReviewDate = DateTime.Now,
				NextReviewDate = DateTime.Now
			};
			if (cardInput.SectionId != 0) { //If there is no sectionId provided, the property of the card 
				Section? section = await _db.Sections.FindAsync(cardInput.SectionId); // will be left blank 
				if (section == null)                                            // as it doesn't belong
					return NotFound("Adding to non-existent section");          //  to any course
				else {
					card.SectionId = cardInput.SectionId;
					card.Section = section;
				}
			}
			if (cardInput.DeckId != 0) {
				Deck? deck = await _db.Decks.FindAsync(cardInput.DeckId);
				if (deck == null)
					return NotFound("Adding to non-existent deck");
				else {
					card.DeckId = cardInput.DeckId;
					card.Deck = deck;
				}
			}
			await _db.Cards.AddAsync(card);
			await _db.SaveChangesAsync();

			return new JsonResult(new { card });
		}
		[HttpPost("delete")]
		public async Task<IActionResult> DeleteById(int id) {
			var card = await _db.Cards.FindAsync(id);
			if (card == null) { return NotFound(); }

			_db.Remove(card);
			await _db.SaveChangesAsync();

			return Ok();
		}
	}
}
