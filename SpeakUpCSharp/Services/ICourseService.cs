namespace SpeakUpCSharp.Services {
	public interface ICourseService {
		Task StartLearningCourse(int userId, string courseCode);
	}
}