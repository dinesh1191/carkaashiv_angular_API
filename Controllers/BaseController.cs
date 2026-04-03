using carkaashiv_angular_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace carkaashiv_angular_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected int CurrentUserId
        {
            get
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    throw new UnauthorizedAccessException("Invalid token");

                return int.Parse(userIdClaim);
            }
        }
    }
}
