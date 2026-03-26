namespace backendSzM.DTOs
{
    public class LobbyDTO
    {
       
        public string TtType { get; set; } =string.Empty;
        public string LobbyName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public DateTime StartDate { get; set; } = new DateTime();
        public int Duration { get; set; } = 0;
        public int PlayerLimit { get; set; } = 0;
        public int PlayerMin { get; set; } = 1;

    }
}
