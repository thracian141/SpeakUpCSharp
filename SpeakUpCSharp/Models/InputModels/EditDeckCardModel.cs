﻿namespace SpeakUpCSharp.Models.InputModels {
	public class EditDeckCardModel {
		public int Id { get; set; }
		public string Front { get; set; }
		public string Back { get; set; }
		public int Difficulty { get; set; }
		public bool FlaggedAsImportant { get; set; }
	}
}
