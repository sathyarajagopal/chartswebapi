using ChartsMicroservice.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ChartsMicroservice.Controllers
{
    [ApiVersion("1")]
    [Route("/v{version:apiVersion}/phantomjs")]
    [ApiController]
    public class PhantomJsController : ControllerBase
    {
        private readonly ILogger _logger;
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

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

        private static readonly string sample = System.IO.File.ReadAllText(Path.GetFullPath(Path.Combine("Mocks", "basic.json")));

        public PhantomJsController(IConfiguration configuration, IWebHostEnvironment env, ILogger<ExportController> logger)
        {
            Configuration = configuration;
            Environment = env;
            ProjectPath = env.ContentRootPath;
            WebRootPath = env.WebRootPath;
            _logger = logger;
        }

        // GET phantomjs/v1/sample
        [Route("sample")]
        [HttpGet]
        public async Task<ProcessAsyncHelper.Result> Sample()
        {
            ProcessAsyncHelper.Result result = null;
            string svg = "";
            try
            {
                string command = FileHelper.GetFilePath("phantomjs.exe");
                string args = string.Format("highcharts-convert.js -infile {0} -constr Chart -type svg -resources resources.json", sample);
                string wd = System.IO.File.ReadAllText(Path.GetFullPath("phantomjs"));
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = @"C:\Users\sathya\Git\chartsmicroservice\phantomjs\phantomjs.exe";
                startInfo.Arguments = args;
                startInfo.WorkingDirectory = wd;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                Process process = new Process();
                process.StartInfo = startInfo;
                process.ErrorDataReceived += (a, b) =>
                {
                    Console.WriteLine(b.Data);
                };
                process.Start();
                process.BeginErrorReadLine();
                StreamReader rdr = process.StandardOutput;
                string line = "";
                while ((line = rdr.ReadLine()) != null)
                {
                    if (line.IndexOf("Exited with message ") > -1)
                    {
                        svg = line.Replace("Exited with message ", "").Replace("'", "");
                        break;
                    }
                }
                rdr.Close();
                rdr.Dispose();
                //result = await ProcessAsyncHelper.RunAsync(startInfo);
            }
            catch (Exception e)
            {
                _logger.LogCritical("Exception occurred. Message: {0} \n Stack trace: {1}", e.Message, e.StackTrace);
            }
            finally { }
            return result;
        }
    }
}
