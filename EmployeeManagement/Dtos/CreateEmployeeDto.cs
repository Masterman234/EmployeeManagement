using EmployeeManagement.Enums;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Dtos;

public class CreateEmployeeDto
{
    [Required(ErrorMessage = "First name is required.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required."), StringLength(50)]
    public string Email { get; set; }

    [Required(ErrorMessage = "Telephone is required."), StringLength(15)]
    public string Telephone { get; set; }
    public Guid CompanyId { get; set; }

    [Required(ErrorMessage = "Department is required.")]
    public Department Department { get; set; }

    [Required(ErrorMessage = "Gender is required.")]
    public Gender Gender { get; set; }
}


