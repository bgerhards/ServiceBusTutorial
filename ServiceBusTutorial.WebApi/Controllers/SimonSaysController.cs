using Microsoft.AspNetCore.Mvc;

namespace ServiceBusTutorial.WebApi.Controllers;

public class SimonSaysController : ControllerBase
{
    [HttpGet("SimonSays")]
    public IActionResult SimonSays([FromQuery] string name)
    {
        return new OkObjectResult($"Simon says, {name}");
    }
}