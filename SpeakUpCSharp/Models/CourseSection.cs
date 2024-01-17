using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakUp.Models
{
	public class CourseSection
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public int Level { get; set; } = 0;
		public DateTime? LastReviewDate { get; set; }
		public DateTime? NextReviewDate { get; set; }
		[Required]
		public int DeckId { get; set; }
		[ForeignKey("DeckId")]
		public virtual Deck Deck { get; set; }
	}
}
