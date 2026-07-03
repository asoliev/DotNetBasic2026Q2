using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainstormSessions.Core.Interfaces;
using BrainstormSessions.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace BrainstormSessions.Infrastructure;

public class EFStormSessionRepository(AppDbContext dbContext) : IBrainstormSessionRepository
{
    public Task<BrainstormSession> GetByIdAsync(int id)
    {
        return dbContext.BrainstormSessions
            .Include(s => s.Ideas)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public Task<List<BrainstormSession>> ListAsync()
    {
        return dbContext.BrainstormSessions
            .Include(s => s.Ideas)
            .OrderByDescending(s => s.DateCreated)
            .ToListAsync();
    }

    public Task AddAsync(BrainstormSession session)
    {
        dbContext.BrainstormSessions.Add(session);
        return dbContext.SaveChangesAsync();
    }

    public Task UpdateAsync(BrainstormSession session)
    {
        dbContext.Entry(session).State = EntityState.Modified;
        return dbContext.SaveChangesAsync();
    }
}
