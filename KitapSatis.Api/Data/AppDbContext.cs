using Microsoft.EntityFrameworkCore;
using KitapSatis.Api.Models;

namespace KitapSatis.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;


    }
}
