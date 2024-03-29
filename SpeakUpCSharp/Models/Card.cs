﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SpeakUp.Models
{
	public class Card
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Front { get; set; }
        [Required]
        public string Back { get; set; }
		[Required]
		public int Level { get; set; } = 0;
		[Required]
		public int Difficulty { get; set; } = 0;
		[Required]
		public DateTime DateAdded { get; set; }
        public DateTime? LastReviewDate { get; set; }
        public DateTime? NextReviewDate { get; set; }
        public int? DeckId { get; set; }
        [ForeignKey("DeckId")]
		[JsonIgnore]
        public virtual Deck Deck { get; set; }
		public int? SectionId { get; set; } //words dont necessarily have a sectionId
		[ForeignKey("SectionId")]
		[JsonIgnore]
		public virtual Section? Section { get; set; }
	}
}