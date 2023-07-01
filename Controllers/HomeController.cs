using Microsoft.AspNetCore.Mvc;

namespace HackApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        //public IActionResult Index()
        //{
        //    return Ok("Its Up!!");
        //}
        [HttpGet(Name = "Home")]
        public IActionResult Get()
        {
            return Ok("Its Up!!");

        }
    }
}
