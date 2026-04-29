using Microsoft.EntityFrameworkCore;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Infrastructure.Repositories
{
    public class CropsRepository : ICropsRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CropsRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Crop crop, CancellationToken ct = default)
        {
            await _dbContext.Crops.AddAsync(crop, ct);
        }

        public async Task<Crop?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbContext.Crops.FirstOrDefaultAsync(f => f.Id == id, ct);
        }

        public async Task<List<Crop>> GetByFieldIdAsync(Guid fieldId, CancellationToken ct = default)
        {
            return await _dbContext.Crops.Where(f => f.FieldId == fieldId).ToListAsync(ct);
        }
        public void Delete(Crop crop)
        {
            _dbContext.Crops.Remove(crop);
        }

        public void Update(Crop crop)
        {
            _dbContext.Crops.Update(crop);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
