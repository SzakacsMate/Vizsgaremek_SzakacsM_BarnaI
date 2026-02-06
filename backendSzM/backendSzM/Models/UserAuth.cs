using System.ComponentModel.DataAnnotations;

namespace backendSzM.Models
{
    public class UserAuth
    {
        [Key]
        public Guid UserAuthId { get; set; }
        public float IsAdmin { get; set; }
        public ICollection<UserData>? UserDatas { get; set; }
    }
}
