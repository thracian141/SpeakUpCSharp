﻿using SpeakUpCSharp.Models;
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
	public class Deck
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string DeckName { get; set; }
		[Required]
		public int Level { get; set; } = 0;
		[Required]
		public int Difficulty { get; set; } = 0;
		public string? DeckDescription { get; set; }
		[Required]
        public int OwnerId { get; set; }
		[ForeignKey("OwnerId")]
		[JsonIgnore]
		public virtual ApplicationUser Owner { get; set; }
	}
}
