namespace backendSzM.DTOs
{
    public class TokenDTO
    {
        public Guid Id { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
