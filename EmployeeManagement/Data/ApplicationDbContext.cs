using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<EmployeeEducation> EmployeeEducations { get; set; }
    public DbSet<EmployeeEducationQualification> EmployeeEducationQualifications { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<EmployeeEducationQualification>()
            .HasOne(x => x.EmployeeEducation)
            .WithMany(x => x.Qualifications)
            .HasForeignKey(x => x.EmployeeEducationId);

        modelBuilder.Entity<EmployeeEducationQualification>()
            .Property(x => x.Qualification)
            .HasConversion<string>();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();    

        modelBuilder.Entity<UserToken>()    
            .HasIndex(ut => ut.Token)
            .IsUnique();


    }
}
