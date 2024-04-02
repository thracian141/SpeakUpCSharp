using SpeakUp.Models;
using System.Text.Json.Serialization;

namespace SpeakUpCSharp.Models.InputModels {
	public class DeckCardInput {
		public string Front { get; set; }
		public string Back { get; set; }
		public int Level { get; set; } = 0;
		public int Difficulty { get; set; } = 0;
		public bool FlaggedAsImportant { get; set; } = false;
		public DateTime DateAdded { get; set; }
		public DateTime? LastReviewDate { get; set; }
		public DateTime? NextReviewDate { get; set; }
		public int DeckId { get; set; }
		public int UserId { get; set; }
	}
}
