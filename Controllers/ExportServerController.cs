using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace ChartsMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportServerController : ControllerBase
    {
        private readonly ILogger _logger;
        public IConfiguration Configuration { get; }

        private static string _projectPath;
        public static string ProjectPath
        {
            set { _projectPath = value; }
            get { return _projectPath; }
        }

        private static string _webRootPath;
        public static string WebRootPath
        {
            set { _webRootPath = value; }
            get { return _webRootPath; }
        }

        public ExportServerController(IConfiguration configuration, IHostingEnvironment env, ILogger<ExportServerController> logger)
        {
            ProjectPath = env.ContentRootPath;
            WebRootPath = env.WebRootPath;
            Configuration = configuration;
            _logger = logger;
        }

        // GET api/exportserver
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            string result = string.Empty;
            var response = new HttpResponseMessage();
            try
            {
                using (var client = new HttpClient())
                {
                    //string chartServerUrl = ConfigSection.GetSection("appSettings").GetSection("ChartServerUrl").Value;
                    string chartServerUrl = "http://172.17.0.2:7801";
                    string route = chartServerUrl + "/health";
                    _logger.LogInformation("Chart export server request url is {0}", route);
                    response = await client.GetAsync(route);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        result = response.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical("Exception occurred. Message: {0} \n Stack trace: {1}", e.Message, e.StackTrace);
            }
            finally
            {

            }
            return Ok(result);
        }

        // POST api/exportserver
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
    }
}
