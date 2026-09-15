using EmployeeManagement.Enums;

public class UpdateEmployeeEducationDto
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string Institution { get; set; } = string.Empty;     

    public HashSet<Qualification> Qualifications { get; set; } = new();

    public string FieldOfStudy { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
