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
        [ForeignKey(nameof(KommentaloId))]
        public UserData? KommentaloUser { get; set; }

        public Guid FogadoId { get; set; }
        [ForeignKey(nameof(FogadoId))]
        public UserData? FogadoUser { get; set; }
    }
}
