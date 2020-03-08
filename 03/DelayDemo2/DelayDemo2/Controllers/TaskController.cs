namespace DelayDemo2.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/tasks")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskServices _svc;

        public TaskController(ITaskServices svc)
        {
            _svc = svc;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            await _svc.DoTaskAsync();
            return "done";
        }
    }
}
