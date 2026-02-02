namespace backendSzM.Models
{
    public class UserAuth
    {
        public int UserAuthId { get; set; }
        public float IsAdmin { get; set; }
        public ICollection<UserData> UserDatas { get; set; }
    }
}
