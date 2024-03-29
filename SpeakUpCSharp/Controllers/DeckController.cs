using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeakUp.Models;
using SpeakUpCSharp.Data;
using SpeakUpCSharp.Models;
using SpeakUpCSharp.Models.InputModels;
using System.Diagnostics;

namespace SpeakUpCSharp.Controllers {
	[ApiController]
	[Route("deck")]
	public class DeckController : Controller {
		private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;

		public DeckController(ILogger<HomeController> logger, ApplicationDbContext db, UserManager<ApplicationUser> userManager) {
			_logger=logger;
			_db = db;
			_userManager = userManager;
		}

		[HttpPost("create")]
		public async Task<IActionResult> CreateDeck([FromBody] DeckInputModel deckInput) {
			var creator = await _userManager.GetUserAsync(User);

			Deck deck = new Deck {
				DeckName = deckInput.DeckName,
				DeckDescription = deckInput.DeckDescription,
				Owner = creator,
				OwnerId = creator.Id,
				Level = 0,
				Difficulty = 0
			};

			await _db.Decks.AddAsync(deck);
			await _db.SaveChangesAsync();

			return Ok(deck.Id);
		}
		[HttpGet("getById")]
		public async Task<IActionResult> GetById(string id) { //id cannot be int initially
			var deck = await _db.Decks.FindAsync(Int32.Parse(id));
			if (deck == null) return NotFound();

			return new JsonResult(new { deck });
		}
		[HttpGet("list")]
		public async Task<IActionResult> ListDecks() {
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return NotFound("Not logged in");

			var list = await _db.Decks.Where(d => d.OwnerId == user.Id).ToListAsync();

			_logger.LogInformation(list.ToString());
			return new JsonResult(new { list });
		}
	}
}
