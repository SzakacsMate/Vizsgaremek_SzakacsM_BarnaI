namespace backendSzM.DTOs
{
    public class LobbyDTO
    {
        
        public string TtType { get; set; } =string.Empty; 
        public string Location { get; set; }=string.Empty;
        public DateTime TimeDate { get; set; }
        public int PlayerLimit { get; set; } = 0;
    }
}
