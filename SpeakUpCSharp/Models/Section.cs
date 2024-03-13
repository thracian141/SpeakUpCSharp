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
	public class Section
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Title { get; set; }
		public string? Description { get; set; }
		[Required]
		public DateTime LastEdited { get; set; }
		[Required]
		public string CourseCode { get; set; }
		[Required]
		public int Order { get; set; }
		[Required]
		public int LastEditorId { get; set; }
		[ForeignKey("LastEditorId")]
		[JsonIgnore]
		public virtual ApplicationUser LastEditor { get; set; }
	}
}
