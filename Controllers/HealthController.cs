using Microsoft.AspNetCore.Mvc;

namespace ChartsWebAPI.Controllers
{
    [ApiVersion("1")]
    [ApiVersion("2")]
    [Route("[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        [Route("/v{version:apiVersion}/health")]
        public ActionResult<string> Get()
        {
            return "Charts API is up and running.";
        }
    }
}
