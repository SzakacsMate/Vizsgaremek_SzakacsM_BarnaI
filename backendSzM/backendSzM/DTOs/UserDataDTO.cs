using backendSzM.Models;

namespace backendSzM.DTOs
{
    public class UserDataDTO
    {
        public string Name { get; set; }   = string.Empty;
        public string Gmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public int Rep {  get; set; } = 0;
       
        




    }
}
