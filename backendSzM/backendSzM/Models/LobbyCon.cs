namespace backendSzM.Models
{
    public class LobbyCon
    {
        public int UserId { get; set; }
        public int LobbyId { get; set; }
        public UserData UserData { get; set; }
        public Lobby Lobby { get; set; }
    }
}
