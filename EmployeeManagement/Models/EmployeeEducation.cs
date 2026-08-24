using EmployeeManagement.Enums;

namespace EmployeeManagement.Models;

public class EmployeeEducation : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public string Institution { get; set; }
    public ICollection<Qualification> Qualifications { get; set; } = new HashSet<Qualification>();
    public string FieldOfStudy { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}