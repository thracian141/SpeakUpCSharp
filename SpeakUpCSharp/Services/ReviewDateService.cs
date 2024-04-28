using SpeakUpCSharp.Models;

namespace SpeakUpCSharp.Services {
	public class ReviewDateService : IReviewDateService {
		public DateTime CorrectAnswerGetReviewDate(int level) {
			DateTime currentTime = DateTime.UtcNow;
			if (level <= 0)
				currentTime.AddMinutes(5);
			else if (level <= 5)
				currentTime.AddMinutes(15);
			else if (level <= 10)
				currentTime.AddHours(3);
			else if (level <= 15)
				currentTime.AddHours(12);
			else if (level <= 20)
				currentTime.AddDays(1);
			else if (level <= 25)
				currentTime.AddDays(5);
			else if (level <= 30)
				currentTime.AddDays(10);
			else if (level <= 35)
				currentTime.AddDays(25);
			else if (level <= 40)
				currentTime.AddDays(35);
			else if (level <= 45)
				currentTime.AddDays(60);
			else if (level <= 50)
				currentTime.AddMonths(3);
			else
				currentTime.AddMonths(5); 
			return currentTime;
		}

		public DateTime WrongAnswerGetReviewDate() {
			DateTime currentTime = DateTime.UtcNow;
			currentTime.AddMinutes(7);
			return currentTime;
		}
	}
}
