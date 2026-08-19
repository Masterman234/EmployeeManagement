using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Data;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Telephone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Department)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(e => e.Gender)
        .HasConversion<string>()
            .IsRequired();  

    }
}
