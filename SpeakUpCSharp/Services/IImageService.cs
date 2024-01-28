namespace SpeakUpCSharp.Services {
	public interface IImageService {
		Task<string> SaveProfilePictureReturnUrl();
		Task<FileStream> GetProfilePicture(int questionId);
	}
}
