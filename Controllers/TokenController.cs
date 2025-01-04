using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Ensure this controller requires authentication
    public class TokenController : ControllerBase
    {
        private readonly ILogger<TokenController> _logger;

        public TokenController(ILogger<TokenController> logger)
        {
            _logger = logger;
        }

        // A simple GET endpoint that will require JWT Authentication
        [HttpGet("test")]
        public IActionResult Get()
        {
            // This will be a secured endpoint that only allows authenticated users
            var user = User.Identity?.Name; // Retrieve the name of the user from the JWT
            return Ok(new { message = "This is a secure endpoint.", user });
        }

        // Another example endpoint
        [HttpGet("info")]
        public IActionResult GetInfo()
        {
            var userId = User.Identity?.Name; // Extract information from the JWT token if needed
            return Ok(new { user = userId, message = "Information retrieved successfully" });
        }
    }
}
