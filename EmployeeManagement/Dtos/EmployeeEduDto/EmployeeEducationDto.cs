using EmployeeManagement.Enums;

namespace EmployeeManagement.Dtos;

public class EmployeeEducationDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string Institution { get; set; }
    public Qualification Qualification { get; set; }
    public string FieldOfStudy { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}