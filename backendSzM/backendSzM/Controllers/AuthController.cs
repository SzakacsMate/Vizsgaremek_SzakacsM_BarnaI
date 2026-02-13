using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
            BannedUser ujBanned = new BannedUser();
            ujBanned.Warnings = 0;
            ujBanned.IsBanned = false;
            _context.BannedUsers.Add(ujBanned);
            _context.Users.Add(ujUserData);
            await _context.SaveChangesAsync();
            return Ok(new { ujUserData.Id, ujUserData.Name, ujUserData.Gmail, ujUserData.Role });

        }


        
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDataDTO request)
        {

            var user = _context.Users.FirstOrDefault(u => u.Gmail == request.Gmail && u.Name == request.Name);
            var CurrentBan = _context.BannedUsers.Where(x => x.Id == user.Id).FirstOrDefault();
           

            if (user == null)
            {
                return Unauthorized();
            }
            if (CurrentBan.IsBanned == true)
            {
                return Unauthorized("Ki vagy bannolva (womp womp)");
            }
            if (new PasswordHasher<UserData>().VerifyHashedPassword(user,user.Hash,request.Password)==PasswordVerificationResult.Failed)
            {
                return Unauthorized("Rossz jelszó");
            }
            var CurrentToken = _context.Tokens.Where(x => x.Id ==user.Id).FirstOrDefault();
            


            if (CurrentToken== null)
            { 
               var  newToken = new Token();
                newToken.Id = Guid.NewGuid();
                newToken.UserDataId =user.Id;
                CurrentToken = newToken;
                _context.Tokens.Add(newToken);
                await _context.SaveChangesAsync();
            }
            string accestoken = CreateToken(user);
            
            var refresh_token = new TokenResponseDto
            {
                AccesToken = accestoken,
                RefreshToken = await GenAndSaveRefreshTokenAsync(CurrentToken)
            };
          
            return Ok(refresh_token);
            
        }
        [HttpPost("refresh")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenReqDto request)
        {
            var result = await RefreshTokenAsync(request);
            if (result == null||result.AccesToken is null) { return Unauthorized("Invalid refresh token"); }
            return Ok(result);
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
        private async Task<Token> ValidateRefreshTokenAsync(Guid Id, string refreskToken)
        {
            var token = await _context.Tokens.FirstOrDefaultAsync(u => u.Id == Id);
            if (token == null || token.RefreshToken != refreskToken || token.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return null;
            }
            return token;
        }
        private string GenRefreshToken()
        {
            var randomN = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomN);
            return Convert.ToBase64String(randomN);
        }
        private async Task<string> GenAndSaveRefreshTokenAsync(Token token)
        {
            var refreshToken = GenRefreshToken();
            token.RefreshToken = refreshToken;
            token.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();
            return refreshToken;
        }
        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenReqDto request)
        {
            var token = await ValidateRefreshTokenAsync(request.Id, request.RefreshToken);
            if (token == null) return null;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (user == null) return null;
            var newAccesToken = CreateToken(user);
            var newRefreshToken = await GenAndSaveRefreshTokenAsync(token);
            return new TokenResponseDto
            {
                AccesToken = newAccesToken,
                RefreshToken = newRefreshToken
            };

        }
    }
}
