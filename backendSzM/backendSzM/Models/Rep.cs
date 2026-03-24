using System.ComponentModel.DataAnnotations.Schema;

namespace backendSzM.Models
{
    public class Rep
    {
        public Guid Id { get; set; }
        public int Value { get; set; }
        [ForeignKey(nameof(RepAdoUser))]
        public Guid RepAdoUserId { get; set; }
        public UserData? RepAdoUser { get; set; }


        [ForeignKey(nameof(RepFogadoUser))]
        public Guid RepFogadoUserId { get; set; }
        public UserData? RepFogadoUser { get; set; }
    }
}
