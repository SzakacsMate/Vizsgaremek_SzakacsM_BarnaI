namespace backendSzM.Models
{
    public class UserData
    {
        public int Id { get; set; }
        public string Hash { get; set; }
        public string Name { get; set; }
        public string Gmail { get; set; }
        public string Rep { get; set; }
       public ICollection<LobbyCon> LobbyCons {  get; set; }
        public UserAuth UserAuth { get; set; }
        public BannedUser BannedUser { get; set; }
        


    }
}
