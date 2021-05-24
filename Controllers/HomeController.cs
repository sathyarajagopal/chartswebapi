using Microsoft.AspNetCore.Mvc;

namespace ChartsWebAPI.Controllers
{
    public class HomeController : ControllerBase
    {
        public IActionResult Index()
        {
            return Redirect("~/swagger");
        }
    }
}
