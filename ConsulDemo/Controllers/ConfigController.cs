using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ConsulDemo.Controllers
{
    [Route("[controller]")]
    public class ConfigController : Controller
    {
        private readonly IConfigurationRoot _configuration;

        public ConfigController(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("{key}")]
        public IActionResult Get(string key)
        {
            return Ok(_configuration.GetValue<string>(key));
        }
    }
}