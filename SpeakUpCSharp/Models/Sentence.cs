using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpeakUp.Models
{
	public class Sentence
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Front { get; set; }
		[Required]
		public string Back { get; set; }
		[Required]
		public int WordId { get; set; }
		[ForeignKey("WordId")]
		[JsonIgnore]
		public virtual CourseCard? Word { get; set; }
    }
}
