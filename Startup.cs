using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ChartsMicroservice
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> logger)
        {
            _logger = logger;
            _logger.LogInformation($"Using appsettings.{env.EnvironmentName}.json");
            Configuration = configuration;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appSettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public IConfiguration Configuration { get; }
        private readonly ILogger _logger;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Charts Microservice",
                    Description = "An ASP.NET Web API solution for reading JSON file and generate charts at server side.",
                    TermsOfService = "None"
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                //c.OperationFilter<FormatXmlCommentProperties>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Charts Microservice V1");
            });
            app.UseMvc();
        }
    }

    /*
        //FormatXmlCommentProperties.cs
        public class FormatXmlCommentProperties : IOperationFilter
        {
            public void Apply(Operation operation, OperationFilterContext context)
            {
                operation.description = Formatted(operation.description);
                operation.summary = Formatted(operation.summary);
            }

            private string Formatted(string text)
            {
                if (text == null) return null;

                // Strip out the whitespace that messes up the markdown in the xml comments,
                // but don't touch the whitespace in <code> blocks. Those get fixed below.
                string resultString = Regex.Replace(text, @"(^[ \t]+)(?![^<]*>|[^>]*<\/)", "", RegexOptions.Multiline);
                resultString = Regex.Replace(resultString, @"<code[^>]*>", "<pre>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
                resultString = Regex.Replace(resultString, @"</code[^>]*>", "</pre>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
                resultString = Regex.Replace(resultString, @"<!--", "", RegexOptions.Multiline);
                resultString = Regex.Replace(resultString, @"-->", "", RegexOptions.Multiline);

                try
                {
                    string pattern = @"<pre\b[^>]*>(.*?)</pre>";

                    foreach (Match match in Regex.Matches(resultString, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline))
                    {
                        var formattedPreBlock = FormatPreBlock(match.Value);
                        resultString = resultString.Replace(match.Value, formattedPreBlock);
                    }
                    return resultString;
                }
                catch
                {
                    // Something went wrong so just return the original resultString
                    return resultString;
                }
            }

            private string FormatPreBlock(string preBlock)
            {
                // Split the <pre> block into multiple lines
                var linesArray = preBlock.Split('\n');
                if (linesArray.Length < 2)
                {
                    return preBlock;
                }
                else
                {
                    // Get the 1st line after the <pre>
                    string line = linesArray[1];
                    int lineLength = line.Length;
                    string formattedLine = line.TrimStart(' ', '\t');
                    int paddingLength = lineLength - formattedLine.Length;

                    // Remove the padding from all of the lines in the <pre> block
                    for (int i = 1; i < linesArray.Length - 1; i++)
                    {
                        linesArray[i] = linesArray[i].Substring(paddingLength);
                    }

                    var formattedPreBlock = string.Join("", linesArray);
                    return formattedPreBlock;
                }
            }
        }
        */
}
