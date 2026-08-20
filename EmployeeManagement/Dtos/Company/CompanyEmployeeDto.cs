using EmployeeManagement.Enums;

public class CompanyEmployeeDto
{
    public Guid EmployeeId { get; set; }
    public string FullName { get; set; }
    public Department Department { get; set; }

  //  public string Department { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }

}