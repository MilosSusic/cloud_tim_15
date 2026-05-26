using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Infrastructure.Persistence.AzureSql.Repositories
{
    public class SmartScaleRepository : ISmartScaleRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public SmartScaleRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(SmartScale device, CancellationToken ct = default)
        {
            await _dbContext.SmartScales.AddAsync(device, ct);
        }

        public async Task<SmartScale?> GetBySerialNumberAsync(string serialNumber, CancellationToken ct = default)
        {
            return await _dbContext.SmartScales
                .FirstOrDefaultAsync(s => s.SerialNumber == serialNumber, ct);
        }

        public async Task<SmartScale?> GetByTokenAsync(string token, CancellationToken ct = default)
        {
            return await _dbContext.SmartScales
            .FirstOrDefaultAsync(s => s.AccessToken == token, cancellationToken: ct);
        }

        public async Task<bool> IsSerialNumberUniqueAsync(string serialNumber, CancellationToken ct = default)
        {
            return !await _dbContext.SmartScales.AnyAsync(s => s.SerialNumber == serialNumber, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _dbContext.SaveChangesAsync(ct);
        }

        public void Update(SmartScale device)
        {
            _dbContext.SmartScales.Update(device);
        }
    }
}
