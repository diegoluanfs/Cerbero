using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task AddAsync(User user)
    {
        // Verificar se o email já está em uso
        var existingUser = await _userRepository.GetByEmailAsync(user.Email, user.TenantId);
        if (existingUser != null)
        {
            throw new InvalidOperationException("O email já está em uso.");
        }

        // Adicionar o novo usuário
        await _userRepository.AddAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        var existingUser = await _userRepository.GetByIdAsync(user.Id);
        if (existingUser == null)
        {
            throw new InvalidOperationException("Usuário não encontrado.");
        }

        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteAsync(Guid id)
    {
        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null)
        {
            throw new InvalidOperationException("Usuário não encontrado.");
        }

        await _userRepository.DeleteAsync(id);
    }
}