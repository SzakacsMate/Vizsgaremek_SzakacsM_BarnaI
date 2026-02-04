using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backendSzM.DTOs;
using backendSzM.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace backendSzM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IConfiguration configuration) : ControllerBase
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
        [HttpPost("login")]
        public ActionResult<string> Login(UserDataDTO request)
        {
            if (user.Name != request.Name || user.Gmail != request.Gmail)
            {
                return BadRequest("User not found");
            }
            if (new PasswordHasher<UserData>().VerifyHashedPassword(user, user.Hash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                return BadRequest("Wrond password");
            }
            string token= CreateToken(user);
            return Ok(token);
        }
        private string CreateToken(UserData user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Name)
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("Appsettings:Token")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("Appsettings:Issuer"),
                 audience: configuration.GetValue<string>("Appsettings:Audience"),
                 claims: claims,
                 expires: DateTime.UtcNow.AddDays(1),
                 signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
