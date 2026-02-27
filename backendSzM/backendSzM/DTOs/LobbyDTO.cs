namespace backendSzM.DTOs
{
    public class LobbyDTO
    {
       
        public string TtType { get; set; } =string.Empty;

        public DateTime StartDate { get; set; } = new DateTime();
        public DateTime EndDate { get; set; } = new DateTime();
        public int PlayerLimit { get; set; } = 0;

    }
}
