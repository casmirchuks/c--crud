using Microsoft.EntityFrameworkCore;

namespace CustomerAPI.Data
{
    public class CustomerAPIDbContext : DbContext
    {
        public CustomerAPIDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Models.Customer> Customers { get; set; }
    }
}
