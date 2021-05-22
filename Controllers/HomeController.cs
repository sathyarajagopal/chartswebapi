using ChartsMicroservice.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ChartsMicroservice.Controllers
{
    public class HomeController : ControllerBase
    {
        public IActionResult Index()
        {
            return Redirect("~/swagger");
        }
    }
}
