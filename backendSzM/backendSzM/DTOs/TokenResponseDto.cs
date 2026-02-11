namespace backendSzM.DTOs
{
    public class TokenResponseDto
    {
        public required string AccesToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
