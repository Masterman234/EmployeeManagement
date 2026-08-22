using EmployeeManagement.Enums;
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

        builder.Property(ee => ee.Qualifications)
             .HasConversion(
                 v => string.Join(',', v.Select(x => x.ToString())),
                 v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                       .Select(x => (Qualification)Enum.Parse(typeof(Qualification), x))
                       .ToList()
             )
             .IsRequired()
            .HasMaxLength(200);

        builder.Property(ee => ee.FieldOfStudy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ee => ee.StartDate)
            .IsRequired();

        builder.Property(ee => ee.EndDate);

        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(ee => ee.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}