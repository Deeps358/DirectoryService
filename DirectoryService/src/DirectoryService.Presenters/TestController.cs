using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presenters
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("TestClick")]
        public IActionResult TestClick()
        {
            return Ok("Ура ура работает");
        }
    }
}
