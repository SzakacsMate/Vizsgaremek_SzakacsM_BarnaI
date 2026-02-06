using backendSzM.Data;
using backendSzM.DTOs;
using backendSzM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backendSzM.Services
{
    
    public class AuthService(UserDataDBContext context,IConfiguration configuration) : IAuthService
    {
        
        public async Task<string?> LoginAsync(UserDataDTO request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Name == request.Name);
          //  var gmail = await context.Users.FirstOrDefaultAsync(u => u.Gmail == request.Gmail);

            if (user is null /*|| gmail is null*/)
            {
                return null;
            }
            if (new PasswordHasher<UserData>().VerifyHashedPassword(user, user.Hash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                return null;
            }
            
            return CreateToken(user);
        }

        public async Task<UserData?> RegisterAsync(UserDataDTO request)
        {
            if (await context.Users.AnyAsync(u => u.Name == request.Name))
            {
                return null;
            }
            var user = new UserData();
            var hashedPassword = new PasswordHasher<UserData>()
                .HashPassword(user, request.Password);
            user.Name = request.Name;
            user.Hash = hashedPassword;
          //  user.Gmail = request.Gmail;

            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }
        private string CreateToken(UserData user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Name),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
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
