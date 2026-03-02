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
        public string Role { get; set; } = "User";
        public int Rep { get; set; } = 0;
        public int Warnings { get; set; } = 0;
        public string ProfileI { get; set; }
        public bool IsSuspended { get; set; } = false; 

        [ForeignKey(nameof(Token))]
        public Guid? TokenId { get; set; }
        public Token? Token { get; set; }
        
        
        public ICollection<KommentCon>? Komments { get; set; } = new List<KommentCon>();



        public ICollection<LobbyCon>? LobbyCons { get; set; } = new List<LobbyCon>();

        


    }
}
