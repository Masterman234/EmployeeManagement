using EmployeeManagement.Dtos;

namespace EmployeeManagement.Service;

public interface IEmployeeEducationService
{
    Task<BaseResponseModel<CreateEmployeeEducationDto>> CreateEmployeeEducationAsync(CreateEmployeeEducationDto request);
    Task<BaseResponseModel<EmployeeEducationDto>> GetEmployeeEducationByIdAsync(Guid id);
    Task<BaseResponseModel<IEnumerable<EmployeeEducationDto>>> GetAllEmployeeEducationsAsync();
    Task<BaseResponseModel<EmployeeEducationDto>> UpdateEmployeeEducationAsync(UpdateEmployeeEducationDto request);
    Task<BaseResponseModel<bool>> DeleteEmployeeEducationAsync(Guid id);
}