namespace SpeakUpCSharp.Services {
	public interface IReviewDateService {
		DateTime CorrectAnswerGetReviewDate(int level);
		DateTime WrongAnswerGetReviewDate();
	}
}
