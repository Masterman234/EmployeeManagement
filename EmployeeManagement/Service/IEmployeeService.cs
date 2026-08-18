using EmployeeManagement.Dtos;

namespace EmployeeManagement.Service;

public interface IEmployeeService
{
    Task<BaseResponseModel<CreateEmployeeDto>> CreateEmployeeAsync(CreateEmployeeDto request);
    Task<BaseResponseModel<EmployeeDto>> GetEmployeeByIdAsync(Guid id);
    Task<BaseResponseModel<IEnumerable<EmployeeDto>>> GetAllEmployeesAsync();
    Task<BaseResponseModel<EmployeeDto>> UpdateEmployeeAsync(EmployeeDto request);
    Task<BaseResponseModel<bool>> DeleteEmployeeAsync(Guid id);
}
