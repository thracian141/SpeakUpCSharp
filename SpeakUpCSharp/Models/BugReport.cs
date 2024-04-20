using SpeakUp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpeakUpCSharp.Models {
	public class BugReport {
		[Key]
		public int Id { get; set; }
		[Required]
		public string Text { get; set; }
		[Required]
		public string CourseCode { get; set; }
		[Required]
		public int ReporterId { get; set; }
		[ForeignKey("ReporterId")]
		public virtual ApplicationUser Reporter { get; set; }
		public int? CardId { get; set; }
		[ForeignKey("CardId")]
		public virtual CourseCard? Card { get; set; }
	}
}
