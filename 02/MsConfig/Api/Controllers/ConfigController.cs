namespace Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;

    [ApiController]
    [Route("[controller]")]
    public class ConfigController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get([FromQuery]string appName, [FromQuery]string env)
        {
            if (string.IsNullOrWhiteSpace(appName))
                return BadRequest("appName is empty");

            if (string.IsNullOrWhiteSpace(env))
                return BadRequest("env is empty");

            return Ok(ConfigResult.GetResult(appName, env));
        }

        public class ConfigResult
        {
            public int Code { get; set; }

            public string Msg { get; set; }

            public Dictionary<string, string> Data { get; set; }

            public static ConfigResult GetResult(string appName, string env)
            {
                var rd = new Random();

                var dict = new Dictionary<string, string>
                {
                    { "appName", appName },
                    { "env", env },
                    { "key1", $"val1-{rd.NextDouble()}" },
                    { "key2", $"val2-{rd.NextDouble()}" },
                    { "SC1__key1", $"sc1_val1-{rd.NextDouble()}" },
                    { "SC2:key1", $"sc2_val1-{rd.NextDouble()}" },
                };

                return new ConfigResult
                {
                    Code = 0,
                    Msg = "OK",
                    Data = dict
                };
            }
        }
    }
}
