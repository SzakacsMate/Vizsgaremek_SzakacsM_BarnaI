using backendSzM.Data;
using backendSzM.DTOs;
using backendSzM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
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
        private readonly TimeSpan _accessTokenLifetime = TimeSpan.FromHours(10);
        private readonly TimeSpan _refreshTokenLifetime = TimeSpan.FromDays(7);

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
            if(ujUserData.Name==""|| ujUserData.Gmail == "" || ujUserData.Hash == "" )
            {
                return BadRequest("Név, Gmail vagy jelszó hiányzik");
            }
            _context.Users.Add(ujUserData);
            await _context.SaveChangesAsync();
            return Ok(new { ujUserData.Id, ujUserData.Name, ujUserData.Gmail, ujUserData.Role,ujUserData.Rep,ujUserData.ProfileI});

        }


        
        [HttpPost("login")]//működik
        public async Task<IActionResult> Login(UserDataDTO request)
        {
            var banned = _context?.BannedUsers.FirstOrDefault(x => x.BannedGmail == request.Gmail);

            var user = _context?.Users.FirstOrDefault(u => u.Gmail == request.Gmail && u.Name == request.Name);
            if (user == null)
            {
                return Unauthorized("Nincs ilyen felhasználó");
            }
            if (user.IsSuspended==true)
            {
                return Unauthorized("Ez a felhasználó fel van függesztve!");
            }

            
            if (new PasswordHasher<UserData>().VerifyHashedPassword(user,user.Hash,request.Password)==PasswordVerificationResult.Failed)
            {
                return Unauthorized("Rossz jelszó");
            }
            if (banned != null)
            {
                return Unauthorized("Ez a Gmail ki van bannolva");
            }
            var CurrentToken = _context.Tokens.FirstOrDefault(x => x.UserDataId ==user.Id);
            var accessToken = CreateToken(user);
            var accessExpiry = DateTime.UtcNow.Add(_accessTokenLifetime);
            if (CurrentToken== null)
            { 
               var  newToken = new Token
                    {
                   Id = Guid.NewGuid(),
                   UserDataId = user.Id,
                   AccesToken= accessToken,
                   AccessTokenExpiryTime = accessExpiry


               };
                _context.Tokens.Add(newToken);
                CurrentToken = newToken;
            }
            else
            {
                CurrentToken.AccesToken = accessToken;
                CurrentToken.AccessTokenExpiryTime = accessExpiry;
                _context.Tokens.Update(CurrentToken);
            }
            await _context.SaveChangesAsync();
            var refresh_token = new TokenDTO
            {

                AccesToken = accessToken,
                AccessTokenExpiryTime = CurrentToken.AccessTokenExpiryTime,
                RefreshToken = await GenAndSaveRefreshTokenAsync(CurrentToken),
                RefreshTokenExpiryTime = CurrentToken.RefreshTokenExpiryTime

            };
            return Ok(refresh_token);
            
        }
        


        
        [HttpPost("refresh")]//refreshexpiry 1sec es 
        public async Task<ActionResult<TokenDTO>> RefreshToken(RefreshTokenReqDto request)
        {
            
            var result = await RefreshTokenAsync(request);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (result == null||result.AccesToken is null) { return Unauthorized("Invalid refresh token"); }
            if (user.Name!=request.name|| user.Gmail!=request.email)
            {
                return Unauthorized("Téves felhasználói adat");
            }
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
            token.RefreshTokenExpiryTime = DateTime.UtcNow.Add(_refreshTokenLifetime);
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
                expires:DateTime.UtcNow.Add(_accessTokenLifetime),
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
        private async Task<ActionResult> ValidateAccesToken()
        {
            var idclaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idclaim) || !Guid.TryParse(idclaim, out var userId))
                return Unauthorized();
            var tokenRow = await _context.Tokens.FirstOrDefaultAsync(t => t.UserDataId == userId);
            if (tokenRow == null)
                return Unauthorized("No token record");
            if (tokenRow.AccessTokenExpiryTime.HasValue && tokenRow.AccessTokenExpiryTime <= DateTime.UtcNow)
                return Unauthorized("Access token expired");
          
            return null;
        }

        public async Task<TokenDTO?> RefreshTokenAsync(RefreshTokenReqDto request)
        {
            var token = await ValidateRefreshTokenAsync(request.Id, request.RefreshToken);
            if (token == null) return null;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (user == null) return null;
            var newAccesToken = CreateToken(user);
            token.AccesToken = newAccesToken;
            token.AccessTokenExpiryTime = DateTime.UtcNow.Add(_accessTokenLifetime);

            var newRefreshToken = await GenAndSaveRefreshTokenAsync(token);
            return new TokenDTO
            {
                AccesToken = newAccesToken,
                AccessTokenExpiryTime = token.AccessTokenExpiryTime,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiryTime = token.RefreshTokenExpiryTime

            };

        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet("CurrentUser")]
        public async Task<ActionResult<TokenDTO>> AuthenthicatedUser()
        {
           var check=await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }

            var name= User?.FindFirst(ClaimTypes.Name)?.Value;
            if (name == null)
            {
                return Unauthorized("Nincs ilyen felhasználó");
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
                ImageUrl = image,
                Rep=currentUser.Rep
            }
            ;
            return Ok(resp);
        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet("Search for user")]
        public async Task<ActionResult<TokenDTO>> SearchUser(Guid id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var lookedUser = _context.Users.FirstOrDefault(x => x.Id == id);


            return Ok(new {lookedUser.Name,lookedUser.ProfileI,lookedUser.Rep,lookedUser.Role});

        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet("GetLobbies_youre_in")]
        public async Task<ActionResult<List<LobbyInfoDTO>>> GetLobbiesIn()
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
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
        [Authorize(Roles = "User,Admin")]
        [HttpPatch("ChangeUserData")]// átdolgozandó
        public async Task<ActionResult<CurrentUserDTO>> ChangeUserData(ChangeDataDTO profile)
        {
            var check = await ValidateAccesToken();
            if (check != null) return check;

            var idClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var userId))
                return Unauthorized();

            var changedUser = await _context.Users.FindAsync(userId);
            if (changedUser == null)
                return NotFound("User not found");

            
            if (changedUser.Id != userId)
                return Unauthorized("Csak saját profilodat módosíthatod");

            
            var pwVerify = new PasswordHasher<UserData>()
                .VerifyHashedPassword(changedUser, changedUser.Hash, profile.CurrentPassword);
            if (pwVerify == PasswordVerificationResult.Failed || profile.CurrentName != changedUser.Name)
                return Unauthorized("rossz eredeti név vagy jelszó");

            if (string.IsNullOrWhiteSpace(profile.ChangeName) || string.IsNullOrWhiteSpace(profile.ChangePassword))
                return BadRequest("Valamelyik mező üres");

            
            if (await _context.Users.AnyAsync(u => u.Name == profile.ChangeName && u.Id != userId))
                return BadRequest("Már van ilyen felhasználónév");

            
            changedUser.Name = profile.ChangeName;
            changedUser.Hash = new PasswordHasher<UserData>().HashPassword(changedUser, profile.ChangePassword);
            if (!string.IsNullOrEmpty(profile.ChangeProfileI))
                changedUser.ProfileI = profile.ChangeProfileI;

            _context.Users.Update(changedUser);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPatch("Add/RemoveRep")]// átnézendő-működik
        public async Task<ActionResult<CurrentUserDTO>> AddRep(RepDTO rep, Guid id)
        {
            var Tesz=0;
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var kapoUser = _context.Users.FirstOrDefault(x => x.Id == id);
            var currentId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
           // var Changeduser = _context.Users.FirstOrDefault(x => x.Id.ToString() == id);
           if (kapoUser.Id.ToString() == currentId)
                {
                return BadRequest("Nem szavazhatsz magadra!");
            }
            if (rep.Rep == 1)
            {
                Tesz ++;
            }
            else if (rep.Rep == -1)
            {
                Tesz--;
            }
            else
            {
                return BadRequest("Invalid Rep action");
            }
            kapoUser.Rep+= Tesz;
            _context.Users.Update(kapoUser);
            await _context.SaveChangesAsync();
            Tesz = 0;
            return Ok();
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPost("AddLocation")]// működik
        public async Task<ActionResult<CurrentUserDTO>> AddLocation(LocationDTO request)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
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
            if (ujLocation.LocationName==""|| ujLocation.Adress=="")
            {
                return BadRequest("Helyszín neve / címe hiányzik");
            }
            _context.Locations.Add(ujLocation);
            await _context.SaveChangesAsync();
            return Ok(new());
        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet("GetLocations")]//
        public async Task<ActionResult<List<LocationDTO>>> GetLocations()
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var locations = await _context.Locations.Select(x => new { x.LocationName, x.Adress, x.Description, x.Image }).ToListAsync();
            if (locations == null)
            {
                return NotFound();
            }
            return Ok(locations);
        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet("GetLocation")]//  Egyszerre több helyet mutat - működik
        public async Task<ActionResult<LocationDTO>> GetLocation(Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var locationId = _context.Locations.FirstOrDefault(x => x.Id==Id);
            var locations = _context.Locations.Select(x => new { x.LocationName, x.Adress, x.Description, x.Image,x.Id }).Where(x=>x.Id==Id);
            if (locations == null)
            {
                return NotFound();
            }
            return Ok(locations);
        }
        [Authorize(Roles = "User,Admin")]
        [HttpDelete("DeleteLocation/{Id}")]
        public async Task<ActionResult<CurrentUserDTO>> DeleteLocation(Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var torlendoLocation = _context?.Locations.Where(x => x.Id == Id).FirstOrDefault();
            var torlendoLobbies = _context?.Lobbies.Where(x => x.LocationId == Id).FirstOrDefault();

            if (torlendoLocation == null)
            {
                return NotFound();
            }
            if (torlendoLobbies != null)
            {
                _context.Lobbies.Remove(torlendoLobbies);
                await _context.SaveChangesAsync();
            }

            _context.Locations.Remove(torlendoLocation);
            
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPost("CreateLobby")]//Player limitet hozzáadni
        public async Task<ActionResult<CurrentUserDTO>> CreateLobby(LobbyDTO request, Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
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
            ujLobby.PlayerCount = 0;
            ujLobby.locationName = location;
            ujLobby.LocationId = locationId.Id;
            if(ujLobby.PlayerLimit<=2)
            {
                return BadRequest("Legalább 3-nak kell lennie a Játékos limitnek");
            }
            if(locationId.Id ==null)
            {
                return BadRequest("Nincs ilyen helyszín");
            }
            LobbyCon newLobbyCon = new LobbyCon();
            newLobbyCon.Id = Guid.NewGuid();
            newLobbyCon.UserDataId = user.Id;
            newLobbyCon.LobbyId = ujLobby.Id;
            if (locationId == null)
            {
                return BadRequest("Nincs ilyen helyszín");
            }
            var reserved = await _context.Lobbies.FirstOrDefaultAsync(x => x.LocationId == locationId.Id && x.StartDate <= request.EndDate && request.StartDate <= x.EndDate);
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
        [HttpGet("GetAllLobbies")]//működik
        public async Task<ActionResult<List<LobbyDTO>>> GetAllLobbies()
        {
            List<UserData> names = new();
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            
            
            var lobbies = await _context.Lobbies.Select(x => new { x.Dm, x.locationName, x.TtType, x.StartDate, x.EndDate, x.PlayerLimit}).ToListAsync();
            if (lobbies == null)
            {
                return NotFound();
            }
            return Ok(lobbies);
        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet("GetLobby")]//   - működik
        public async Task<ActionResult<LocationDTO>> GetLobby(Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }

            var lobbies = _context.Lobbies.Select(x => new { x.Dm, x.locationName, x.TtType, x.StartDate, x.EndDate, x.PlayerLimit, x.PlayerCount, x.Id }).Where(x => x.Id == Id);
            if (lobbies == null)
            {
                return NotFound();
            }
            return Ok(lobbies);
        }
        [Authorize(Roles = "User,Admin")]//Dm és admin törölhet lobbyt, de csak a sajátját - tesztelendő
        [HttpDelete("DeleteLobby/{Id}")]
        public async Task<ActionResult<CurrentUserDTO>> DeleteLobby(Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var claimId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentId = _context.Users.FirstOrDefault(x => x.Id.ToString() == claimId);
            var torlendoLobbyCon = _context?.LobbyCons.Where(x => x.LobbyId == Id).FirstOrDefault();
            var torlendoLobbies = _context?.Lobbies.Where(x => x.Id == Id).FirstOrDefault();
            if(torlendoLobbies.Dm != claimId||currentId.Role!="Admin" )
            {
                return Unauthorized("Nincs jogod a lobby törléséhez");
            }
            if (torlendoLobbies == null)
            {
                return NotFound();
            }

            _context.LobbyCons.Remove(torlendoLobbyCon);
            _context.Lobbies.Remove(torlendoLobbies);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPost("AddPlayer")]//Csak saját magát rakhatja be egy játékos lobbyba - tesztelendő
        public async Task<ActionResult<TokenDTO>> AddPlayer(JoinLobbyDTO request)
        {
            var claimId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = _context.Users.FirstOrDefault(x => x.Id == request.PlayerId);
            var lobby = _context.Lobbies.FirstOrDefault(x => x.Id == request.LobbyId);
            
            if(user.Id.ToString() != claimId)
            {
                return Unauthorized("Nem adhatsz más játékost hozzáadni a lobbyhoz!");
            }
            
            lobby.PlayerCount++;
            LobbyCon newLobbyCon = new LobbyCon();
            newLobbyCon.Id = Guid.NewGuid();
            newLobbyCon.UserDataId = user.Id;
            newLobbyCon.LobbyId = lobby.Id;
            
            _context.LobbyCons.Add(newLobbyCon);
            _context.Lobbies.Update(lobby);
            await _context.SaveChangesAsync();
            return Ok(new());
        }
        [Authorize(Roles = "User,Admin")]
        [HttpDelete("RemovePlayerFromLobby/{Id}")] //DM saját lobbyban/Admin bárkit player csak magát- tesztelendő
        public async Task<ActionResult<CurrentUserDTO>> RemovePlayer(Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var idClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentId = _context.Users.FirstOrDefault(x => x.Id.ToString() == idClaim);
            var torlendoLobbyCon = _context?.LobbyCons.Where(x => x.LobbyId == Id).FirstOrDefault();
            var torlendoLobbies = _context?.Lobbies.Where(x => x.Id == Id).FirstOrDefault();
            //admin /dm törölhet játékost
            if (torlendoLobbies == null)
            {
                return NotFound();
            }
            if (torlendoLobbies.Dm != idClaim||currentId.Role!="Admin")
            {
                return Unauthorized("Nincs jogod ehhez!");
            }

            _context.LobbyCons.Remove(torlendoLobbyCon);
            _context.Lobbies.Remove(torlendoLobbies);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize(Roles = "User,Admin")]
        [HttpDelete("LeavFromLobby")] //DM saját lobbyban/Admin bárkit player csak magát- tesztelendő
        public async Task<ActionResult<CurrentUserDTO>> LeavFromLobby(Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var idClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentId = _context.Users.FirstOrDefault(x => x.Id.ToString() == idClaim);
            var torlendoLobbyCon = _context?.LobbyCons.Where(x => x.LobbyId == Id).FirstOrDefault();
            var torlendoLobbies = _context?.Lobbies.Where(x => x.Id == Id).FirstOrDefault();
            //admin /dm törölhet játékost
            if (torlendoLobbies == null)
            {
                return NotFound();
            }
            if (idClaim!=currentId.ToString())
            {
                return Unauthorized("Nincs jogod ehhez!");
            }

            _context.LobbyCons.Remove(torlendoLobbyCon);
            _context.Lobbies.Remove(torlendoLobbies);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize(Roles = "User,Admin")]
        [HttpDelete("LeaveLobby")]//tesztelendő
        public async Task<ActionResult<CurrentUserDTO>> LeaveLobby(Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var idClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentId = _context.Users.FirstOrDefault(x => x.Id.ToString() == idClaim);
            var torlendoLobbyCon = _context?.LobbyCons.Where(x => x.LobbyId == Id && x.UserDataId.ToString() == idClaim).FirstOrDefault();
            if (torlendoLobbyCon == null)
            {
                return NotFound();
            }
            _context.LobbyCons.Remove(torlendoLobbyCon);
            
            await _context.SaveChangesAsync();
            return Ok();
        }


        [Authorize(Roles = "User,Admin")]
        [HttpPost("WriteComment")] // fogado accese lehet csak 
        public async Task<IActionResult> Comment(KommentDTO request)
        {
            var check = await ValidateAccesToken();
            if (check != null) return check;

            var idClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var userId))
                return Unauthorized();

            // fetch current user directly by Guid
            var kommentelo = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (kommentelo == null)
                return Unauthorized("User not found");

            var fogado = await _context.Users.FirstOrDefaultAsync(x => x.Name == request.Fogado);
            if (fogado == null)
                return BadRequest("Nincs ilyen fogadó");

            if (kommentelo.Id != userId)
                return Unauthorized();

            var ujKomment = new Komment
            {
                Id = Guid.NewGuid(),
                KommentSzoveg = request.KommentSzoveg,
                KommentaloUserId = kommentelo.Id,
                FogadoUserId = fogado.Id
            };

            _context.Komments.Add(ujKomment);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet("FogadoComments/{Id}")]
        public async Task<IActionResult> Comments(Guid Id)
            {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var fogado = await _context.Users.FindAsync(Id);
            if (fogado == null)
                return BadRequest("Nincs ilyen fogadó");
            var fogadoId=_context.Komments.FirstOrDefault(x => x.FogadoUserId == Id);
            var kommentelo = _context.Users.FirstOrDefault(x=>x.Id==fogadoId.KommentaloUserId);
            var kommentek = await _context.Komments.Where(x => x.FogadoUserId == Id).Select(x => new { x.KommentSzoveg, x.KommentaloUserId,kommentelo.Name }).ToListAsync(); 
            return Ok(kommentek);
        }


        [Authorize(Roles = "User,Admin")]
        [HttpDelete("DeleteComment/{Id}")]
        public async Task<ActionResult<CurrentUserDTO>> DeleteComment(Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var torlendoKomment = _context?.Komments.Where(x => x.Id == Id).FirstOrDefault();

            if (torlendoKomment == null)
            {
                return NotFound();
            }

            
            _context.Komments.Remove(torlendoKomment);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("Authorization checker")]
        public async Task<ActionResult> AuthorizedEndpoint()
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            return Ok("You are authorized!");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public async Task<ActionResult<CurrentUserDTO>> AdminOnlyEndpoint()
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            return Ok("You are an admin!");
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("SuspendUser")]// müködik
        public async Task<IActionResult> SuspendUser([FromQuery]Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var suspendedUser = _context.Users.FirstOrDefault(x => x.Id == Id);

            if (suspendedUser == null)
            {
                return BadRequest("Nincs ilyen felhasználó");
            }
            if (suspendedUser.Warnings % 3 == 0 && suspendedUser.Warnings<=3)
            {
                return BadRequest("Ennek a felhasználónak nincs elég warningja");
            }
            suspendedUser.IsSuspended = true;
            _context.Users.Update(suspendedUser);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("UnSuspendUser")]// müködik
        public async Task<IActionResult> UnSuspendUser([FromQuery] Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var UnsuspendedUser = _context.Users.FirstOrDefault(x => x.Id == Id);

            if (UnsuspendedUser == null)
            {
                return BadRequest("Nincs ilyen felhasználó");
            }
            
            UnsuspendedUser.IsSuspended = false;
            _context.Users.Update(UnsuspendedUser);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("GiveWarning")]// működik
        public async Task<IActionResult> GiveWarning(Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
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
        [HttpDelete("BanUser/{Id}")]// működik
        public async Task<IActionResult> DeleteUser(Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var torlendoUser = _context?.Users.Where(x => x.Id == Id).FirstOrDefault();
            var torlendoUserKomments = _context?.Komments.Where(x => x.KommentaloUserId == Id || x.FogadoUserId == Id).ToList();
            var torlendoUserToken = _context?.Tokens.Where(x => x.UserDataId == Id).FirstOrDefault();

            if (torlendoUser == null)
            {
                return NotFound();
            }
            BannedUser ujBanned = new BannedUser();
            ujBanned.Id = Guid.NewGuid();
            ujBanned.BannedGmail = torlendoUser.Gmail;
            _context.BannedUsers.Add(ujBanned);
            if (torlendoUserToken != null)
            {
                _context.Tokens.Remove(torlendoUserToken);
            }
            _context.RemoveRange(torlendoUserKomments);
            _context.Users.Remove(torlendoUser);

            await _context.SaveChangesAsync();
            
            return Ok();
        }
        [HttpPatch("ChangeRole")]// 
        public async Task<ActionResult<CurrentUserDTO>> ChangeRole(RoleDTO role, Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
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
        [HttpPatch("Change Role Rendszergazda")]
        public async Task<ActionResult<CurrentUserDTO>> ChangeRoleNoAdmin(RoleNoAdminDTO role)
        {
            var changedUser = _context.Users.FirstOrDefault(x => x.Id == role.ChangedUserId);

            if (changedUser == null)
            {
                return BadRequest("Nincs ilyen felhasználó");
            }
            

            changedUser.Role = role.Role;
            _context.Users.Update(changedUser);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<List<UserData>>> GetAllUsers()
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            
            var users = await _context.Users
                .Select(x => new { x.Id, x.Name, x.Rep, x.Gmail, x.Role, x.Warnings, x.IsSuspended })
                .ToListAsync();
            
            if (users == null || !users.Any())
            {
                return NotFound();
            }
            
            return Ok(users);
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
    
        }
      var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var presented = authHeader.Substring("Bearer ".Length).Trim();
                if (!string.IsNullOrEmpty(tokenRow.AccesToken) && !string.Equals(tokenRow.AccesToken, presented, StringComparison.Ordinal))
                    return Unauthorized("Token mismatch");
            }
    */
}
