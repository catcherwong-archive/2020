namespace SwaggerCusParamterDemo.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[] { "value1", "value2" };
        }

        [HttpPost]
        public string Post([FromBody]string value)
        {
            return value;
        }
    }
}
