namespace ConfigDemo.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;

    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppSettings _settings;
        private readonly AppSettings _sSettings;
        private readonly AppSettings _mSettings;

        public WeatherForecastController(
            IConfiguration configuration,
            IOptions<AppSettings> options,
            IOptionsSnapshot<AppSettings> sOptions,
            IOptionsMonitor<AppSettings> _mOptions
            )
        {
            _configuration = configuration;
            _settings = options.Value;
            _sSettings = sOptions.Value;
            _mSettings = _mOptions.CurrentValue;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            var list = new List<string>();

            Console.WriteLine($"===============================================");

            var other = _configuration["other"];
            Console.WriteLine($"other = {other}");

            var appName = _configuration["appName"];
            var env = _configuration["env"];
            var key1 = _configuration["key1"];
            var SC1key1 = _configuration["SC1__key1"];
            var SC2key1 = _configuration["SC2:key1"];

            Console.WriteLine($"appName={appName},env={env},key1={key1},SC1key1={SC1key1},SC2key1={SC2key1}");

            var str1 = Newtonsoft.Json.JsonConvert.SerializeObject(_settings);
            Console.WriteLine($"IOptions");
            Console.WriteLine($"{str1}");

            var str2 = Newtonsoft.Json.JsonConvert.SerializeObject(_sSettings);
            Console.WriteLine($"IOptionsSnapshot");
            Console.WriteLine($"{str2}");

            var str3 = Newtonsoft.Json.JsonConvert.SerializeObject(_mSettings);
            Console.WriteLine($"IOptionsMonitor");
            Console.WriteLine($"{str3}");

            Console.WriteLine($"===============================================");
            Console.WriteLine("");

            return list;
        }
    }
}
