using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cerbero.Domain.Entities;

namespace Cerbero.Domain.Interfaces.Repositories
{
    public interface ITenantRepository
    {
        Task<IEnumerable<Tenant>> GetAllAsync();
        Task<Tenant> GetByIdAsync(Guid id);
        Task<Tenant> AddAsync(Tenant tenant);
        Task<Tenant> UpdateAsync(Tenant tenant);
        Task DeleteAsync(Guid id);
    }
}