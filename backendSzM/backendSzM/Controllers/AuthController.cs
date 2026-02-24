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
            var banned=_context.BannedUsers.FirstOrDefault(x=>x.BannedGmail==request.Gmail);
            if (await _context.Users.AnyAsync(u => u.Name == request.Name))
            {
                return BadRequest("Már van ilyen felhasználó");
            }
            if(banned!=null)
            {
                return Unauthorized("Ez a Gmail ki van bannolva");
            }
            UserData ujUserData=new UserData();
            var hashedPassword=new PasswordHasher<UserData>()
                .HashPassword(ujUserData,request.Password);
            ujUserData.Name=request.Name;
            ujUserData.Hash = hashedPassword;
            ujUserData.Gmail=request.Gmail;
            ujUserData.Role=request.Role;
            ujUserData.Rep = 0;
            _context.Users.Add(ujUserData);
            await _context.SaveChangesAsync();
            return Ok(new { ujUserData.Id, ujUserData.Name, ujUserData.Gmail, ujUserData.Role,ujUserData.Rep });

        }


        
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDataDTO request)
        {

            var user = _context.Users.FirstOrDefault(u => u.Gmail == request.Gmail && u.Name == request.Name);
            
           

            if (user == null)
            {
                return Unauthorized();
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
        [HttpPatch("Change Role")]
        public async Task<IActionResult>ChangeRole(RoleDTO role,Guid Id,Guid Id2)
        {
            var changedUser = _context.Users.FirstOrDefault(x => x.Id ==Id);
            var roleChanger = _context.Users.FirstOrDefault(x=>x.Id ==Id2&& role.Role=="Admin");
            if (changedUser == null)
            {
                return BadRequest("Nincs ilyen felhasználó");
            }
            if (roleChanger.Role != "Admin")
            {
                return Unauthorized("Ehhez nincs jogod!");
            }
            changedUser.Role = role.Role;
            _context.Users.Update(changedUser);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost("Create Lobby")]
        public async Task<IActionResult> CreateLobby(LobbyDTO request)
        {
            var user = _context.Users.FirstOrDefault(x=>x.Name==request.Dm);

            if (user == null)
            {
              return BadRequest("Nincs ilyen felhasználó");
            }
            Lobby ujLobby = new Lobby();
            ujLobby.Id = Guid.NewGuid();
            ujLobby.Dm = user.Name;
            ujLobby.Location = request.Location;
            ujLobby.TimeDate = request.TimeDate;
            ujLobby.PlayerLimit = request.PlayerLimit;
            ujLobby.TtType = request.TtType;
            ujLobby.Image = request.Image;
            if (ujLobby.Image == null)
            {
                ujLobby.Image = "N/A";
            }
            LobbyCon newLobbyCon = new LobbyCon();
            newLobbyCon.Id=Guid.NewGuid();
            newLobbyCon.UserDataId = user.Id;
            newLobbyCon.LobbyId=ujLobby.Id;
            
            _context.Lobbies.Add(ujLobby);
            await _context.SaveChangesAsync();
            _context.LobbyCons.Add(newLobbyCon);
            await _context.SaveChangesAsync();
            return Ok(new());
        }
        [HttpPost("AddPlayer")]
        public async Task<IActionResult> AddPlayer(JoinLobbyDTO request)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == request.PlayerId);
            var lobby=_context.Lobbies.FirstOrDefault(x=>x.Id == request.LobbyId);
            LobbyCon newLobbyCon = new LobbyCon();
            newLobbyCon.Id = Guid.NewGuid();
            newLobbyCon.UserDataId = user.Id;
            newLobbyCon.LobbyId = lobby.Id;
            _context.LobbyCons.Add(newLobbyCon);
            await _context.SaveChangesAsync();
            return Ok(new());
        }

        [HttpPost("WriteComment")]
        public async Task<IActionResult>Comment(KommentDTO request)
        {
            Komment ujKomment=new Komment();
            ujKomment.KommentSzoveg=request.KommentSzoveg;

            _context.Komments.Add(ujKomment);
            await _context.SaveChangesAsync();
            return Ok(new());   
        }

        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteJelolt(Guid Id)
        {
            var torlendoJelolt = _context?.Users.Where(x=>x.Id==Id).FirstOrDefault();
            var torlendoJeloltToken = _context?.Tokens.Where(x => x.UserDataId ==Id).FirstOrDefault();

            if (torlendoJelolt == null)
            {
                return NotFound();
            }
            BannedUser ujBanned = new BannedUser();
            ujBanned.Id = Guid.NewGuid();
            ujBanned.BannedGmail = torlendoJelolt.Gmail;
            _context.BannedUsers.Add(ujBanned);
            _context.Users.Remove(torlendoJelolt);
            _context.Tokens.Remove(torlendoJeloltToken);
            await _context.SaveChangesAsync();
            return Ok();
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
                expires:DateTime.UtcNow.AddMinutes(5),
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
            token.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(10);
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
