using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backendSzM.Models
{
    public class UserData
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Hash { get; set; } = string.Empty;
        public string Gmail { get; set; } = string.Empty;

        public int Rep { get; set; } = 0;
        public ICollection<LobbyCon>? LobbyCons { get; set; }
        [ForeignKey(nameof(UserAuth))]
        public Guid AuthId { get; set; }
        public UserAuth? UserAuth { get; set; }
        [ForeignKey(nameof(BannedUser))]
        public Guid BannedId { get; set; }
        public BannedUser? BannedUser { get; set; }
        


    }
}
