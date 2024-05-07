using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CatalogAPI.Controllers;

[Route("api/v{version:apiVersion}/test")]
[ApiController]
[ApiVersion("2.0")] // Define a versão da API
public class TestV2Controller : ControllerBase
{
    [HttpGet]
    public string GetVersion()
    {
        return "Test V2 - GET - API Version 2.0";
    }
}
