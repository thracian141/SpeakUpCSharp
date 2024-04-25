using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpeakUpCSharp.Data;
using SpeakUpCSharp.Models;
using SpeakUpCSharp.Services;

namespace SpeakUpCSharp.Controllers {
	[ApiController]
	[Route("dailyperformance")]
	public class DailyPerformanceController : ControllerBase {
		private readonly ApplicationDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IDailyPerformanceService _daily;
        private readonly ILogger<DailyPerformanceController> _logger;
        public DailyPerformanceController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
            IDailyPerformanceService daily, ILogger<DailyPerformanceController> logger)
        {
            _db = db;
            _userManager = userManager;
            _daily = daily;
            _logger = logger;
        }

        [HttpGet("today")]
        public async Task<IActionResult> GetToday() {
            var user = await _userManager.GetUserAsync(User);
			var todaysPerformance = await _daily.GetDailyPerformance(user.Id);
            if (todaysPerformance == null) return BadRequest("No performance found for today");

            return new JsonResult(new { todaysPerformance });
        }
        [HttpGet("streak")]
        public async Task<IActionResult> GetStreak() {
            var user = await _userManager.GetUserAsync(User);
            // New words learned, New words goal, Words guessed, Words guessed goal, Streak
            int[] values = new int[5]; 
            var dailyGoals = await _daily.GetDailyGoals(user.Id);
            var streak = await _daily.GetStreak(user.Id);
            values[0] = dailyGoals[0];
            values[1] = dailyGoals[1];
            values[2] = dailyGoals[2];
            values[3] = dailyGoals[3];
            values[4] = streak;

            return new JsonResult(new { values });
        }
        [HttpPost("changeDailyGoal")]
        public async Task<IActionResult> ChangeDailyGoal(int newGoal) {
            var user = await _userManager.GetUserAsync(User);
            user.DailyWordGoal = newGoal;
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
