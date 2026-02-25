namespace backendSzM.DTOs
{
    public class LobbyDTO
    {
        public string Dm { get; set; } = string.Empty;
        public string TtType { get; set; } =string.Empty; 
        
        
        public DateTime TimeDate { get; set; } = new DateTime();
        public int PlayerLimit { get; set; } = 0;
        public string Image { get; set; } = string.Empty;

    }
}
