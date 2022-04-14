using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using ServiceBusTutorial.Core.Services;

namespace ServiceBusTutorial.WebApi.Background;

public interface IServiceBusTopicSubscription
    {
        Task PrepareFiltersAndHandleMessages();
        ValueTask DisposeAsync();
 
    }
 
    public class ServiceBusTopicSubscription : IServiceBusTopicSubscription
    {
        private readonly IContactService _contactService;
        private const string TopicPath = "updatenamefunction";
        private const string SubscriptionName = "S1";
        private readonly ILogger _logger;
        private readonly ServiceBusClient _client;
        private readonly ServiceBusAdministrationClient _adminClient;
        private ServiceBusProcessor _processor;
 
        public ServiceBusTopicSubscription(IContactService contactService, 
            IConfiguration configuration, 
            ILogger<ServiceBusTopicSubscription> logger)
        {
            _contactService = contactService;
            _logger = logger;
 
            var connectionString = configuration.GetConnectionString("ServiceBusConnection");
            _client = new ServiceBusClient(connectionString);
            _adminClient = new ServiceBusAdministrationClient(connectionString);
        }
 
        public async Task PrepareFiltersAndHandleMessages()
        {
            var serviceBusProcessorOptions = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false,
            };
 
            _processor = _client.CreateProcessor(TopicPath, SubscriptionName, serviceBusProcessorOptions);
            _processor.ProcessMessageAsync += ProcessMessagesAsync;
            _processor.ProcessErrorAsync += ProcessErrorAsync;

            await _processor.StartProcessingAsync().ConfigureAwait(false);
        }
 
        private async Task ProcessMessagesAsync(ProcessMessageEventArgs args)
        {
            Console.Write("I'm in here!");
            var myPayload = args.Message.Body.ToString();
            _contactService.SetName(myPayload);
            await args.CompleteMessageAsync(args.Message).ConfigureAwait(false);
        }
 
        private Task ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            _logger.LogError(arg.Exception, "Message handler encountered an exception");
            _logger.LogDebug($"- ErrorSource: {arg.ErrorSource}");
            _logger.LogDebug($"- Entity Path: {arg.EntityPath}");
            _logger.LogDebug($"- FullyQualifiedNamespace: {arg.FullyQualifiedNamespace}");
 
            return Task.CompletedTask;
        }
 
        public async ValueTask DisposeAsync()
        {
            await _client.DisposeAsync().ConfigureAwait(false);
        }
    }