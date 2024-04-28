using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpeakUp.Models;
using SpeakUpCSharp.Data;
using SpeakUpCSharp.Models;

namespace SpeakUpCSharp.Services {
	public class DailyPerformanceService : IDailyPerformanceService {
		private readonly ApplicationDbContext _db;
        private readonly ILogger<DailyPerformanceService> _logger;
        public DailyPerformanceService(ApplicationDbContext db, ILogger<DailyPerformanceService> logger) {
            _db = db;
            _logger = logger;
		}

        public async Task<DailyPerformance> GetDailyPerformance(int userId) {
            _logger.LogInformation("SERVICE REACHED");
			var user = await _db.ApplicationUsers.FindAsync(userId);
            _logger.LogInformation("USER FOUND: " + user.Id);
            var dailyPerformance = await _db.DailyPerformances
                .Where(dp => dp.UserId == userId && dp.Date.Date == DateTime.UtcNow.Date)
                .FirstOrDefaultAsync();
            _logger.LogInformation("DAILY PERFORMANCE PASSED");
			if (dailyPerformance == null)
				dailyPerformance = await CreateDailyPerformance(userId);
            _logger.LogInformation("DAILY PERFORMANCE CREATED");

            return dailyPerformance;
		}

        public async Task<DailyPerformance> CreateDailyPerformance(int userId) {
            var user = await _db.ApplicationUsers.FindAsync(userId);
            DailyPerformance dailyPerformance = new DailyPerformance {
                User = user,
                UserId = user.Id,
                Date = DateTime.UtcNow.Date
            };
            await _db.DailyPerformances.AddAsync(dailyPerformance);
            await _db.SaveChangesAsync();
            return dailyPerformance;
        }

        public async Task AddGuessedWord(int userId) {
            var dailyPerformance = await GetDailyPerformance(userId);

            dailyPerformance.WordsGuessedCount += 1;
            await _db.SaveChangesAsync();
        }
        public async Task AddNewWord(int userId) {
            var dailyPerformance = await GetDailyPerformance(userId);

            dailyPerformance.NewWords += 1;
            await _db.SaveChangesAsync();
		}

        public async Task<int[]> GetDailyGoals(int userId) {
			// New words learned, New words goal, Words guessed, Words guessed goal
			int[] values = { 0, 0, 0, 0 };
            var user = await _db.ApplicationUsers.FindAsync(userId);
            var dailyPerformance = await GetDailyPerformance(userId);
            if (dailyPerformance != null) {
                values[0] = dailyPerformance.NewWords;
                values[2] = dailyPerformance.WordsGuessedCount;
            }
			values[1] = user.DailyWordGoal;
			values[3] = user.DailyWordGoal * 5;

            return values;
		}

        public async Task<int> GetStreak(int userId) {
            var user = await _db.ApplicationUsers.FindAsync(userId);
            int dailyGoal = user.DailyWordGoal;
			int streak = 0;
			DateTime dateIndex = DateTime.UtcNow.AddDays(-1);

            while (true) {
                var performance = await _db.DailyPerformances
                    .Where(dp => dp.UserId == userId && dp.Date.Date == dateIndex.Date)
                    .FirstOrDefaultAsync();
                if (performance == null || performance.NewWords < user.DailyWordGoal) 
                    break;
                else { 
                    streak += 1;
                    dateIndex = dateIndex.AddDays(-1);
				}
			}

            return streak;
		}
	}
}
