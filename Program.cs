using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TestBoard.API.Data;
using TestBoard.API.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(worker => worker.UseNewtonsoftJson())
    .ConfigureServices(services =>
    {
        services.AddDbContext<TaskBoardDbContext>(options => options.UseInMemoryDatabase("TaskBoard"));
        services.AddScoped<ITaskService, TaskService>();
        services.AddSingleton<IOpenApiConfigurationOptions>(_ => new OpenApiConfigurationOptions
        {
            Info = new OpenApiInfo
            {
                Version = "v1",
                Title = "TestBoard API",
                Description = "Azure Functions backend for a JIRA-style task board application.",
                Contact = new OpenApiContact
                {
                    Name = "TestBoard API"
                }
            },
            Servers = DefaultOpenApiConfigurationOptions.GetHostNames(),
            OpenApiVersion = DefaultOpenApiConfigurationOptions.GetOpenApiVersion(),
            IncludeRequestingHostName = DefaultOpenApiConfigurationOptions.IsFunctionsRuntimeEnvironmentDevelopment(),
            ForceHttps = DefaultOpenApiConfigurationOptions.IsHttpsForced(),
            ForceHttp = DefaultOpenApiConfigurationOptions.IsHttpForced()
        });
    })
    .ConfigureOpenApi()
    .Build();

await TaskBoardSeedData.SeedAsync(host.Services);
await host.RunAsync();
