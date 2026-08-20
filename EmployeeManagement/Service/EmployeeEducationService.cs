using EmployeeManagement.Dtos;
using EmployeeManagement.Models;
using EmployeeManagement.Repository;

namespace EmployeeManagement.Service;

public class EmployeeEducationService(IEmployeeEducationRepository employeeEducationRepository) : IEmployeeEducationService
{
    public async Task<BaseResponseModel<CreateEmployeeEducationDto>> CreateEmployeeEducationAsync(CreateEmployeeEducationDto request)
    {
        if (request == null)
        {
            return BaseResponseModel<CreateEmployeeEducationDto>.FailureResponse("Request cannot be null");
        }

        var employeeEducation = new EmployeeEducation
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            Institution = request.Institution,
            Qualification = request.Qualification,
            FieldOfStudy = request.FieldOfStudy,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            CreatedAt = DateTime.UtcNow
        };

        await employeeEducationRepository.CreateEmployeeEducationAsync(employeeEducation);

        var response = new CreateEmployeeEducationDto
        {
            EmployeeId = employeeEducation.EmployeeId,
            Institution = employeeEducation.Institution,
            Qualification = employeeEducation.Qualification,
            FieldOfStudy = employeeEducation.FieldOfStudy,
            StartDate = employeeEducation.StartDate,
            EndDate = employeeEducation.EndDate
        };

        return BaseResponseModel<CreateEmployeeEducationDto>.SuccessResponse(response, "Employee education created successfully");
    }

    public async Task<BaseResponseModel<bool>> DeleteEmployeeEducationAsync(Guid id)
    {
        var employeeEducation = await employeeEducationRepository.GetEmployeeEducationByIdAsync(id);
        if (employeeEducation == null)
        {
            return BaseResponseModel<bool>.FailureResponse("Employee education not found");
        }

        await employeeEducationRepository.DeleteEmployeeEducationAsync(id);

        return BaseResponseModel<bool>.SuccessResponse(true, "Employee education deleted successfully");
    }

    public async Task<BaseResponseModel<IEnumerable<EmployeeEducationDto>>> GetAllEmployeeEducationsAsync()
    {
        var employeeEducations = await employeeEducationRepository.GetAllEmployeeEducationsAsync();

        if (employeeEducations == null || !employeeEducations.Any())
        {
            return BaseResponseModel<IEnumerable<EmployeeEducationDto>>.FailureResponse("No employee education records found");
        }

        var response = new List<EmployeeEducationDto>();

        foreach (var employeeEducation in employeeEducations)
        {
            response.Add(new EmployeeEducationDto
            {
                Id = employeeEducation.Id,
                EmployeeId = employeeEducation.EmployeeId,
                Institution = employeeEducation.Institution,
                Qualification = employeeEducation.Qualification,
                FieldOfStudy = employeeEducation.FieldOfStudy,
                StartDate = employeeEducation.StartDate,
                EndDate = employeeEducation.EndDate
            });
        }

        return BaseResponseModel<IEnumerable<EmployeeEducationDto>>.SuccessResponse(response, "Employee education records retrieved successfully");
    }

    public async Task<BaseResponseModel<EmployeeEducationDto>> GetEmployeeEducationByIdAsync(Guid id)
    {
        var employeeEducation = await employeeEducationRepository.GetEmployeeEducationByIdAsync(id);

        if (employeeEducation == null)
        {
            return BaseResponseModel<EmployeeEducationDto>.FailureResponse("No employee education record found");
        }

        var response = new EmployeeEducationDto
        {
            Id = employeeEducation.Id,
            EmployeeId = employeeEducation.EmployeeId,
            Institution = employeeEducation.Institution,
            Qualification = employeeEducation.Qualification,
            FieldOfStudy = employeeEducation.FieldOfStudy,
            StartDate = employeeEducation.StartDate,
            EndDate = employeeEducation.EndDate
        };

        return BaseResponseModel<EmployeeEducationDto>.SuccessResponse(response, "Employee education retrieved successfully");
    }

    public async Task<BaseResponseModel<EmployeeEducationDto>> UpdateEmployeeEducationAsync(UpdateEmployeeEducationDto request)
    {
        var employeeEducation = await employeeEducationRepository.GetEmployeeEducationByIdAsync(request.Id);
        if (employeeEducation == null)
        {
            return BaseResponseModel<EmployeeEducationDto>.FailureResponse("Employee education not found");
        }

        employeeEducation.EmployeeId = request.EmployeeId;
        employeeEducation.Institution = request.Institution;
        employeeEducation.Qualification = request.Qualification;
        employeeEducation.FieldOfStudy = request.FieldOfStudy;
        employeeEducation.StartDate = request.StartDate;
        employeeEducation.EndDate = request.EndDate;

        await employeeEducationRepository.UpdateEmployeeEducationAsync(employeeEducation);

        var response = new EmployeeEducationDto
        {
            Id = employeeEducation.Id,
            EmployeeId = employeeEducation.EmployeeId,
            Institution = employeeEducation.Institution,
            Qualification = employeeEducation.Qualification,
            FieldOfStudy = employeeEducation.FieldOfStudy,
            StartDate = employeeEducation.StartDate,
            EndDate = employeeEducation.EndDate
        };

        return BaseResponseModel<EmployeeEducationDto>.SuccessResponse(response, "Employee education updated successfully");
    }
}