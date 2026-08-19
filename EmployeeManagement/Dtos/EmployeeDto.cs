using EmployeeManagement.Enums;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Dtos;

public class EmployeeDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }
    public Department Department { get; set; }
    public Gender Gender { get; set; }

}


