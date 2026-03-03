using System.ComponentModel.DataAnnotations.Schema;

namespace backendSzM.Models
{
    public class Komment
    {
        public Guid Id { get; set; }
        public string KommentSzoveg { get; set; } = string.Empty;
       
        
        [ForeignKey(nameof(KommenteloUser))]
        public Guid KommentaloUserId { get; set; }
        public UserData? KommenteloUser { get; set; }


        [ForeignKey(nameof(FogadoUser))]
        public Guid FogadoUserId { get; set; }
        public UserData? FogadoUser { get; set; }


    }
}
