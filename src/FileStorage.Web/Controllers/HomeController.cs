using Microsoft.AspNetCore.Mvc;

namespace FileStorage.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            // Just returning index.html to use angular on the client
            return View("index");
        }
    }
}
