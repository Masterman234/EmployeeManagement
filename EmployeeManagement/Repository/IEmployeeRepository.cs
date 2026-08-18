using EmployeeManagement.Models;

namespace EmployeeManagement.Repository;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllEmployeesAsync();
    Task<Employee> GetEmployeeByIdAsync(Guid id);
    Task<Employee> CreateEmployeeAsync(Employee employee);
    Task<bool> UpdateEmployeeAsync(Employee employee);
    Task<bool> DeleteEmployeeAsync(Guid id);
    Task<Employee> ExistByEmailAsync(string email);
    Task<Employee> ExistByPhoneAsync(string phone);
}
