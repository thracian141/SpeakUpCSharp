using SpeakUp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SpeakUpCSharp.Models.InputModels {
	public class CardInputModel {
		public string Front { get; set; }
		public string Back { get; set; }
		public int Difficulty { get; set; }
		public int DeckId { get; set; }
		public int SectionId { get; set; } //words dont necessarily have a sectionId
	}
}
