using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ChartsMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        // GET api/charts
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Charts API is up and running.";
        }
    }
}
