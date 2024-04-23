using SpeakUpCSharp.Models;

namespace SpeakUpCSharp.Services {
	public class ReviewDateService : IReviewDateService {
		public DateTime CorrectAnswerGetReviewDate(int level) {
			DateTime currentTime = DateTime.UtcNow;
			if (level <= 0)
				return currentTime.AddMinutes(5);
			else if (level <= 5)
				return currentTime.AddMinutes(15);
			else if (level <= 10)
				return currentTime.AddHours(3);
			else if (level <= 15)
				return currentTime.AddHours(12);
			else if (level <= 20)
				return currentTime.AddDays(1);
			else if (level <= 25)
				return currentTime.AddDays(5);
			else if (level <= 30)
				return currentTime.AddDays(10);
			else if (level <= 35)
				return currentTime.AddDays(25);
			else if (level <= 40)
				return currentTime.AddDays(35);
			else if (level <= 45)
				return currentTime.AddDays(60);
			else if (level <= 50)
				return currentTime.AddMonths(3);
			else
				return currentTime.AddMonths(6); 
		}

		public DateTime WrongAnswerGetReviewDate() {
			return DateTime.UtcNow.AddMinutes(7);
		}
	}
}
