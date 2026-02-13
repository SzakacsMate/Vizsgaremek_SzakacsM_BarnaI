using System.ComponentModel.DataAnnotations;

namespace backendSzM.Models
{
    public class BannedUser
    {
        
        public Guid Id { get; set; }
        public string BannedGmail {  get; set; }
        
        
    }
}

 