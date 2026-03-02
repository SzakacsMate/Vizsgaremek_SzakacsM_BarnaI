using System.ComponentModel.DataAnnotations.Schema;

namespace backendSzM.Models
{
    public class Komment
    {
        public Guid Id { get; set; }
        public string KommentSzoveg { get; set; } = string.Empty;
        public string Kommentalo { get; set; } = string.Empty;

        public string Fogado { get; set; } = string.Empty;
        public Guid KommentaloId { get; set; }
        public Guid FogadoId { get; set; }
        public ICollection<KommentCon>? Komments { get; set; } = new List<KommentCon>();
        
    }
}
