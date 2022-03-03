using System;
using System.Net.Http;
using System.Net.Http.Headers;
using DEFRA.SGS.Listener;
using DEFRA.SGS.Listener.Models;
using DEFRA.SGS.Listener.Repositories;
using DEFRA.SGS.Listener.Repositories.Interfaces;
using DEFRA.SGS.Listener.Services;
using DEFRA.SGS.Listener.Services.Interfaces;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace DEFRA.SGS.Listener
{
    public class Startup : FunctionsStartup
    {

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var tenantId = Environment.GetEnvironmentVariable("TenantId");
            var authority = $"https://login.microsoftonline.com/{tenantId}";

            var config = new Config
            {
                BaseAddress = Environment.GetEnvironmentVariable("BaseAddress"),
                ClientId = Environment.GetEnvironmentVariable("ClientId"),
                Secret = Environment.GetEnvironmentVariable("Secret"),
                TenantId = tenantId,
                Authority = authority,
                Api = "/api/data/v9.2/"
            };

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.BaseAddress = new Uri($"{config.BaseAddress}{config.Api}");

            builder.Services.AddSingleton(config);
            builder.Services.AddSingleton(httpClient);
            builder.Services.AddSingleton<IDynamicsRepository, DynamicsRepository>();
            builder.Services.AddSingleton<ILogicService, LogicService>();
            builder.Services.AddSingleton<INotifyDynamicsRepository, NotifyDynamicsRepository>();
            builder.Services.AddSingleton<INotifyLogicService, NotifyLogicService>();
        }

    }
}
