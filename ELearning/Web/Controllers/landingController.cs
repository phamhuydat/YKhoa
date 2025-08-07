using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class landingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
