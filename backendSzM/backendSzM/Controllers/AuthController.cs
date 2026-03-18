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
           // var user = _context.Users.FirstOrDefault();
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
        


        
        [HttpPost("refresh")]// működik rn
        public async Task<ActionResult<TokenDTO>> RefreshToken(RefreshTokenReqDto request)
        {
            
            var result = await RefreshTokenAsync(request);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (result == null||result.AccesToken is null) { return Unauthorized("Invalid refresh token"); }
           /* if (user.Name!=request.name|| user.Gmail!=request.email)
            {
                return Unauthorized("Téves felhasználói adat");
            }*/
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
        [HttpGet("CurrentUser")] // műlödik
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
            var idClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var userId))
            {
                return Unauthorized();
            }
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (currentUser == null)
            {
                return Unauthorized("Nincs ilyen felhasználó");
            }
            var role = User?.FindFirst(ClaimTypes.Role)?.Value ?? currentUser.Role;
            var image = currentUser.ProfileI;

            var resp= new CurrentUserDTO
            {
                Id=idClaim,
                Name = name,
                Role = role,
                ImageUrl = image,
                Rep=currentUser.Rep
            }
            ;
            return Ok(resp);
        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet("Search for user")] // működik
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
        public async Task<ActionResult<List<LobbyDTO>>> GetLobbiesIn()
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var idClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found");
            
          
            var lobbies = await _context.LobbyCons
                .Where(x => x.UserDataId == userId)
                .Select(x => new 
                {
                     x.Lobby.Dm,x.Lobby.locationName,x.Lobby.TtType,x.Lobby.StartDate,x.Lobby.EndDate,x.Lobby.PlayerLimit,x.Lobby.PlayerCount,x.Lobby.Status,x.Lobby.PlayerMin,x.Lobby.Location.Adress,
                    Users = x.Lobby.LobbyCons
                        .Where(lc => lc.UserData.Name != x.Lobby.Dm)
                        .Select(lc => lc.UserData.Name)
                        .ToList(),
                   
                })
                .ToListAsync();



            return Ok(lobbies);
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPatch("ChangeUserData")]// működik
        public async Task<ActionResult<TokenDTO>> ChangeUserData(ChangeDataDTO profile)
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
            var newAccessToken = CreateToken(changedUser);
            var tokenRow = await _context.Tokens.FirstOrDefaultAsync(t => t.UserDataId == userId);

            if (tokenRow != null)
            {
                tokenRow.AccesToken = newAccessToken;
                tokenRow.AccessTokenExpiryTime = DateTime.UtcNow.Add(_accessTokenLifetime);
                _context.Tokens.Update(tokenRow);

                var newRefreshToken = await GenAndSaveRefreshTokenAsync(tokenRow);

                await _context.SaveChangesAsync();

                return Ok(new TokenDTO
                {
                    AccesToken = newAccessToken,
                    AccessTokenExpiryTime = tokenRow.AccessTokenExpiryTime,
                    RefreshToken = newRefreshToken,
                    RefreshTokenExpiryTime = tokenRow.RefreshTokenExpiryTime
                });
            }
            return Ok();
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPatch("Add/RemoveRep")]// működik
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
        
        [HttpGet("GetLocations")]// működik
        public async Task<ActionResult<List<LocationDTO>>> GetLocations()
        {
            
            var locations = await _context.Locations.Select(x => new { x.Id,x.LocationName, x.Adress, x.Description, x.Image }).ToListAsync();
            if (locations == null)
            {
                return NotFound();
            }
            return Ok(locations);
        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet("GetLocation")]// működik
        public async Task<ActionResult<LocationDTO>> GetLocation(Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            //var locationId = _context.Locations.FirstOrDefault(x => x.Id==Id);
            var locations = _context.Locations.Select(x => new { x.LocationName, x.Adress, x.Description, x.Image,x.Id }).Where(x=>x.Id==Id);
            if (locations == null)
            {
                return NotFound();
            }
            return Ok(locations);
        }
        [Authorize(Roles = "User,Admin")]
        [HttpDelete("DeleteLocation/{Id}")] //müködik
        public async Task<ActionResult<CurrentUserDTO>> DeleteLocation(Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var torlendoLocation = _context?.Locations.Where(x => x.Id == Id).FirstOrDefault();
            var torlendoLobbies = _context?.Lobbies.Where(x => x.LocationId == Id).ToList();
            var torlendoLobbyCon = _context?.LobbyCons.Where(x => x.Lobby.LocationId == Id).ToList();
            if (torlendoLocation == null)
            {
                return NotFound();
            }
            if (torlendoLobbies == null)
            {
                _context.Locations.Remove(torlendoLocation);
                await _context.SaveChangesAsync();
                return Ok();
            }
            _context.LobbyCons.RemoveRange(torlendoLobbyCon);
            _context.Lobbies.RemoveRange(torlendoLobbies);
            _context.Locations.Remove(torlendoLocation);
            
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPost("CreateLobby")]// működik
        public async Task<ActionResult<CurrentUserDTO>> CreateLobby(LobbyDTO request, Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            
            var name = User?.FindFirst(ClaimTypes.Name)?.Value;
            var user = _context?.Users.FirstOrDefault(x => x.Name == name);
            var locationId = _context?.Locations.FirstOrDefault(x => x.Id == Id);
            var location = locationId.LocationName;
            
            if (user == null)
            {
                return BadRequest("Nincs ilyen felhasználó");
            }
            
            Lobby ujLobby = new Lobby();
            ujLobby.Id = Guid.NewGuid();
            ujLobby.Dm = user.Name;
            ujLobby.LobbyName = request.LobbyName;
            ujLobby.StartDate = request.StartDate;
            ujLobby.EndDate = request.EndDate;
            ujLobby.PlayerLimit = request.PlayerLimit;
            ujLobby.TtType = request.TtType;
            ujLobby.PlayerCount = 0;
            ujLobby.locationName = location;
            ujLobby.LocationId = locationId.Id;
            ujLobby.Status = "Pending";
            ujLobby.PlayerMin = request.PlayerMin;
            /*
            if(ujLobby.PlayerLimit<=2)
            {
                return BadRequest("Legalább 3-nak kell lennie a Játékos limitnek");
            }*/
            if (ujLobby.PlayerMin > ujLobby.PlayerLimit)
            {
                return BadRequest("Minimum játékosszám nem lehet nagyobb a maximumnál");
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
           
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            

            


            var lobbies = await _context.Lobbies.Select(x => new { x.Id,x.LobbyName, x.Dm, x.locationName, x.TtType, x.StartDate, x.EndDate, x.PlayerLimit,x.PlayerCount,x.Status,x.Location.Adress,
                Users = x.LobbyCons
                    .Where(lc => lc.UserData.Name != x.Dm)
                    .Select(lc => lc.UserData.Name)
                    .ToList()
            }).ToListAsync();
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
            var lobbies = _context.Lobbies.Select(x => new { x.LobbyName, x.Dm, x.locationName, x.TtType, x.StartDate, x.EndDate, x.PlayerLimit, x.PlayerCount, x.Id, x.Status,x.Location.Adress ,
                Users = x.LobbyCons
                    .Where(lc => lc.UserData.Name != x.Dm)
                    .Select(lc => lc.UserData.Name)
                    .ToList()
            }).Where(x => x.Id == Id);  



            if (lobbies == null)
            {
                return NotFound();
            }
            return Ok(lobbies);
        }
        [Authorize(Roles = "User,Admin")]
        [HttpDelete("DeleteLobby/{Id}")] //működik
        public async Task<ActionResult<CurrentUserDTO>> DeleteLobby(Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var id = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var userId))
            {
                return Unauthorized();
            }
            var currentUser = await _context.Users.FindAsync(userId);
            if (currentUser == null)
            {
                return Unauthorized("User not found");
            }
            var name = User?.FindFirst(ClaimTypes.Name)?.Value;
            var torlendoLobbyCon = await _context.LobbyCons.FirstOrDefaultAsync(x => x.LobbyId == Id);
            var torlendoLobbies = await _context.Lobbies.FirstOrDefaultAsync(x => x.Id == Id);

            if (torlendoLobbies == null)
            {
                return NotFound();
            }
            if (torlendoLobbies.Dm != name && currentUser.Role != "Admin")
            {
                return Unauthorized("Nincs jogod a lobby törléséhez");
            }

            if (torlendoLobbyCon != null)
            {
                _context.LobbyCons.Remove(torlendoLobbyCon);
            }
            _context.Lobbies.Remove(torlendoLobbies);

            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPost("AddPlayer")]//működik
        public async Task<IActionResult> AddPlayer(Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }

            var claimId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claimId) || !Guid.TryParse(claimId, out var userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return Unauthorized("User not found");
            }

            var lobby = await _context.Lobbies.FindAsync(Id);
            if (lobby == null)
            {
                return NotFound("Lobby not found");
            }

            
            if (lobby.Dm == user.Name)
            {
                return BadRequest("A DM nem csatlakozhat a saját lobbyjához.");
            }

            
            var alreadyInLobby = await _context.LobbyCons
                .AnyAsync(x => x.LobbyId == Id && x.UserDataId == user.Id);
            if (alreadyInLobby)
            {
                return BadRequest("Már csatlakoztál ehhez a lobbyhoz.");
            }

            
            if (lobby.PlayerCount >= lobby.PlayerLimit)
            {
                return BadRequest("A lobby elérte a maximum játékosszámot.");
            }

            lobby.PlayerCount++;

            var newLobbyCon = new LobbyCon
            {
                Id = Guid.NewGuid(),
                UserDataId = user.Id,
                LobbyId = lobby.Id
            };

            _context.LobbyCons.Add(newLobbyCon);
            _context.Lobbies.Update(lobby);

            await _context.SaveChangesAsync();

            if (lobby.PlayerCount >= lobby.PlayerMin)
            {
                lobby.Status = "Confirmed";
                _context.Lobbies.Update(lobby);
                await _context.SaveChangesAsync();
            }

            return Ok(new());
        }/*
           if (torlendoLobbies.Dm != currentId.Name||currentId.Role!="Admin")
            {
                return Unauthorized("Nincs jogod ehhez!");
            }*/
        [Authorize(Roles = "User,Admin")]
        [HttpDelete("RemovePlayerFromLobby")] // működik
        public async Task<IActionResult> RemovePlayerFromLobby([FromQuery] Guid lobbyId, [FromQuery] Guid userId)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var id = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var CurrentuserId))
            {
                return Unauthorized();
            }
            var currentUser = await _context.Users.FindAsync(CurrentuserId);
            if (currentUser == null)
            {
                return NotFound("Nincs ilyen felhasználó.");
            }
            var lobby = await _context.Lobbies.FirstOrDefaultAsync(x => x.Id == lobbyId);
            if (lobby == null)
            {
                return NotFound("Nincs ilyen lobby.");
            }
            var lobbyCon = await _context.LobbyCons.FirstOrDefaultAsync(x => x.LobbyId == lobbyId && x.UserDataId == userId);
            if (lobbyCon == null)
            {
                return BadRequest("A felhasználó nincs ebben a lobbyban.");
            }

            if (lobby.Dm != currentUser.Name && currentUser.Role != "Admin")
            {
                return Unauthorized("Nincs jogod a játékos eltávolításához!");
            }
            _context.LobbyCons.Remove(lobbyCon);
            if (lobby.PlayerCount > 0)
            {
                lobby.PlayerCount--;
                _context.Lobbies.Update(lobby);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
        /*
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
        }*/
        [Authorize(Roles = "User,Admin")]
        [HttpDelete("LeaveLobby")]//tesztelendő
        public async Task<ActionResult<CurrentUserDTO>> LeaveLobby(Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var claimId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claimId) || !Guid.TryParse(claimId, out var userId))
            {
                return Unauthorized();
            }
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return Unauthorized("User not found");
            }
            var csokkentoLobby = _context.Lobbies.FirstOrDefault(x => x.Id == Id);
            var torlendoLobbyCon = _context?.LobbyCons.FirstOrDefault(x => x.LobbyId == Id && x.UserDataId == user.Id);
            
            if (torlendoLobbyCon == null)
            {
                return NotFound();
            }
            csokkentoLobby.PlayerCount--;
            if( csokkentoLobby.PlayerCount!>=csokkentoLobby.PlayerMin)
            {
                csokkentoLobby.Status = "Pending";
                _context.Lobbies.Update(csokkentoLobby);
                await _context.SaveChangesAsync();
            }
            _context.Lobbies.Update(csokkentoLobby);
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


            var kommentalo = await _context.Users.FindAsync(userId);
            if (kommentalo == null)
            {
                return Unauthorized("User not found");
            }

            var fogado = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Fogado);
            if (fogado == null)
                return BadRequest("Nincs ilyen fogadó");

            

            var ujKomment = new Komment
            {
                Id = Guid.NewGuid(),
               KommentSzoveg = request.KommentSzoveg,
                KommentaloUserId = kommentalo.Id,
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
        [Authorize(Roles = "Admin")]
        [HttpPatch("ChangeToAdmin")]// müködik
        public async Task<ActionResult<CurrentUserDTO>> ChangeRoleAdmin(Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var currentId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentRole = User?.FindFirst(ClaimTypes.Role)?.Value;
            var changedUser = _context.Users?.FirstOrDefault(x => x.Id == Id);
            var roleChanger = _context.Users?.FirstOrDefault(x => x.Id.ToString() == currentId && currentRole == "Admin");
            if (changedUser == null)
            {
                return BadRequest("Nincs ilyen felhasználó");
            }
            if (currentRole != "Admin")
            {
                return Unauthorized("Ehhez nincs jogod!");
            }
            changedUser.Role = "Admin";
            _context.Users.Update(changedUser);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("ChangeToUser")]// müködik
        public async Task<ActionResult<CurrentUserDTO>> ChangeRoleUser( Guid Id)
        {
            var check = await ValidateAccesToken();
            if (check != null)
            {
                return check;
            }
            var currentId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentRole = User?.FindFirst(ClaimTypes.Role)?.Value;
            var changedUser = _context.Users?.FirstOrDefault(x => x.Id == Id);
            var roleChanger = _context.Users?.FirstOrDefault(x => x.Id.ToString() == currentId && currentRole== "Admin");
            if (changedUser == null)
            {
                return BadRequest("Nincs ilyen felhasználó");
            }
            if (currentRole!= "Admin")
            {
                return Unauthorized("Ehhez nincs jogod!");
            }
            changedUser.Role = "User";
            _context.Users.Update(changedUser);
            await _context.SaveChangesAsync();
            var newAccessToken = CreateToken(changedUser);
            var tokenRow = await _context.Tokens.FirstOrDefaultAsync(t => t.UserDataId == changedUser.Id);

            if (tokenRow != null)
            {
                tokenRow.AccesToken = newAccessToken;
                tokenRow.AccessTokenExpiryTime = DateTime.UtcNow.Add(_accessTokenLifetime);
                _context.Tokens.Update(tokenRow);

                var newRefreshToken = await GenAndSaveRefreshTokenAsync(tokenRow);

                await _context.SaveChangesAsync();

                return Ok(new TokenDTO
                {
                    AccesToken = newAccessToken,
                    AccessTokenExpiryTime = tokenRow.AccessTokenExpiryTime,
                    RefreshToken = newRefreshToken,
                    RefreshTokenExpiryTime = tokenRow.RefreshTokenExpiryTime
                });
            }
            return Ok();
        }
        [HttpPatch("Change Role Rendszergazda")] // müködik
        public async Task<ActionResult<CurrentUserDTO>> ChangeRoleNoAdmin(RoleNoAdminDTO role)
        {
            var changedUser = _context.Users.FirstOrDefault(x => x.Id == role.ChangedUserId);

            if (changedUser == null)
            {
                return BadRequest("Nincs ilyen felhasználó");
            }
            

            changedUser.Role = "Admin";
            _context.Users.Update(changedUser);
            await _context.SaveChangesAsync();
            var newAccessToken = CreateToken(changedUser);
            var tokenRow = await _context.Tokens.FirstOrDefaultAsync(t => t.UserDataId == changedUser.Id);

            if (tokenRow != null)
            {
                tokenRow.AccesToken = newAccessToken;
                tokenRow.AccessTokenExpiryTime = DateTime.UtcNow.Add(_accessTokenLifetime);
                _context.Tokens.Update(tokenRow);

                var newRefreshToken = await GenAndSaveRefreshTokenAsync(tokenRow);

                await _context.SaveChangesAsync();

                return Ok(new TokenDTO
                {
                    AccesToken = newAccessToken,
                    AccessTokenExpiryTime = tokenRow.AccessTokenExpiryTime,
                    RefreshToken = newRefreshToken,
                    RefreshTokenExpiryTime = tokenRow.RefreshTokenExpiryTime
                });
            }
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllUsers")] //müködik
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
