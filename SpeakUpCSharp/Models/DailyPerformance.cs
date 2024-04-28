using SpeakUpCSharp.Models;
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
	public class DailyPerformance
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public DateTime Date { get; set; }
		[Required]
		public int WordsGuessedCount { get; set; } = 0;
        [Required]
        public int MinutesSpentLearning { get; set; } = 0;
		[Required]
		public int NewWords { get; set; } = 0;
        [Required]
        public int UserId { get; set; }
		[ForeignKey("UserId")]
		[JsonIgnore]
		public virtual ApplicationUser User { get; set; }
	}
}
