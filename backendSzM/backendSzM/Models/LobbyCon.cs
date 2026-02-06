using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backendSzM.Models
{
    public class LobbyCon
    {
        [Key]
        public Guid LobbyConId { get; set; }
        [ForeignKey(nameof(UserData))]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(Lobby))]
        public Guid LobbyId { get; set; }
        public UserData? UserData { get; set; }
        public Lobby? Lobby { get; set; }
    }
}
