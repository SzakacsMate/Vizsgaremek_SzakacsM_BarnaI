using backendSzM.Data;
using backendSzM.DTOs;
using backendSzM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        
        [HttpPost("register")]//működik
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
            ujUserData.Id=Guid.NewGuid();
            ujUserData.Name=request.Name;
            ujUserData.Hash = hashedPassword;
            ujUserData.Gmail=request.Gmail;
            ujUserData.Role="User";
            ujUserData.Rep = 0;
            ujUserData.ProfileI = "";
            ujUserData.IsSuspended = false;
            _context.Users.Add(ujUserData);
            await _context.SaveChangesAsync();
            return Ok(new { ujUserData.Id, ujUserData.Name, ujUserData.Gmail, ujUserData.Role,ujUserData.Rep,ujUserData.ProfileI,ujUserData.IsSuspended });

        }


        
        [HttpPost("login")]//működik
        public async Task<IActionResult> Login(UserDataDTO request)
        {

            var user = _context.Users.FirstOrDefault(u => u.Gmail == request.Gmail && u.Name == request.Name);
            
           if (user.IsSuspended==true)
            {
                return Unauthorized("Ez a felhasználó fel van függesztve!");
            }

            if (user == null)
            {
                return Unauthorized();
            }
            if (new PasswordHasher<UserData>().VerifyHashedPassword(user,user.Hash,request.Password)==PasswordVerificationResult.Failed)
            {
                return Unauthorized("Rossz jelszó");
            }
            var CurrentToken = _context.Tokens.FirstOrDefault(x => x.UserDataId ==user.Id);
            


            if (CurrentToken== null)
            { 
               var  newToken = new Token();
                newToken.Id = Guid.NewGuid();
                newToken.UserDataId =user.Id;
                newToken.AccesToken = CreateToken(user);
                CurrentToken = newToken;
                _context.Tokens.Add(newToken);
                await _context.SaveChangesAsync();
            }
            string accestoken = CreateToken(user);
            

            var refresh_token = new TokenDTO
            {

                AccesToken = accestoken,
                RefreshToken = await GenAndSaveRefreshTokenAsync(CurrentToken)
            };
          
            return Ok(refresh_token);
            
        }  
        [HttpPost("AddPlayer")]//
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


        
            [HttpPost("refresh")]
        public async Task<ActionResult<TokenDTO>> RefreshToken(RefreshTokenReqDto request)
        {
            
            var result = await RefreshTokenAsync(request);
            if (result == null||result.AccesToken is null) { return Unauthorized("Invalid refresh token"); }
            return Ok(result);
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
            token.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(1);
            await _context.SaveChangesAsync();
            return refreshToken;
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

        
        private async Task<Token> ValidateRefreshTokenAsync(Guid Id, string refreskToken)
        {
            var token = await _context.Tokens.FirstOrDefaultAsync(u => u.UserDataId== Id);
            if (token == null || token.RefreshToken != refreskToken || token.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }
            return token;
        }
        
        
        public async Task<TokenDTO?> RefreshTokenAsync(RefreshTokenReqDto request)
        {
            var token = await ValidateRefreshTokenAsync(request.Id, request.RefreshToken);
            if (token == null) return null;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (user == null) return null;
            var newAccesToken = CreateToken(user);
            var newRefreshToken = await GenAndSaveRefreshTokenAsync(token);
            return new TokenDTO
            {
                AccesToken = newAccesToken,
                RefreshToken = newRefreshToken
            };

        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet("CurrentUser")]
        public async Task<ActionResult> AuthenthicatedUser()
        {
            var name= User?.FindFirst(ClaimTypes.Name)?.Value;
            if (name == null)
            {
                return Unauthorized("Womp");
            }
            var id = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUser = _context?.Users.FirstOrDefault(x=>x.Name== name);
            
            var role = User?.FindFirst(ClaimTypes.Role)?.Value ?? currentUser.Role;
            var image = currentUser.ProfileI;
            var resp= new CurrentUserDTO
            {
                Id=id,
                Name = name,
                Role = role,
                ImageUrl = image
            }
            ;
            return Ok(resp);
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPost("AddLocation")]//működik

        public async Task<IActionResult> AddLocation(LocationDTO request)
        {
            Location ujLocation = new Location();
            ujLocation.Id = Guid.NewGuid();
            ujLocation.LocationName = request.LocationName;
            ujLocation.Adress = request.Adress;
            ujLocation.Description = request.Description;
            ujLocation.Image = request.Image;
            if (ujLocation.Image == null)
            {
                ujLocation.Image = "N/A";
            }
            _context.Locations.Add(ujLocation);
            await _context.SaveChangesAsync();
            return Ok(new());
        }
        [Authorize(Roles = "User,Admin")]
        [HttpDelete("DeleteLocation/{Id}")]//működik
        public async Task<IActionResult> DeleteLocation(Guid Id)
        {
            var torlendoJelolt = _context?.Locations.Where(x => x.Id == Id).FirstOrDefault();
            var torlendoLobbies = _context?.Lobbies.Where(x => x.LocationId == Id).FirstOrDefault();

            if (torlendoJelolt == null)
            {
                return NotFound();
            }

            _context.Locations.Remove(torlendoJelolt);
            _context.Lobbies.Remove(torlendoLobbies);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet("GetLobbies you're in")]
        public async Task<ActionResult<List<LobbyInfoDTO>>> GetLobbiesIn()
        {
            var idClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var userId))
                return Unauthorized("womp womp");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found");

            var lobbies = await _context.LobbyCons
                .Where(lc => lc.UserDataId == userId)
                .Include(lc => lc.Lobby)
                .Where(lc => lc.Lobby != null)
                .Select(lc => new LobbyInfoDTO
                {
                    Dm = lc.Lobby.Dm,
                    LocationName = lc.Lobby.locationName,
                    TtType = lc.Lobby.TtType,
                    StartDate = lc.Lobby.StartDate,
                    EndDate = lc.Lobby.EndDate,
                    PlayerLimit = lc.Lobby.PlayerLimit
                })
                .ToListAsync();



            return Ok(lobbies);
        }
        
       // [Authorize(Roles = "User,Admin")]
        [HttpPost("WriteComment")]//Foreign key error 19 konkrét Idkkal működne
        public async Task<IActionResult> Comment(KommentDTO request)
        {
            var kommentelo = _context.Users.FirstOrDefault(x => x.Name == request.Kommentalo);
            var fogado = _context?.Users.FirstOrDefault(x => x.Name == request.Fogado);
            if (fogado == null)
                return BadRequest("Nincs ilyen fogadó");
            Komment ujKomment = new Komment();
            ujKomment.Id = Guid.NewGuid();
            ujKomment.Fogado = fogado.Name;
            ujKomment.Kommentalo = kommentelo.Name;
            ujKomment.KommentSzoveg = request.KommentSzoveg;
            ujKomment.FogadoId = fogado.Id;
            ujKomment.KommentaloId = kommentelo.Id;
            
            KommentCon newKomment = new KommentCon();
            newKomment.Id = Guid.NewGuid();
            newKomment.UserDataId = Guid.NewGuid();
            newKomment.komment=ujKomment.Id;
            _context.KommentCons.Add(newKomment);
            _context.Komments.Add(ujKomment);
            await _context.SaveChangesAsync();
            return Ok(new());
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPost("Create Lobby")]//működik
        public async Task<ActionResult<CurrentUserDTO>> CreateLobby(LobbyDTO request, Guid Id)
        {
            var name = User?.FindFirst(ClaimTypes.Name)?.Value;
            var user = _context.Users.FirstOrDefault(x => x.Name == name);
            var locationId = _context?.Locations.FirstOrDefault(x => x.Id == Id);
            var location = locationId.LocationName;

            if (user == null)
            {
                return BadRequest("Nincs ilyen felhasználó");
            }

            Lobby ujLobby = new Lobby();
            ujLobby.Id = Guid.NewGuid();
            ujLobby.Dm = user.Name;
            ujLobby.StartDate = request.StartDate;
            ujLobby.EndDate = request.EndDate;
            ujLobby.PlayerLimit = request.PlayerLimit;
            ujLobby.TtType = request.TtType;

            ujLobby.locationName = location;
            ujLobby.LocationId = locationId.Id;

            LobbyCon newLobbyCon = new LobbyCon();
            newLobbyCon.Id = Guid.NewGuid();
            newLobbyCon.UserDataId = user.Id;
            newLobbyCon.LobbyId = ujLobby.Id;
            if (locationId == null)
            {
                return BadRequest("Nincs ilyen helyszín");
            }
            var reserved = await _context.Lobbies.FirstOrDefaultAsync(x =>x.LocationId == locationId.Id &&x.StartDate <= request.EndDate &&request.StartDate <= x.EndDate); 
            if (reserved != null)
            {
                return BadRequest("Ezen a helyen és időpontban már van egy foglalt lobby!");
            }



            _context.Lobbies.Add(ujLobby);
            await _context.SaveChangesAsync();
            _context.LobbyCons.Add(newLobbyCon);
            await _context.SaveChangesAsync();
            return Ok(new());
        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet("GetLocations")]//
        public async Task<ActionResult<List<LocationDTO>>> GetLocations()
        {
            var locations = await _context.Locations.Select(x => new { x.LocationName, x.Adress, x.Description, x.Image }).ToListAsync();
            if (locations == null)
            {
                return NotFound();
            }
            return Ok(locations);
        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet("GetAllLobbies")]//működik
        public async Task<ActionResult<List<LobbyDTO>>> GetAllLobbies()
        {
            var lobbies = await _context.Lobbies.Select(x => new { x.Dm, x.locationName, x.TtType, x.StartDate, x.EndDate, x.PlayerLimit }).ToListAsync();
            if (lobbies == null)
            {
                return NotFound();
            }
            return Ok(lobbies);
        }
        
        [HttpGet("GetLocation")]//
        public async Task<ActionResult<LocationDTO>> GetLocation(Guid Id)
        {
            var locations =  _context.Locations.Select(x => new { x.LocationName, x.Adress, x.Description, x.Image });
            if (locations == null)
            {
                return NotFound();
            }
            return Ok(locations);
        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet("Authorization")]
        public IActionResult AuthorizedEndpoint()
        {
            return Ok("You are an admin!");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are an admin!");
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("Suspend User")]//
        public async Task<IActionResult> SuspendUser(Guid Id)
        {
            var suspendedUser = _context.Users.FirstOrDefault(x => x.Id == Id);

            if (suspendedUser == null)
            {
                return BadRequest("Nincs ilyen felhasználó");
            }
            if (suspendedUser.Warnings % 3 == 0)
            {
                return BadRequest("Ez a felhasználónak nincs elég warningja");
            }
            suspendedUser.IsSuspended = true;
            _context.Users.Update(suspendedUser);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("Give Warning")]//
        public async Task<IActionResult> GiveWarning(Guid Id)
        {
            var warnedUser = _context.Users.FirstOrDefault(x => x.Id == Id);

            if (warnedUser == null)
            {
                return BadRequest("Nincs ilyen felhasználó");
            }
            warnedUser.Warnings += 1;
            _context.Users.Update(warnedUser);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{Id}")]//
        public async Task<IActionResult> DeleteUser(Guid Id)
        {
            var torlendoUser = _context?.Users.Where(x => x.Id == Id).FirstOrDefault();
            var torlendoUserToken = _context?.Tokens.Where(x => x.UserDataId == Id).FirstOrDefault();

            if (torlendoUser == null)
            {
                return NotFound();
            }
            BannedUser ujBanned = new BannedUser();
            ujBanned.Id = Guid.NewGuid();
            ujBanned.BannedGmail = torlendoUser.Gmail;
            _context.BannedUsers.Add(ujBanned);
            _context.Users.Remove(torlendoUser);
            _context.Tokens.Remove(torlendoUserToken);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPatch("Change Role")]//
        public async Task<ActionResult<CurrentUserDTO>> ChangeRole(RoleDTO role, Guid Id)
        {
            var currentId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var changedUser = _context.Users.FirstOrDefault(x => x.Id == Id);
            var roleChanger = _context.Users.FirstOrDefault(x => x.Id.ToString() == currentId && role.Role == "Admin");
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
        
    }
    //Lehet nem kellenek már
    /* lehet törölve lesz
        [Authorize(Roles = "User,Admin")]
        [HttpGet("GetUserName")]
        public async Task<ActionResult> GetUserName(CurrentUserDTO req)
        {
            var jeloltek = _context.Users.FirstOrDefault(x => x.Id.ToString() == req.Id);
            if (jeloltek == null)
            {
                return NotFound();
            }
            return Ok(jeloltek.Name);
        }
    
        [HttpGet("GetUserImage")]
        public async Task<ActionResult<List<UserData>>> GetUserImage(Guid id)
        {
            var jeloltek = _context.Users.FirstOrDefault(x => x.Id == id);
            if (jeloltek == null)
            {
                return NotFound();
            }
            return Ok(jeloltek.ProfileI);
    [HttpPatch("Change Role Rendszergazda")]
        public async Task<IActionResult> ChangeRoleNoAdmin(RoleNoAdminDTO role, Guid Id)
        {
            var changedUser = _context.Users.FirstOrDefault(x => x.Id == Id);

            if (changedUser == null)
            {
                return BadRequest("Nincs ilyen felhasználó");
            }

            changedUser.Role = role.Role;
            _context.Users.Update(changedUser);
            await _context.SaveChangesAsync();
            return Ok();
        }
        }*/
}
