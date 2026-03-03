namespace backendSzM.DTOs
{
    public class TokenDTO
    {
        
        public string? RefreshToken { get; set; }
        public string ? AccesToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime? AccessTokenExpiryTime { get; set; }


    }
}
