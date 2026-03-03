using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backendSzM.Models
{
    public class Token
    {
        
        public Guid Id { get; set; }
        
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime? AccessTokenExpiryTime { get; set; }

        public string? AccesToken {get; set;  }

        
        [ForeignKey(nameof(UserData))]
        public Guid? UserDataId { get; set; }
        public UserData? UserData { get; set; }
    }
}
