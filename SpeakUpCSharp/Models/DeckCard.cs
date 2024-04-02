using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SpeakUp.Models;
using SpeakUpCSharp.Models.InputModels;

namespace SpeakUpCSharp.Models {
	public class DeckCard {
		[Key]
		public int Id { get; set; }
		[Required]
		public string Front { get; set; }
		[Required]
		public string Back { get; set; }
		[Required]
		public int Level { get; set; } = 0;
		[Required]
		public int Difficulty { get; set; } = 0;
		[Required]
		public bool FlaggedAsImportant { get; set; } = false;
		[Required]
		public DateTime DateAdded { get; set; }
		public DateTime? LastReviewDate { get; set; }
		public DateTime? NextReviewDate { get; set; }
		[Required]
		public int DeckId { get; set; }
		[ForeignKey("DeckId")]
		[JsonIgnore]
		public virtual Deck Deck { get; set; }
		public int? UserId { get; set; }
		[ForeignKey("UserId")]
		[JsonIgnore]
		public virtual ApplicationUser? User { get; set; }
	}
}
