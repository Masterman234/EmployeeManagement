using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EmployeeManagement.Data;

public class EmployeeEducationConfiguration : IEntityTypeConfiguration<EmployeeEducation>
{
    public void Configure(EntityTypeBuilder<EmployeeEducation> builder)
    {
        builder.ToTable("EmployeeEducations");
        builder.Property(ee => ee.Institution)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(ee => ee.Qualification)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ee => ee.FieldOfStudy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ee => ee.StartDate)
            .IsRequired();

        builder.Property(ee => ee.EndDate)
            .IsRequired();
        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(ee => ee.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
