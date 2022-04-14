using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using ServiceBusTutorial.Core.Services;

[assembly: FunctionsStartup(typeof(ServiceBusTutorial.FunctionApp.Startup))]

namespace ServiceBusTutorial.FunctionApp;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var configuration = builder.GetContext().Configuration;
        builder.Services
            .AddHttpClient()
            .AddSingleton<IContactService, ContactService>();

        builder.Services.AddAzureClients(azureClientFactoryBuilder =>
        {
            azureClientFactoryBuilder.AddServiceBusClient(
                configuration.GetConnectionString("ServiceBusConnection"));
        });
    }
}