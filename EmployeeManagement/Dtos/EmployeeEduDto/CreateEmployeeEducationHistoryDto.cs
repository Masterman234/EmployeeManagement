using EmployeeManagement.Enums;
namespace EmployeeManagement.Dtos.EmployeeEduDto;


public class CreateEmployeeEducationHistoryDto
{
    public Guid EmployeeId { get; set; }

    public List<EducationEntryDto> EducationHistory { get; set; } = new();
}

public class EducationEntryDto
{
    public string Institution { get; set; } = string.Empty;

    public HashSet<Qualification> Qualifications { get; set; } = new();

    public string FieldOfStudy { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}