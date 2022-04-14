using Microsoft.Extensions.Azure;
using ServiceBusTutorial.Core.Services;
using ServiceBusTutorial.WebApi.Background;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddSingleton<IContactService, ContactService>()
    .AddSingleton<IServiceBusTopicSubscription, ServiceBusTopicSubscription>()
    .AddAzureClients(azureClientFactoryBuilder =>
    {
        azureClientFactoryBuilder.AddServiceBusClient(
            configuration.GetConnectionString("ServiceBusConnection"));
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
var busSubscription = app.Services.GetService<IServiceBusTopicSubscription>();
busSubscription.PrepareFiltersAndHandleMessages().GetAwaiter().GetResult();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();