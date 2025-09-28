
using Microsoft.EntityFrameworkCore;
    using TaskifyAPI.Models;
namespace TaskifyAPI.Data { 
    public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<User> Users { get; set; }
}
}
