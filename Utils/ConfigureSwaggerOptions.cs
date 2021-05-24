using ChartsWebAPI.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace ChartsWebAPI.Utils
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        private readonly AppSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IOptions<AppSettings> settings, IHttpClientFactory httpClientFactory)
        {
            _provider = provider;
            _settings = settings.Value;
            _httpClientFactory = httpClientFactory;
        }

        public void Configure(SwaggerGenOptions options)
        {
            var discoveryDocument = GetDiscoveryDocument();

            options.OperationFilter<AuthorizeOperationFilter>();
            options.DescribeAllParametersInCamelCase();
            options.CustomSchemaIds(x => x.FullName);

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }

            options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(discoveryDocument.AuthorizeEndpoint),
                        TokenUrl = new Uri(discoveryDocument.TokenEndpoint),
                        Scopes = new Dictionary<string, string>
                        {
                            { "User.Read", "https://graph.microsoft.com/User.Read" }
                        }
                    }
                },
                Description = "f684295a-2616-4263-86b7-7ce8f15f6b4e OpenId Security Scheme"
            });
        }

        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = "Charts API",
                Version = description.ApiVersion.ToString(),
                Description = "An ASP.NET Web API solution for reading JSON file and generate charts.",
                Contact = new OpenApiContact() { Name = "Sathya", Url = new Uri("https://github.com/sathyarajagopal/chartswebapi#readme") },
                License = new OpenApiLicense()
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }

        private DiscoveryDocumentResponse GetDiscoveryDocument()
        {
            return _httpClientFactory
                .CreateClient()
                .GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
                {
                    Address = _settings.AzureAd.Authority,
                    Policy =
                    {
                        ValidateIssuerName = false,
                        ValidateEndpoints = false,
                    },
                })
                .GetAwaiter()
                .GetResult();
        }
    }
}

