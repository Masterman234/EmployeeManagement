using EmployeeManagement.Models;

namespace EmployeeManagement.Repository;

public interface IEmployeeEducationRepository
{
    Task<EmployeeEducation> CreateEmployeeEducationAsync(EmployeeEducation employeeEducation);
    Task<IEnumerable<EmployeeEducation>> CreateEmployeeEducationHistoryAsync(IEnumerable<EmployeeEducation> employeeEducations);
    Task<EmployeeEducation?> GetEmployeeEducationByIdAsync(Guid id);
    Task<IEnumerable<EmployeeEducation>> GetAllEmployeeEducationsAsync();
    Task UpdateEmployeeEducationAsync(EmployeeEducation employeeEducation);
    Task DeleteEmployeeEducationAsync(Guid id);
}