using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeakUp.Models;
using SpeakUpCSharp.Data;
using SpeakUpCSharp.Models;
using SpeakUpCSharp.Models.InputModels;
using SpeakUpCSharp.Utilities;
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
		[HttpPost("setactivedeck")]
		public async Task<IActionResult> SetActiveDeck(int deckId) {
			var user = await _userManager.GetUserAsync(User);
			var deck = await _db.Decks.FindAsync(deckId);
			if (user == null || deck == null)
				return NotFound("User or deck not found");
			user.LastDeck = deckId;
			await _db.SaveChangesAsync();
			return Ok();
		}
		[HttpGet("getlastdeck")]
		public async Task<IActionResult> GetLastDeck() {
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return NotFound("User not found");
			if (user.LastDeck == null)
				return NoContent();
			var deck = await _db.Decks.FindAsync(user.LastDeck);
			if (deck == null)
				return NotFound("Deck not found");
			return new JsonResult(new { deck });
		}
		[HttpPost("deletedeck")]
		public async Task<IActionResult> DeleteDeck(int deckId) {
			var user = await _userManager.GetUserAsync(User);
			var deck = await _db.Decks.FindAsync(deckId);
			if (user == null || deck == null)
				return NotFound("User or deck not found");
			if (user.Id != deck.OwnerId)
				return Unauthorized("Not the owner of the deck");


			var cards = await _db.DeckCards.Where(c => c.DeckId == deckId).ToListAsync();

			_db.DeckCards.RemoveRange(cards);
			_db.Decks.Remove(deck);
			await _db.SaveChangesAsync();
			return Ok();
		}
		[HttpPost("editDeck")]
		public async Task<IActionResult> EditDeck([FromBody] string[] values) {
			var user = await _userManager.GetUserAsync(User);
			var deck = await _db.Decks.FindAsync(Int32.Parse(values[0]));

			deck.DeckName = values[1];
			deck.DeckDescription = values[2];
			await _db.SaveChangesAsync();

			return new JsonResult(new { deck });
		}

		[Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.SysAdmin}")]
		[HttpGet("search")]
		public async Task<IActionResult> SearchDecks(string search) {
			if (search == "@all") {
				var list = await _db.Decks.Include(d => d.Owner).ToListAsync();
				var owners = list.Select(d => d.Owner).Select(o => o.UserName).ToList();
				return new JsonResult(new { list, owners });
			} else {
				var searchNormalized = search.ToLower();
				var list = await _db.Decks.Where(d => d.DeckName.ToLower().Contains(searchNormalized)).Include(d => d.Owner).ToListAsync();
				list.Concat(await _db.Decks.Where(d => d.DeckDescription.ToLower().Contains(searchNormalized)).ToListAsync());

				var owners = list.Select(d => d.Owner).Select(o => o.UserName).ToList();

				return new JsonResult(new { list, owners });
			}
		}
		[Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.SysAdmin}")]
		[HttpGet("getowner")]
		public async Task<IActionResult> GetDeckOwner(int deckId) {
			var deck = await _db.Decks.FindAsync(deckId);
			if (deck == null)
				return NotFound("Deck not found");
			var owner = await _db.Users.FindAsync(deck.OwnerId);
			if (owner == null)
				return NotFound("Owner not found");
			return Ok(owner.UserName);
		}
	}
}
