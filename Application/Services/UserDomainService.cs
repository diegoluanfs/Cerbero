
using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace Application.Services;

public class UserDomainService
{
    private readonly IUserRepository _userRepository;

    public UserDomainService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task CreateUserAsync(User user)
    {
        await _userRepository.AddAsync(user);
    }
}