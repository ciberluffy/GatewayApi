using Microsoft.EntityFrameworkCore;
using MusalaSoft.GatewayApi.Model;

namespace MusalaSoft.GatewayApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options) {}

        public DbSet<Gateway> gateways { get; set; }
        public DbSet<Device> devices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)  
        {}
    }
}