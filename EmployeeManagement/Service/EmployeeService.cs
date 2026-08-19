using EmployeeManagement.Dtos;
using EmployeeManagement.Models;
using EmployeeManagement.Repository;

namespace EmployeeManagement.Service;

public class EmployeeService(IEmployeeRepository employeeRepository) : IEmployeeService
{
    public async Task<BaseResponseModel<CreateEmployeeDto>> CreateEmployeeAsync(CreateEmployeeDto request)
    {
        if (request == null)
        {
            return BaseResponseModel<CreateEmployeeDto>.FailureResponse("Request cannot be null");
        }

        var existingEmployee = await employeeRepository.ExistByEmailAsync(request.Email);
        if (existingEmployee != null)
        {
            return BaseResponseModel<CreateEmployeeDto>.FailureResponse("Employee with the same email already exists");
        }

        var phone = await employeeRepository.ExistByPhoneAsync(request.Telephone);
        if (phone != null)
        {
            return BaseResponseModel<CreateEmployeeDto>.FailureResponse("Employee with the same phone number already exists");
        }

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Telephone = request.Telephone,
            Department = request.Department,
            Gender = request.Gender,
            CreatedAt = DateTime.UtcNow

        };
        await employeeRepository.CreateEmployeeAsync(employee);

        var response = new CreateEmployeeDto
        {
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Telephone = employee.Telephone,
            Department = employee.Department,
            Gender = employee.Gender
        };

        return BaseResponseModel<CreateEmployeeDto>.SuccessResponse(response, "Employee created successfully");
    }

    public async Task<BaseResponseModel<bool>> DeleteEmployeeAsync(Guid id)
    {
        var employee = employeeRepository.GetEmployeeByIdAsync(id);
        if (employee == null)
        {
            return BaseResponseModel<bool>.FailureResponse("Employee not found");
        }
        await employeeRepository.DeleteEmployeeAsync(id);

        return BaseResponseModel<bool>.SuccessResponse(true, "Employee deleted successfully");
    }

    public async Task<BaseResponseModel<IEnumerable<EmployeeDto>>> GetAllEmployeesAsync()
    {
        var employees = await employeeRepository.GetAllEmployeesAsync();

        if (employees == null || !employees.Any())
        {
            return BaseResponseModel<IEnumerable<EmployeeDto>>.FailureResponse("No employees found");
        }

        var response = new List<EmployeeDto>();

        foreach (var employee in employees)
        {
            response.Add(new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Telephone = employee.Telephone,
                Department = employee.Department,
                Gender = employee.Gender,
            });
        }

        return BaseResponseModel<IEnumerable<EmployeeDto>>.SuccessResponse(response, "Employees retrieved successfully");
    }

    public async Task<BaseResponseModel<EmployeeDto>> GetEmployeeByIdAsync(Guid id)
    {
        var employee = await employeeRepository.GetEmployeeByIdAsync(id);

        if (employee == null)
        {
            return BaseResponseModel<EmployeeDto>.FailureResponse("No employees found");
        }

        var response = new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Telephone = employee.Telephone,
            Department = employee.Department,
            Gender = employee.Gender,
        };

        return BaseResponseModel<EmployeeDto>.SuccessResponse(response, "Employee retrieved successfully");

    }

    public async Task<BaseResponseModel<EmployeeDto>> UpdateEmployeeAsync(EmployeeDto request)
    {
        var employee = await employeeRepository.GetEmployeeByIdAsync(request.Id);
        if (employee == null)
        {
            return BaseResponseModel<EmployeeDto>.FailureResponse("Employee not found");
        }

        employee.Id = request.Id;
        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;
        employee.Email = request.Email;
        employee.Email = request.Email;
        employee.Telephone = request.Telephone;
        employee.Department = request.Department;
        employee.Gender = request.Gender;

        await employeeRepository.UpdateEmployeeAsync(employee);

        var employeeDto = new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Telephone = employee.Telephone,
            Department = employee.Department,
            Gender = employee.Gender
        };

        return BaseResponseModel<EmployeeDto>.SuccessResponse(request, "Employee updated successfully");

    }

}
