using JobNet.CoreApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobNet.CoreApi.Data;

public class JobNetDbContext : DbContext
{
    public JobNetDbContext(DbContextOptions<JobNetDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Company> Companies { get; set; }
    
    public DbSet<Experience> Experiences { get; set; }
    
    public DbSet<Education> Educations { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Skill> Skills { get; set; }
    
    public DbSet<School> Schools { get; set; }
    public DbSet<Follow> Follows { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Follow>()
            .HasOne(f => f.FollowerUser)
            .WithMany(u => u.Following)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Follow>()
            .HasOne(f => f.FollowingUser)
            .WithMany(u => u.Followers)
            .HasForeignKey(f => f.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
    
}