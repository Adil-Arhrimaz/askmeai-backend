using Microsoft.EntityFrameworkCore;
using AskMeAI.API.DbContexts;

public class DbContextFixture : IDisposable
{
    public DbContextOptions<AskMeAiDbContext> Options { get; private set; }

    public DbContextFixture()
    {
        Options = new DbContextOptionsBuilder<AskMeAiDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

    }

    public AskMeAiDbContext CreateContext()
    {
        var context = new AskMeAiDbContext(Options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();
        context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;");
        return context;
    }

    public void Dispose()
    {
        
    }
}
