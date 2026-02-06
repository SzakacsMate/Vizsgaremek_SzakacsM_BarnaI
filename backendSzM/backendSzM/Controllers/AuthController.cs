using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using backendSzM.DTOs;
using backendSzM.Models;
using backendSzM.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace backendSzM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        public static UserData user = new();

        [HttpPost("register")]
        public async Task<ActionResult<UserData>> Register(UserDataDTO request)
        {
          var user = await authService.RegisterAsync(request);
            if (user is null)
            {
                return BadRequest("Username already exists");
            }

            return Ok(user);
            
        }
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDataDTO request)
        {
            var token = await authService.LoginAsync(request);
            if (token is null)
                return BadRequest("Invalid username or password or gmail");
            
            return Ok(token);
        }

    }
}
