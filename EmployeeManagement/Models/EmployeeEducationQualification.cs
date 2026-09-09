using EmployeeManagement.Enums;

namespace EmployeeManagement.Models;

public class EmployeeEducationQualification : BaseEntity
{
    public Guid EmployeeEducationId { get; set; }

    public Qualification Qualification { get; set; }

    public EmployeeEducation EmployeeEducation { get; set; } = null!;
}