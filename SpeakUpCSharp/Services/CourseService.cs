using SpeakUpCSharp.Data;

namespace SpeakUpCSharp.Services {
	public class CourseService {
		private ApplicationDbContext _db;
        public CourseService(ApplicationDbContext db) {
            _db = db;
        }
    }
}
