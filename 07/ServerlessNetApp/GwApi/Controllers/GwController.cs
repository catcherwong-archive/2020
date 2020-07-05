using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace GwApi.Controllers
{
    [ApiController]
    [Route("")]
    public class GwController : ControllerBase
    {
        private readonly IHttpClientFactory _factory;

        public GwController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        public string Get()
        {
            return $"gw-svc in k8s";
        }

        [HttpGet("svc")]
        public async Task<string> GetAsync()
        {
            var client = _factory.CreateClient();

            var resp = await client.GetAsync("http://api-nfx-svc/api/values");
            resp.EnsureSuccessStatusCode();
            var res = await resp.Content.ReadAsStringAsync();

            return $"ok - {res}";
        }
    }
}
