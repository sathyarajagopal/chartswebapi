using ChartsWebAPI.Models;
using ChartsWebAPI.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChartsWebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            _logger = logger;
            Configuration = configuration;
            Environment = env;
            _logger.LogInformation($"Using appsettings.{env.EnvironmentName}.json");
        }

        public IConfiguration Configuration { get; }
        public readonly IWebHostEnvironment Environment;
        private readonly ILogger _logger;

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration);
            services.AddControllers().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });
            services.AddVersionedApiExplorer(options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });
            services.AddHttpClient();
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddScoped(sp => sp.GetRequiredService<IOptionsSnapshot<AppSettings>>().Value);
            services.AddMicrosoftIdentityWebApiAuthentication(Configuration, "AzureAd");
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(options =>
            //{
            //    Configuration.Bind($"{nameof(AppSettings.AzureAd)}", options);
            //});
            //string identityUrl = Configuration.GetSection("appSettings").GetSection("AuthorityUrl").Value;
            //string callBackUrl = Configuration.GetSection("appSettings").GetSection("CallBackUrl").Value;
            //var sessionCookieLifetime = Configuration.GetValue("SessionCookieLifetimeMinutes", 60);
            //.AddCookie(setup => setup.ExpireTimeSpan = TimeSpan.FromMinutes(sessionCookieLifetime));
            //.AddOpenIdConnect(options =>
            //{
            //    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.Authority = identityUrl.ToString();
            //    options.SignedOutRedirectUri = callBackUrl.ToString();
            //    options.ClientId = Configuration.GetSection("appSettings").GetSection("ClientId").Value;
            //    options.ClientSecret = Configuration.GetSection("appSettings").GetSection("Secret").Value;
            //    options.CallbackPath = callBackUrl;
            //    options.ResponseType = "code id_token token";
            //    options.SaveTokens = true;
            //    options.GetClaimsFromUserInfoEndpoint = true;
            //    options.RequireHttpsMetadata = false;
            //    options.Scope.Add("openid");
            //    options.Scope.Add("profile");
            //});
            //.AddMicrosoftAccount(microsoftOptions =>
            //    {
            //        microsoftOptions.ClientId = Configuration["Authentication:Microsoft:ClientId"];
            //        microsoftOptions.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
            //    })
            //.AddGoogle(googleOptions => { })
            //.AddFacebook(facebookOptions => { })
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            var appSettings = Configuration.Get<AppSettings>();
            if (env.IsDevelopment())
            {
                _logger.LogInformation("In Local environment");
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/error-local-development");
            }
            else
            {
                _logger.LogInformation($"In {Environment.EnvironmentName} environment");
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseStaticFiles();
            app.UseCors(x =>
            {
                x.AllowAnyMethod()
                .AllowAnyHeader()
                .WithOrigins("http://localhost:5000", "https://localhost:5001", "https://chartswebapi.azurewebsites.net");
            });
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    options.OAuthClientId(appSettings?.AzureAd?.ClientId);
                    options.OAuthScopeSeparator(" ");
                    options.OAuthAppName("Charts API");
                    options.OAuthUsePkce();
                }
            });
            //app.UseHttpsRedirection();
            app.UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapDefaultControllerRoute();
                    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                });
        }
    }
}
