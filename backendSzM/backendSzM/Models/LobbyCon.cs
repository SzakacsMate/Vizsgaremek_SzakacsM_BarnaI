using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backendSzM.Models
{
    public class LobbyCon
    {
        
        public Guid Id { get; set; }

        
        

        
       
        public Guid UserId { get; set; }
        public UserData? UserData { get; set; }
        public Guid LobbyId { get; set; }
        public Lobby? Lobby { get; set; }
    }
}
