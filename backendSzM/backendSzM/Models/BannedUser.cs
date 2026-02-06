using System.ComponentModel.DataAnnotations;

namespace backendSzM.Models
{
    public class BannedUser
    {
        
        public Guid Id { get; set; }
        public float IsBanned { get; set; }
        public int Warnings { get; set; }
        public ICollection<UserData>? UserDatas { get; set; } = new List<UserData>();
    }
}

 