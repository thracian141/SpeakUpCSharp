using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SpeakUp.Models
{
	public class CourseCard
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Front { get; set; }
        [Required]
        public string Back { get; set; }
		[Required]
		public int Difficulty { get; set; } = 0;
		[Required]
		public string PartOfSpeech { get; set; }
		[Required]
		public DateTime DateAdded { get; set; }
		[Required]
		public int SectionId { get; set; }
		[ForeignKey("SectionId")]
		[JsonIgnore]
		public virtual Section? Section { get; set; }
		[Required]
		public string CourseCode { get; set; }
	}
}