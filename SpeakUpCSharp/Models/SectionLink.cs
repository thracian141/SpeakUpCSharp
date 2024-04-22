using SpeakUp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SpeakUpCSharp.Models {
	public class SectionLink {
		[Key]
		public int Id { get; set; }
		public int? SectionId { get; set; }
		[ForeignKey("SectionId")]
		public virtual Section? Section { get; set; }
		public int? UserId { get; set; }
		[ForeignKey("UserId")]
		[JsonIgnore]
		public virtual ApplicationUser? User { get; set; }
		[Required]
		public string CourseCode { get; set; }
		[Required]
		public int Order { get; set; }
		[Required]
		public bool Finished { get; set; } = false;
	}
}
