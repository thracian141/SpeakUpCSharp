﻿namespace SpeakUpCSharp.Models.InputModels {
	public class RegisterInputModel {
		public string UserName { get; set; }
		public string? DisplayName { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string? CourseCode { get; set; }
	}
}
