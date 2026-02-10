using System.ComponentModel.DataAnnotations;

namespace backendSzM.Models
{
    public class BannedUser
    {
        
        public Guid Id { get; set; }
        public bool IsBanned { get; set; } = false;
        public int Warnings { get; set; } = 0;
        public ICollection<UserData>? UserDatas { get; set; } = new List<UserData>();
    }
}

 