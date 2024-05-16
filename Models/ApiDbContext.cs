using Microsoft.EntityFrameworkCore;
using UrlShort.Models;

namespace UrlShort.Models;

public class ApiDbContext : DbContext
{
    private readonly IConfiguration Configuration;
    public virtual DbSet<ShortUrl> Urls { get; set; }
    public ApiDbContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var connectionString = Configuration.GetConnectionString("DefaultConnection");

        base.OnConfiguring(options);
        options.UseSqlite(connectionString);
    }
}