using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using backendSzM.Data;
using backendSzM.DTOs;
using backendSzM.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace backendSzM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
       private readonly UserDataDBContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(UserDataDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        
        [HttpPost("register")]
        public  async Task<IActionResult> Register(UserDataDTO request)
        {
            var user = _context.Users.FirstOrDefault();
            if (await _context.Users.AnyAsync(u => u.Name == request.Name))
            {
                return BadRequest();
            }
            UserData ujUserData=new UserData();
            var hashedPassword=new PasswordHasher<UserData>()
                .HashPassword(ujUserData,request.Password);
            ujUserData.Name=request.Name;
            ujUserData.Hash = hashedPassword;
            ujUserData.Gmail=request.Gmail;
            ujUserData.Role=request.Role;
            _context.Users.Add(ujUserData);
            await _context.SaveChangesAsync();
            return Ok(user);
            
        }
        public async Task<IActionResult>GetTokens(TokenDTO request)
        {
            Token ujToken=new Token();
            ujToken.RefreshToken=request.RefreshToken;
            ujToken.RefreshTokenExpiryTime=request.RefreshTokenExpiryTime;
            _context.Tokens.Add(ujToken);
            ujToken.Id=request.
            await _context.SaveChangesAsync();
            
        }
        private string GenRefreshToken()
        {

        }
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDataDTO request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Gmail == request.Gmail && u.Name == request.Name);
            if (user == null)
            {
                return Unauthorized();
            }
            if(new PasswordHasher<UserData>().VerifyHashedPassword(user,user.Hash,request.Password)==PasswordVerificationResult.Failed)
            {
                return Unauthorized("Rossz jelszó");
            }
            string token = CreateToken(user);
            return Ok(new { access_token = token });
            
        }
        private string CreateToken(UserData user) {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Name),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Role,user.Role)
            };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Appsettings:Token")!));
            var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new JwtSecurityToken(
                issuer:_configuration.GetValue<string>("Appsettings:Issuer"),
                audience: _configuration.GetValue<string>("Appsettings:Audience"),
                claims:claims,
                expires:DateTime.UtcNow.AddDays(1),
                signingCredentials:creds
                );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
       
        [Authorize(Roles ="User,Admin")]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            return Ok("You are authenticated!");
        }
        [Authorize(Roles ="Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are an admin!");
        }
    }
}
