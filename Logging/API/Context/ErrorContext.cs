using Microsoft.EntityFrameworkCore;

namespace Logs;

public class ErrorContext : DbContext
{
    public ErrorContext(DbContextOptions<ErrorContext> options)
        : base(options)
    {
    }

    public DbSet<Error> Errors { get; set; } = null!;
}