namespace backendSzM.DTOs
{
    public class LocationDTO
    {
        public Guid Id { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string Adress { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;

    }
}
