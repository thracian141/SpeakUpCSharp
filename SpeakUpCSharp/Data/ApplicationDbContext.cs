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
		public DbSet<DeckCard> DeckCards { get; set; }
		public DbSet<Section> Sections { get; set; }
		public DbSet<SectionLink> SectionLinks { get; set; }
		public DbSet<CourseCard> CourseCards { get; set; }
		public DbSet<Sentence> Sentences { get; set; }
		public DbSet<CourseLink> CourseLinks { get; set; }
		public DbSet<CardLink> CardLinks { get; set; }
		public DbSet<BugReport> BugReports { get; set; }
	}
}
