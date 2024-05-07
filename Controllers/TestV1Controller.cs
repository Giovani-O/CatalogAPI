using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CatalogAPI.Controllers;

[Route("api/v{version:apiVersion}/test")]
[ApiController]
[ApiVersion("1.0", Deprecated = true)] // Define a versão da API
public class TestV1Controller : ControllerBase
{
    [HttpGet]
    public string GetVersion()
    {
        return "Test V1 - GET - API Version 1.0";
    }
}
