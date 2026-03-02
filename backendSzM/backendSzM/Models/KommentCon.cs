namespace backendSzM.Models
{
    public class KommentCon
    {
        public Guid Id { get; set; }
        public Guid komment {  get; set; }
        public Komment? Komment {  get; set; }
        public Guid UserDataId { get; set; }
        public UserData? UserData { get; set; }

    }
}
