using System.ComponentModel.DataAnnotations;

namespace backendSzM.Models
{
    public class Token
    {
        
        public Guid Id { get; set; }
        
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
      
        public ICollection<UserData>? UserDatas { get; set; } = new List<UserData>();
    }
}
