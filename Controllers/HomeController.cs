using ChartsWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
