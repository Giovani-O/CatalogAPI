using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CatalogAPI.Controllers
{
    [Route("api/teste")]
    [ApiController]
    [ApiVersion(3)]
    [ApiVersion(4)]
    public class TestV3Controller : ControllerBase
    {
        [MapToApiVersion(3)]
        [HttpGet]
        public string GetVersion3()
        {
            return "Version 3 - GET - API Version 3.0";
        }

        [MapToApiVersion(4)]
        [HttpGet]
        public string GetVersion4()
        {
            return "Version 4 - GET - API Version 4.0";
        }
    }
}
