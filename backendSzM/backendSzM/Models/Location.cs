namespace backendSzM.Models
{
    public class Location
    {
        public Guid Id { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string Adress { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        
        public ICollection<Lobby>? Lobbies { get; set; } = new List<Lobby>();
    }
}
