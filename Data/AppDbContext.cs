using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CVManagementSystem.Models.Domain;

namespace CVManagementSystem.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Position> Positions { get; set; }
    public DbSet<PositionAttribute> PositionAttributes { get; set; }
    public DbSet<CandidateProfile> CandidateProfiles { get; set; }
    public DbSet<ProfileAttribute> ProfileAttributes { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<CV> CVs { get; set; }
    public DbSet<CVAttribute> CVAttributes { get; set; }
    public DbSet<CVProject> CVProjects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Position relationships
        modelBuilder.Entity<Position>()
            .HasOne(p => p.CreatedByUser)
            .WithMany(u => u.CreatedPositions)
            .HasForeignKey(p => p.CreatedBy)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Position>()
            .HasMany(p => p.Attributes)
            .WithMany(a => a.Positions)
            .UsingEntity("PositionPositionAttributes");

        // PositionAttribute relationships
        modelBuilder.Entity<PositionAttribute>()
            .HasOne(a => a.CreatedByUser)
            .WithMany(u => u.CreatedAttributes)
            .HasForeignKey(a => a.CreatedBy)
            .OnDelete(DeleteBehavior.Cascade);

        // CandidateProfile relationships
        modelBuilder.Entity<CandidateProfile>()
            .HasOne(cp => cp.User)
            .WithOne(u => u.CandidateProfile)
            .HasForeignKey<CandidateProfile>(cp => cp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ProfileAttribute relationships
        modelBuilder.Entity<ProfileAttribute>()
            .HasOne(pa => pa.CandidateProfile)
            .WithMany(cp => cp.ProfileAttributes)
            .HasForeignKey(pa => pa.CandidateProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProfileAttribute>()
            .HasOne(pa => pa.PositionAttribute)
            .WithMany()
            .HasForeignKey(pa => pa.PositionAttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Project relationships
        modelBuilder.Entity<Project>()
            .HasOne(p => p.User)
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // CV relationships
        modelBuilder.Entity<CV>()
            .HasOne(cv => cv.Position)
            .WithMany(p => p.CVs)
            .HasForeignKey(cv => cv.PositionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CV>()
            .HasOne(cv => cv.User)
            .WithMany(u => u.CVs)
            .HasForeignKey(cv => cv.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint: one CV per candidate per position
        modelBuilder.Entity<CV>()
            .HasIndex(cv => new { cv.UserId, cv.PositionId })
            .IsUnique();

        // CVAttribute relationships
        modelBuilder.Entity<CVAttribute>()
            .HasOne(ca => ca.CV)
            .WithMany(cv => cv.CVAttributes)
            .HasForeignKey(ca => ca.CVId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CVAttribute>()
            .HasOne(ca => ca.PositionAttribute)
            .WithMany(a => a.CVAttributes)
            .HasForeignKey(ca => ca.PositionAttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        // CVProject relationships
        modelBuilder.Entity<CVProject>()
            .HasOne(cp => cp.CV)
            .WithMany(cv => cv.CVProjects)
            .HasForeignKey(cp => cp.CVId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CVProject>()
            .HasOne(cp => cp.Project)
            .WithMany(p => p.CVProjects)
            .HasForeignKey(cp => cp.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique email constraint
        modelBuilder.Entity<ApplicationUser>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}
