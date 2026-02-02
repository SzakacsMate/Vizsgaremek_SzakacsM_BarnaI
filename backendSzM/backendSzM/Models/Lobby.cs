namespace backendSzM.Models
{
    public class Lobby
    {
        public int Id { get; set; }
        public string Dm { get; set; }
        public string TtType { get; set; }
        public string Location { get; set; }
        public DateTime TimeDate { get; set; }
        public int PlayerLimit { get; set; }
        public ICollection<LobbyCon>LobbyCons { get; set; }
    }
}
