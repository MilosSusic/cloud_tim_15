using SmartApiary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Interfaces.Repositories
{
    public interface IFieldRepository
    {
        Task AddAsync(Field field, CancellationToken ct = default);
        Task<Field?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<Field>> GetByOwnerIdAsync(Guid ownerId, CancellationToken ct = default);
        void Update(Field field);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
