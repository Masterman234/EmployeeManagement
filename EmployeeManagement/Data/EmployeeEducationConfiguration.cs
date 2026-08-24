using EmployeeManagement.Enums;
using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        var qualificationsComparer = new ValueComparer<ICollection<Qualification>>(
            (a, b) => a!.SequenceEqual(b!),
            c => c.Aggregate(0, (hash, val) => HashCode.Combine(hash, val.GetHashCode())),
            c => c.ToHashSet()
        );

        builder.Property(ee => ee.Qualifications)
            .HasConversion(
                v => string.Join(',', v.Select(x => x.ToString())),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(x => (Qualification)Enum.Parse(typeof(Qualification), x))
                      .ToHashSet()
            )
            .Metadata.SetValueComparer(qualificationsComparer);

        builder.Property(ee => ee.Qualifications)
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