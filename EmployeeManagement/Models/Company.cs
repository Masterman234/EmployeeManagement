namespace EmployeeManagement.Models;

public class Company : BaseEntity
{
    public string Name { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();

}
