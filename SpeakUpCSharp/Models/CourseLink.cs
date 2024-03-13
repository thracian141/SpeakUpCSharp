using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SpeakUpCSharp.Models {
	public class CourseLink {
		[Key]
		public int Id { get; set; }
		[Required]
		public string CourseCode { get; set; }
		[Required]
		public int UserId { get; set; }
		[ForeignKey("UserId")]
		[JsonIgnore]
		public virtual ApplicationUser User { get; set; }
	}
}
