using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backendSzM.Models
{
    public class UserData
    {
        
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Hash { get; set; } = string.Empty;
        public string Gmail { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int Rep { get; set; } = 0;
        public int Warnings { get; set; } = 0;

        public Guid? BannedId { get; set; }
        public BannedUser? BannedUser { get; set; }

        [ForeignKey(nameof(Token))]
        public Guid? TokenId { get; set; }
        public Token? Token { get; set; }
        /*
        public Guid KommentId { get; set; }
        public Komment? Komment { get; set; }*/



        public ICollection<LobbyCon>? LobbyCons { get; set; } = new List<LobbyCon>();

        


    }
}
