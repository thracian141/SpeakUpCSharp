using SpeakUp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SpeakUpCSharp.Models.InputModels {
	public class CourseCardInput {
		public string Front { get; set; }
		public string Back { get; set; }
		public int Difficulty { get; set; }
		public string PartOfSpeech { get; set; }
		public int SectionId { get; set; } 
		public string CourseCode { get; set; }
	}
}
