namespace ConfigDemo.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using System.Collections.Generic;

    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly AppSettings _settings;

        public WeatherForecastController(IConfiguration configuration, IOptions<AppSettings> optionsAccs)
        {
            _configuration = configuration;

            _settings = optionsAccs.Value;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            var list = new List<string>();

            list.Add(_configuration["key1"]);
            list.Add(_configuration["key2"]);

            var sc1 = _configuration.GetSection("SC1");
            list.Add(sc1["key1"]);

            var sc2 = _configuration.GetSection("SC2");
            list.Add(sc2["key1"]);

            list.Add("=====");

            list.Add(_settings.key1);
            list.Add(_settings.key2);

            list.Add(_settings.SC1?.key1);
            list.Add(_settings.SC2?.key1);

            list.Add("=====");

            list.Add(_configuration["other"]);

            return list;
        }
    }
}
