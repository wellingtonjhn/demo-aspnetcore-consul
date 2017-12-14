using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsulDemo.Api.Controllers
{
    [Route("[Controller]")]
    public class HealthCheckController : Controller
    {
        [HttpGet, HttpHead, AllowAnonymous]
        public IActionResult Ping() => Ok("Api is online! ;)");
    }
}