using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpeakUpCSharp.Data;
using SpeakUpCSharp.Models;

namespace SpeakUpCSharp.Utilities {
	public class DbInitializer : IDbInitializer {
		private readonly ApplicationDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole<int>> _roleManager;

		public DbInitializer(ApplicationDbContext db,UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole<int>> roleManager) {
			_db=db;
			_roleManager=roleManager;
			_userManager=userManager;
		}

		public void Initialize() {
			if (_db.Roles.Any(x => x.Name==ApplicationRoles.Admin))
				return;

			_roleManager.CreateAsync(new IdentityRole<int>(ApplicationRoles.Admin)).GetAwaiter().GetResult();
			_roleManager.CreateAsync(new IdentityRole<int>(ApplicationRoles.Dev)).GetAwaiter().GetResult();
			_roleManager.CreateAsync(new IdentityRole<int>(ApplicationRoles.User)).GetAwaiter().GetResult();

			_userManager.CreateAsync(new ApplicationUser {
				UserName="sysadmin",
				Email="admin@gmail.com",
				EmailConfirmed=true

			},"Parola123?").GetAwaiter().GetResult();

			var user = _db.Users.FirstOrDefaultAsync(u => u.Email=="admin@gmail.com").GetAwaiter().GetResult();
			_userManager.AddToRoleAsync(user,ApplicationRoles.Admin).GetAwaiter().GetResult();
		}
	}
}
