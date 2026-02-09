using System.ComponentModel.DataAnnotations;

namespace backendSzM.Models
{
    public class UserAuth
    {
        
        public Guid Id { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public ICollection<UserData>? UserDatas { get; set; } = new List<UserData>();
    }
}
