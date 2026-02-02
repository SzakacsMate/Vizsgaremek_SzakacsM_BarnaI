namespace backendSzM.Models
{
    public class BannedUser
    {
        public int BanId { get; set; }
        public float IsBanned { get; set; }
        public int Warnings { get; set; }
        public ICollection<UserData> UserDatas { get; set; }
    }
}

 