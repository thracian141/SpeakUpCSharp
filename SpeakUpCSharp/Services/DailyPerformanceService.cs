using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpeakUp.Models;
using SpeakUpCSharp.Data;
using SpeakUpCSharp.Models;

namespace SpeakUpCSharp.Services {
	public class DailyPerformanceService : IDailyPerformanceService {
		private readonly ApplicationDbContext _db;
        public DailyPerformanceService(ApplicationDbContext db) {
            _db = db;
		}

        public async Task<DailyPerformance> GetDailyPerformance(int userId) {
			var user = await _db.ApplicationUsers.FindAsync(userId);
            var dailyPerformance = await _db.DailyPerformances
                .Where(dp => dp.UserId == userId && dp.Date.Date == DateTime.UtcNow.Date)
                .FirstOrDefaultAsync();
			if (dailyPerformance == null)
				dailyPerformance = await CreateDailyPerformance(userId);

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
    }
}
