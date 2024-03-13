using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpeakUp.Models;
using SpeakUpCSharp.Models;

namespace SpeakUpCSharp.Data {
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser,IdentityRole<int>,int> {
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<DailyPerformance> DailyPerformances { get; set; }
		public DbSet<Deck> Decks { get; set; }
		public DbSet<Section> Sections { get; set; }
		public DbSet<Card> Cards { get; set; }
		public DbSet<Sentence> Sentences { get; set; }
		public DbSet<CourseLink> CourseLinks { get; set; }
	}
}
