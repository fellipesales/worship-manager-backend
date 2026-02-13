using Microsoft.EntityFrameworkCore;
using WorshipManager.Core.Entities;
using WorshipManager.Core.Interfaces;
using WorshipManager.Infrastructure.Data;

namespace WorshipManager.Infrastructure.Repositories;

public class MessageTemplateRepository : Repository<MessageTemplate>, IMessageTemplateRepository
{
    public MessageTemplateRepository(ApplicationDbContext context) : base(context) { }

    public async Task<MessageTemplate?> GetByTypeAsync(string type) =>
        await _dbSet.FirstOrDefaultAsync(t => t.Type == type && t.IsActive);

    public async Task<IEnumerable<MessageTemplate>> GetActiveTemplatesAsync() =>
        await _dbSet.Where(t => t.IsActive).OrderBy(t => t.Name).ToListAsync();
}
