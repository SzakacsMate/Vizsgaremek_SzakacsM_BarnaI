namespace backendSzM.DTOs
{
    public class LobbyInfoDTO
    {
        public string Dm { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public string TtType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PlayerLimit { get; set; }
        public int PlayerCount { get; set; }
        public string Status { get; set; }
        public int PlayerMin { get; set; }
        public string Address { get; set; }
        public List<string> Players { get; set; } = new List<string>();
    }
}
