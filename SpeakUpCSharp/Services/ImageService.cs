using SpeakUpCSharp.Data;

namespace SpeakUpCSharp.Services {
	public class ImageService : IImageService {
		private IWebHostEnvironment _env;
		private ApplicationDbContext _db;

		public ImageService(IWebHostEnvironment env,ApplicationDbContext db) {
			_env=env;
			_db=db;
		}
		public async Task<string> SaveProfilePictureReturnUrl(IFormFile imageFile) {
			string uniqueFileName = Guid.NewGuid().ToString()+"_"+imageFile.FileName;
			string filePath = Path.Combine(_env.WebRootPath,"ProfilePictures");
			string fullFilePath = Path.Combine(filePath,uniqueFileName);

			using (var stream = new FileStream(fullFilePath,FileMode.Create)) {
				await imageFile.CopyToAsync(stream);
			}

			return uniqueFileName;
		}

		public async Task<FileStream> GetProfilePictureByUserId(int userId) {
			var user = await _db.ApplicationUsers.FindAsync(userId);
			string filePath = Path.Combine(_env.WebRootPath,"ProfilePictures");

			if (user.ProfilePictureUrl == null) return null;

			string imgUrl = Path.Combine(filePath,user.ProfilePictureUrl);
			var image = System.IO.File.OpenRead(imgUrl);

			return image;
		}
	}
}
