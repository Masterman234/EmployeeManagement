namespace EmployeeManagement.Models;

using EmployeeManagement.Enums;
using EmployeeManagement.Models;

public class Employee : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }
    public string CompanyName { get; set; }
    public Department Department { get; set; }
    public Gender Gender { get; set; }
}
