using SpeakUp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SpeakUpCSharp.Models {
	public class CardLink {
		[Key]
		public int Id { get; set; }
		[Required]
		public int CardId { get; set; }
		[ForeignKey("CardId")]
		[JsonIgnore]
		public virtual CourseCard Card { get; set; }
		public int? UserId { get; set; }
		[ForeignKey("UserId")]
		[JsonIgnore]
		public virtual ApplicationUser? User { get; set; }
		[Required]
		public int Level { get; set; } = 0;
		public DateTime? LastReviewDate { get; set; }
		public DateTime? NextReviewDate { get; set; }
		[Required]
		public bool FlaggedAsImportant { get; set; } = false;
		[Required]
		public string CourseCode { get; set; }

	}
}
