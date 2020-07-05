namespace NfxApi.Controllers
{
    using System.Collections.Generic;
    using System.Web.Http;

    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2", "nfx in k8s" };
        }
    }
}
