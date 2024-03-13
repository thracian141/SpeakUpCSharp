using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SpeakUpCSharp.Models {
	public class ApplicationUser : IdentityUser<int> {
		[Key]
		public override int Id { get; set; }
		[Required]
		public override string UserName { get; set; }
		public string? DisplayName { get; set; }
		public string? ProfilePictureUrl { get; set; }
		[Required]
		public DateTime AccountCreatedDate { get; set; }
		public int? LastDeck { get; set; }
		public string? LastCourse { get; set; }
	}
}
