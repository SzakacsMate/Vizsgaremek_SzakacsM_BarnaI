using System.ComponentModel.DataAnnotations;

namespace backendSzM.Models
{
    public class Lobby
    {
        
        public Guid Id { get; set; }
        public string Dm { get; set; } = string.Empty;
        public string TtType { get; set; }= string.Empty;
        
        public string Location { get; set; } = string.Empty;
        public DateTime TimeDate { get; set; } = new DateTime();
        public int PlayerLimit { get; set; } = 0;
        public ICollection<LobbyCon>?LobbyCons { get; set; } = new List<LobbyCon>();
    }
}
