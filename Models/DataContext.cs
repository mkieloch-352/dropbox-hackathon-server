using Microsoft.EntityFrameworkCore;

namespace webapi.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Contract> Contracts { get; set; }
    }
}
