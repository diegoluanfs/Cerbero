using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Obtém todos os usuários.
        /// </summary>
        /// <returns>Lista de usuários.</returns>
        Task<IEnumerable<User>> GetAllAsync();

        /// <summary>
        /// Obtém um usuário pelo ID.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <returns>Usuário correspondente ao ID.</returns>
        Task<User?> GetByIdAsync(Guid id);

        /// <summary>
        /// Obtém um usuário pelo email.
        /// </summary>
        /// <param name="email">Email do usuário.</param>
        /// <returns>Usuário correspondente ao email.</returns>
        Task<User?> GetByEmailAsync(string email, Guid tenantId);

        /// <summary>
        /// Adiciona um novo usuário.
        /// </summary>
        /// <param name="user">Usuário a ser adicionado.</param>
        Task AddAsync(User user);

        /// <summary>
        /// Atualiza os dados de um usuário.
        /// </summary>
        /// <param name="user">Usuário a ser atualizado.</param>
        Task UpdateAsync(User user);

        /// <summary>
        /// Exclui um usuário pelo ID.
        /// </summary>
        /// <param name="id">ID do usuário a ser excluído.</param>
        Task DeleteAsync(Guid id);
    }
}
