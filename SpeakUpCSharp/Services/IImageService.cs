namespace SpeakUpCSharp.Services {
	public interface IImageService {
		Task<string> SaveProfilePictureReturnUrl(IFormFile imageFile);
		Task<FileStream> GetProfilePictureByUserId(int userId);
	}
}
