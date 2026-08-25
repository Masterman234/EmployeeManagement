using EmployeeManagement.Enums;
using System.ComponentModel.DataAnnotations;

public class UpdateEmployeeEducationDto
{
    [Required(ErrorMessage = "Id is required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Employee ID is required")]
    public Guid EmployeeId { get; set; }

    [Required, MaxLength(200)]
    public string Institution { get; set; }

    [Required, MinLength(1, ErrorMessage = "At least one qualification is required")]
    public HashSet<Qualification> Qualifications { get; set; } = new();

    [Required, MaxLength(150)]
    public string FieldOfStudy { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
