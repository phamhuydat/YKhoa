using AutoMapper;
using Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(GenericRepository _repo, IMapper mapper) : base(_repo, mapper)
        {

        }
        public IActionResult Index()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statusCode)
        {
            return View(statusCode.ToString());
        }
    }
}
