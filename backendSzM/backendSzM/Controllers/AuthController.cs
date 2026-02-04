using backendSzM.DTOs;
using backendSzM.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace backendSzM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static UserData user = new();

        [HttpPost("register")]
        public ActionResult<UserData> Register(UserDataDTO request)
        {
            var hashedPassword = new PasswordHasher<UserData>()
                .HashPassword(user, request.Password);
            user.Name=request.Name;
            user.Hash = hashedPassword;
            user.Gmail = request.Gmail;
            return Ok(user);
        }
    }
}
