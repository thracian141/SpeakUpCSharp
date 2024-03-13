using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SpeakUpCSharp.Models.InputModels {
	public class SectionInputModel {
		public string Title { get; set; }
		public string? Description { get; set; }
		public string CourseCode { get; set; }
	}
}
