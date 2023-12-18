using Microsoft.AspNetCore.Mvc;

namespace RainfallWebApi.Controllers
{
    public class RainfallController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
