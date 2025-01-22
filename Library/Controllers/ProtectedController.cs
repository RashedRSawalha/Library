using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProtectedController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetSecretData()
        {
            return Ok("This is protected data");
        }
    }

}