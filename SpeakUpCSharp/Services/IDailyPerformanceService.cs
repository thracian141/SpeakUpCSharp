using SpeakUp.Models;
using SpeakUpCSharp.Models;

namespace SpeakUpCSharp.Services {
	public interface IDailyPerformanceService {
		Task<int[]> GetDailyGoals(int userId);
		Task<int> GetStreak(int userId);
		Task<DailyPerformance> GetDailyPerformance(int userId);
		Task<DailyPerformance> CreateDailyPerformance(int userId);
		Task AddGuessedWord(int userId);
		Task AddNewWord(int userId);
	}
}
