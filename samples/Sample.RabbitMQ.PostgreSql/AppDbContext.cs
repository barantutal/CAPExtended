using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Sample.RabbitMQ.PostgreSql;

public class Person
{
    public int Id { get; set; }

    public string Name { get; set; }

    public override string ToString()
    {
        return $"Name:{Name}, Id:{Id}";
    }
}
    
public class AppDbContext : DbContext
{
    private string _connectionString;

    public DbSet<Person> Persons { get; set; }

    public AppDbContext(IOptions<DbOptions> dbOptions)
    {
        _connectionString = dbOptions.Value.ConnectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
    }
}