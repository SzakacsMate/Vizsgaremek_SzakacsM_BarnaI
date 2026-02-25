namespace backendSzM.Models
{
    public class Location
    {
        public Guid Id { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string Adress { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public Guid? LobbyId { get; set; }
        public Lobby? Lobby { get; set; }
    }
}
