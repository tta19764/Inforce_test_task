using Microsoft.AspNetCore.Mvc;

namespace URLShortener.WebApp.Controllers
{
    public class UrlsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
