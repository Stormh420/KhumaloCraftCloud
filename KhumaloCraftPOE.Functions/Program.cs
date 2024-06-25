using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask.ContextImplementations;
using Microsoft.Azure.WebJobs.Extensions.DurableTask.Options;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Add Durable Functions services
        services.AddSingleton<IDurableClientFactory, DurableClientFactory>();
        services.AddSingleton<IDurableOrchestrationClient>(serviceProvider =>
        {
            var clientFactory = serviceProvider.GetRequiredService<IDurableClientFactory>();
            return clientFactory.CreateClient(new DurableClientOptions());
        });
    })
    .Build();

host.Run();

