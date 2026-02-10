using backendSzM.Models;

namespace backendSzM.DTOs
{
    public interface TokenResponeDto
    {
        Task<Token>RegisterAsync(TokenDTO token);
        Task<TokenResponeDto> LoginAsync(TokenDTO token);
    }
}
