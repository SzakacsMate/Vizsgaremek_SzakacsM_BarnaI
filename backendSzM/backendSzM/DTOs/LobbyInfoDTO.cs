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
    }
}
