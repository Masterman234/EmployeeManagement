using System.ComponentModel.DataAnnotations;
using EmployeeManagement.Enums;

namespace EmployeeManagement.Dtos;

public class CreateEmployeeEducationDto
{
    [Required]
    public Guid EmployeeId { get; set; }

    [Required, MaxLength(200)]
    public string Institution { get; set; }

    [Required]
    public Qualification Qualification { get; set; }

    [Required, MaxLength(150)]
    public string FieldOfStudy { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}