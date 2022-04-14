using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using ServiceBusTutorial.Core.Services;

namespace ServiceBusTutorial.FunctionApp.Functions;

public class ContactUpdateNameFunction
{
    private readonly IContactService _contactService;
    private readonly ServiceBusClient _serviceBusClient;

    public ContactUpdateNameFunction(IContactService contactService, ServiceBusClient serviceBusClient)
    {
        _contactService = contactService;
        _serviceBusClient = serviceBusClient;
    }

    [FunctionName("ContactSetName")]
    public async Task<IActionResult> TriggerUpdateName(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "ContactName/{name}")] HttpRequest req,
        string name)
    {
        var sender = _serviceBusClient.CreateSender("updatenamefunction");
        var serviceBusMessage = new ServiceBusMessage(name);
        await sender.SendMessageAsync(serviceBusMessage);
        return new OkResult();
    }

    [FunctionName("ContactUpdateNameTrigger")]
    public void RunAsync(
        [ServiceBusTrigger("<functionname>", "<subscription>", Connection = "ServiceBusConnection")] string mySbMsg,
        ILogger log)
    {
        log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");

        _contactService.SetName(mySbMsg);
    }

    [FunctionName("ContactGetName")]
    public IActionResult GetContactServiceDemo(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "ContactName")] HttpRequest req)
    {
        return new OkObjectResult(_contactService.GetName());
    }
}