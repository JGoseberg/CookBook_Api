using Microsoft.AspNetCore.Mvc;

namespace CookBook_Api.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class IsAliveController : ControllerBase
    {
        [HttpGet(Name = "ping")]
        [ActionName("isAlive")]
        public IActionResult Ping()
        {
            return Ok("new Version is there");
        }
    }
}
