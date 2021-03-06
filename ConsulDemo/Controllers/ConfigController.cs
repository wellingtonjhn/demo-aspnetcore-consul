﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ConsulDemo.Api.Controllers
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
            var value = _configuration.GetValue<string>(key);
            return Ok($"Setting key '{key}' has value '{value}'");
        }
    }
}