using EmployeeManagement.Enums;

namespace EmployeeManagement.Dtos;

public class UpdateEmployeeEducationDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string Institution { get; set; }
    public List<Qualification> Qualifications { get; set; } = new();
    public string FieldOfStudy { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}