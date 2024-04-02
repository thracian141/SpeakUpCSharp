using Microsoft.AspNetCore.Mvc;

namespace SpeakUpCSharp.Controllers {
	[ApiController]
	[Route("sentence")]
	public class SentenceController : Controller {

		public IActionResult Index() {
			return View();
		}
	}
}
