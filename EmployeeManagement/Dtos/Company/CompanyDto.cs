namespace EmployeeManagement.Dtos.Company
{
    public class CompanyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
