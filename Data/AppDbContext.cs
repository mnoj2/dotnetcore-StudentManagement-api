using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Entities;

namespace StudentManagementApi.Data {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Mark> Marks { get; set; }
    }
}
