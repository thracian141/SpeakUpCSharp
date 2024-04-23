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
            _logger.LogInformation("GET TODAY CALLED !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            var user = await _userManager.GetUserAsync(User);
            var todaysPerformance = await _daily.GetDailyPerformance(user.Id);
            _logger.LogInformation("NO NO NO NO NO NO NO");
            if (todaysPerformance == null) return BadRequest("No performance found for today");

            return new JsonResult(new { todaysPerformance });
        }
    }
}
