using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByTokenAsync(string token);
        Task CreateUserAsync(User user);
        string GenerateJwtToken(User user);
        string GenerateRefreshToken();
        bool ValidateRefreshToken(string refreshToken);
    }
}