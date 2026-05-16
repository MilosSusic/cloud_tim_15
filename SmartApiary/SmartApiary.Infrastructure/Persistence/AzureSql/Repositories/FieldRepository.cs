using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Persistence;

namespace SmartApiary.Infrastructure.Repositories
{
    public class FieldRepository : IFieldRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public FieldRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Field field, CancellationToken ct = default)
        {
            await _dbContext.Fields.AddAsync(field, ct);
        }

        public async Task<Field?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbContext.Fields.FirstOrDefaultAsync(f => f.Id == id, ct);
        }

        public async Task<List<Field>> GetByOwnerIdAsync(Guid ownerId, CancellationToken ct = default)
        {
            return await _dbContext.Fields.Where(f => f.OwnerId == ownerId).ToListAsync(ct);
        }

        public async Task<List<Field>> GetAllWithCropsAsync(CancellationToken ct = default)
        {
            return await _dbContext.Fields.Include(f => f.Crops).ToListAsync(ct);
        }

        public void Update(Field field)
        {
            _dbContext.Fields.Update(field);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
