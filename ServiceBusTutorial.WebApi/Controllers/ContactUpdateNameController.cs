using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using ServiceBusTutorial.Core.Services;

namespace ServiceBusTutorial.WebApi.Controllers;

public class ContactUpdateNameController : Controller
{
    private const string TopicName = "updatenamefunction";
    private readonly ServiceBusClient _serviceBusClient;
    private readonly IContactService _contactService;

    public ContactUpdateNameController(ServiceBusClient serviceBusClient, IContactService contactService)
    {
        _serviceBusClient = serviceBusClient;
        _contactService = contactService;
    }

    [HttpGet("/ContactName")]
    public IActionResult GetName()
    {
        return new OkObjectResult(_contactService.GetName());
    }

    [HttpGet("/ContactName/{name}")]
    public async Task<IActionResult> Index(string name)
    {
        var serviceBusMessage = new ServiceBusMessage(name);
        var sender = _serviceBusClient.CreateSender(TopicName);
        await sender.SendMessageAsync(serviceBusMessage);
        return Ok();
    }
}