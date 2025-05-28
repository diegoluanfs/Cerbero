using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cerbero.Domain.Entities;
using Cerbero.Domain.Interfaces.Repositories;

namespace Cerbero.Application.Services
{
    public class TenantService
    {
        private readonly ITenantRepository _tenantRepository;

        public TenantService(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
        }

        public async Task<IEnumerable<Tenant>> GetAllAsync()
        {
            return await _tenantRepository.GetAllAsync();
        }

        public async Task<Tenant> GetByIdAsync(Guid id)
        {
            return await _tenantRepository.GetByIdAsync(id);
        }

        public async Task<Tenant> CreateAsync(Tenant tenant)
        {
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            tenant.Id = Guid.NewGuid();
            tenant.CreatedAt = DateTime.UtcNow;
            return await _tenantRepository.AddAsync(tenant);
        }

        public async Task<Tenant> UpdateAsync(Tenant tenant)
        {
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            return await _tenantRepository.UpdateAsync(tenant);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _tenantRepository.DeleteAsync(id);
        }
    }
}