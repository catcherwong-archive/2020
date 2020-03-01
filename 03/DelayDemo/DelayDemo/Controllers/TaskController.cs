namespace DelayDemo.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

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
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} done here");
            return "done";
        }
    }
}
