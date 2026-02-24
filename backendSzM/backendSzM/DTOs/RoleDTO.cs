namespace backendSzM.DTOs
{
    public class RoleDTO
    {
        public Guid ChangedUser { get; set; }
        public Guid RoleChanger { get; set; }
        public string Role {  get; set; }
    }
}
