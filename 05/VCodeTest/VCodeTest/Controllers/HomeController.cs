namespace VCodeTest.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCode()
        {
            var (code, bytes) = VCodeHelper.GenVCode(4);

            // code handle logic
            System.Console.WriteLine(code);

            return File(bytes, "image/gif");
        }
    }
}
