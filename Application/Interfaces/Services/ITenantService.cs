using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cerbero.Domain.Entities;

namespace Cerbero.Application.Interfaces.Services
{
    public interface ITenantService
    {
        Task<IEnumerable<Tenant>> GetAllAsync();
        Task<Tenant> GetByIdAsync(Guid id);
        Task<Tenant> CreateAsync(Tenant tenant);
        Task<Tenant> UpdateAsync(Tenant tenant);
        Task DeleteAsync(Guid id);
    }
}