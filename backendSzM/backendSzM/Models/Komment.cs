using System.ComponentModel.DataAnnotations.Schema;

namespace backendSzM.Models
{
    public class Komment
    {
        public Guid Id { get; set; }
        public string KommentSzoveg { get; set; } = string.Empty;
        public string Kommentalo { get; set; } = string.Empty;
        public string Fogado { get; set; } = string.Empty;
        [ForeignKey(nameof(userData))]
        public Guid UserId { get; set; }
        public UserData? userData { get; set; }
    }
}
