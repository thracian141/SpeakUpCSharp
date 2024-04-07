using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeakUp.Models;
using SpeakUpCSharp.Data;
using SpeakUpCSharp.Models.InputModels;

namespace SpeakUpCSharp.Controllers {
	[ApiController]
	[Route("sentence")]
	public class SentenceController : ControllerBase {
		private readonly ApplicationDbContext _db;
		private readonly ILogger<SentenceController> _logger;
		public SentenceController(ApplicationDbContext db, ILogger<SentenceController> logger) {
			_db = db;
			_logger = logger;
		}

		[HttpGet("listByCard")]
		public async Task<IActionResult> ListByCard(int cardId) {
			List<Sentence> sentences = await _db.Sentences.Where(s => s.WordId == cardId).ToListAsync();
			if (sentences.Count == 0) { return NoContent(); }
			sentences.Reverse();

			return new JsonResult(new { sentences });
		}

		[HttpPost("add")]
		public async Task<IActionResult> AddSentence([FromBody] SentenceInputModel input) {
			var card = await _db.CourseCards.FindAsync(input.WordId);
			if (card == null) { return BadRequest("Invalid card ID"); }

			Sentence sentence = new Sentence {
				Id = 0,
				Front = input.Front,
				Back = input.Back,
				WordId = input.WordId,
				Word = card
			};

			await _db.Sentences.AddAsync(sentence);
			await _db.SaveChangesAsync();

			return new JsonResult(new { sentence });
		}

		[HttpPost("delete")]
		public async Task<IActionResult> DeleteSentence(int id) {
			var sentence = await _db.Sentences.FindAsync(id);
			if (sentence == null) { return NotFound(); }

			_db.Sentences.Remove(sentence);
			await _db.SaveChangesAsync();

			return Ok();
		}
		[HttpPost("deleteAll")]
		public async Task<IActionResult> DeleteAllSentences(int cardId) {
			var list = await _db.Sentences.Where(s => s.WordId == cardId).ToListAsync();
			if (list.Count == 0) { return NoContent(); }

			_db.Sentences.RemoveRange(list);

			await _db.SaveChangesAsync();
			return Ok();
		}
	}
}
