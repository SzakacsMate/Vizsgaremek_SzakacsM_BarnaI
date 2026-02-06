using System.ComponentModel.DataAnnotations;

namespace backendSzM.Models
{
    public class UserAuth
    {
        
        public Guid Id { get; set; }
        public float IsAdmin { get; set; }
        public ICollection<UserData>? UserDatas { get; set; } = new List<UserData>();
    }
}
