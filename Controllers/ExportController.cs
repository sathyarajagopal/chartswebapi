﻿using ChartsWebAPI.Models;
using ChartsWebAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web.Resource;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChartsWebAPI.Controllers
{
    [ApiVersion("1")]
    [ApiVersion("2")]
    [Route("/v{version:apiVersion}/export")]
    [ApiController]
    [Authorize]
    public class ExportController : ControllerBase
    {
        private readonly ILogger _logger;
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
        public IOptionsSnapshot<AppSettings> AppSettings { get; }

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

        // The web API will only accept tokens 1) for users, and 2) having the "access_as_user" scope for this API
        static readonly string[] scopeRequiredByApi = new string[] { "access_as_user" };

        public ExportController(IConfiguration configuration, IWebHostEnvironment env, IOptionsSnapshot<AppSettings> appSettings, ILogger<ExportController> logger)
        {
            Configuration = configuration;
            Environment = env;
            ProjectPath = env.ContentRootPath;
            WebRootPath = env.WebRootPath;
            _logger = logger;
            AppSettings = appSettings;
        }

        // GET api/v1/export/sample/external
        [Route("sample/{source}")]
        [MapToApiVersion("1")]
        [HttpGet]
        public async Task<ActionResult> SampleV1(string source)
        {
            //HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            string result = string.Empty;
            var response = new HttpResponseMessage();
            try
            {
                var appSettings = AppSettings.Value;
                object json = SerializationHelper.DeSerializeObject(sample);
                string exportServerUrl = source.Trim().ToLower() == "external" ? appSettings?.ExportServer?.Public : appSettings?.ExportServer?.Private;
                _logger.LogInformation($"Export server URL is {exportServerUrl}");
                _logger.LogInformation($"Incoming Request is {sample}");
                using (var handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                    using (var client = new HttpClient(handler))
                    {
                        response = await client.PostAsJsonAsync(exportServerUrl, json);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            result = response.Content.ReadAsStringAsync().Result;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical("Exception occurred. Message: {0} \n Stack trace: {1}", e.Message, e.StackTrace);
                throw e;
            }
            finally
            {
            }
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)response?.StatusCode,
                Content = result
            };
        }

        // GET api/v1/export/sample/external
        [Route("sample/{source}")]
        [MapToApiVersion("2")]
        [HttpGet]
        public async Task<ActionResult> SampleV2(string source)
        {
            string result = string.Empty;
            var response = new HttpResponseMessage();
            try
            {
                var appSettings = AppSettings.Value;
                object json = SerializationHelper.DeSerializeObject(sample);
                string exportServerUrl = source.Trim().ToLower() == "external" ? appSettings?.ExportServer?.Public : appSettings?.ExportServer?.Private;
                _logger.LogInformation($"Export server URL is {exportServerUrl}");
                _logger.LogInformation($"Incoming Request is {sample}");
                using (var handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                    using (var client = new HttpClient(handler))
                    {
                        response = await client.PostAsJsonAsync(exportServerUrl, json);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            result = response.Content.ReadAsStringAsync().Result;
                        }
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
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)response?.StatusCode,
                Content = result
            };
        }
    }
}
