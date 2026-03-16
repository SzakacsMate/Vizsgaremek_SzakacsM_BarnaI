using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backendSzM.Models
{
    public class Lobby //lobby name hozzáadása
    {
        
        public Guid Id { get; set; }

        public string Dm { get; set; } = string.Empty;
        public string LobbyName { get; set; } = string.Empty;
        public string TtType { get; set; }= string.Empty;
        public string Status { get; set; } = string.Empty;
        public string locationName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = new DateTime();
        public DateTime EndDate { get; set; } = new DateTime();
        public int PlayerLimit { get; set; } = 0;
        public int PlayerMin { get; set; } = 0;
        public int PlayerCount { get; set; }= 0;
        public string Image { get; set; } = string.Empty;
        public ICollection<LobbyCon>?LobbyCons { get; set; } = new List<LobbyCon>();
        [ForeignKey(nameof(Location))]
        public Guid? LocationId { get; set; }
        public Location? Location { get; set; }
    }
}
