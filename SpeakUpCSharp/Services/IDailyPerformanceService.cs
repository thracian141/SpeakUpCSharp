using SpeakUp.Models;
using SpeakUpCSharp.Models;

namespace SpeakUpCSharp.Services {
	public interface IDailyPerformanceService {
		Task<DailyPerformance> GetDailyPerformance(int userId);
		Task<DailyPerformance> GetDailyPerformance(ApplicationUser user);
		Task<DailyPerformance> CreateDailyPerformance(int userId);
		Task AddGuessedWord(int userId);
		Task AddNewWord(int userId);
	}
}
