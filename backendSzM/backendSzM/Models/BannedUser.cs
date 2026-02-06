using System.ComponentModel.DataAnnotations;

namespace backendSzM.Models
{
    public class BannedUser
    {
        [Key]
        public Guid BanId { get; set; }
        public float IsBanned { get; set; }
        public int Warnings { get; set; }
        public ICollection<UserData>? UserDatas { get; set; }
    }
}

 