using BrainstormSessions.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace BrainstormSessions.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<BrainstormSession> BrainstormSessions { get; set; }
}
