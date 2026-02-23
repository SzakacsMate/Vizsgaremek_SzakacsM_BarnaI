namespace backendSzM.Models
{
    public class Komment
    {
        public Guid Id { get; set; }
        public string KommentSzoveg { get; set; } = string.Empty;
        public string Kommentalo { get; set; } = string.Empty;
        public string Fogado { get; set; } = string.Empty;
       // public ICollection<UserData>? UserDatas { get; set; } = new List<UserData>();
    }
}
