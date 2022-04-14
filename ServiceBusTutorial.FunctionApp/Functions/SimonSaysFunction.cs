using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace ServiceBusTutorial.FunctionApp.Functions;

public class SimonSaysFunction
{
    private readonly ILogger<SimonSaysFunction> _logger;

    public SimonSaysFunction(ILogger<SimonSaysFunction> log)
    {
        _logger = log;
    }

    [FunctionName("SimonSays")]
    [OpenApiOperation("Run", "simonSays")]
    [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string),
        Description = "The **simonSays** parameter")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "text/plain", typeof(string), Description = "The OK response")]
    public async Task<IActionResult> SimonSays(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
        HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string simonSays = req.Query["simonSays"];

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        simonSays ??= data?.name;

        return new OkObjectResult($"Simon Says, {simonSays}. This HTTP triggered function executed successfully.");
    }
}