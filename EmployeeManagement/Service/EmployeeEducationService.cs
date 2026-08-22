using EmployeeManagement.Dtos;
using EmployeeManagement.Dtos.EmployeeEduDto;
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
            Qualifications = request.Qualifications,
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
            Qualifications = employeeEducation.Qualifications,
            FieldOfStudy = employeeEducation.FieldOfStudy,
            StartDate = employeeEducation.StartDate,
            EndDate = employeeEducation.EndDate
        };

        return BaseResponseModel<CreateEmployeeEducationDto>.SuccessResponse(response, "Employee education created successfully");
    }

    public async Task<BaseResponseModel<IEnumerable<EmployeeEducationDto>>> CreateEmployeeEducationHistoryAsync(CreateEmployeeEducationHistoryDto request)
    {
        if (request == null || request.EducationHistory == null || !request.EducationHistory.Any())
        {
            return BaseResponseModel<IEnumerable<EmployeeEducationDto>>.FailureResponse("At least one education record is required");
        }
        if (request.EducationHistory.Count > 1)
        {
            var firstQualificationSet = request.EducationHistory.First().Qualifications
                .OrderBy(qualification => qualification)
                .ToList();

            var allSame = request.EducationHistory.All(entry =>
                entry.Qualifications.OrderBy(qualification => qualification).SequenceEqual(firstQualificationSet));

            if (allSame)
            {
                return BaseResponseModel<IEnumerable<EmployeeEducationDto>>.FailureResponse(
                    "Each education record should have a distinct qualification — all entries currently share the same qualification(s), which looks like a mistake.");
            }
        }

        var employeeEducations = request.EducationHistory.Select(entry => new EmployeeEducation
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            Institution = entry.Institution,
            Qualifications = entry.Qualifications,
            FieldOfStudy = entry.FieldOfStudy,
            StartDate = entry.StartDate,
            EndDate = entry.EndDate,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        await employeeEducationRepository.CreateEmployeeEducationHistoryAsync(employeeEducations);

        var response = employeeEducations.Select(employeeEducation => new EmployeeEducationDto
        {
            Id = employeeEducation.Id,
            EmployeeId = employeeEducation.EmployeeId,
            Institution = employeeEducation.Institution,
            Qualifications = employeeEducation.Qualifications,
            FieldOfStudy = employeeEducation.FieldOfStudy,
            StartDate = employeeEducation.StartDate,
            EndDate = employeeEducation.EndDate
        });

        return BaseResponseModel<IEnumerable<EmployeeEducationDto>>.SuccessResponse(response, "Employee education history created successfully");
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
                Qualifications = employeeEducation.Qualifications,
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
            Qualifications = employeeEducation.Qualifications,
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
        employeeEducation.Qualifications = request.Qualifications;
        employeeEducation.FieldOfStudy = request.FieldOfStudy;
        employeeEducation.StartDate = request.StartDate;
        employeeEducation.EndDate = request.EndDate;

        await employeeEducationRepository.UpdateEmployeeEducationAsync(employeeEducation);

        var response = new EmployeeEducationDto
        {
            Id = employeeEducation.Id,
            EmployeeId = employeeEducation.EmployeeId,
            Institution = employeeEducation.Institution,
            Qualifications = employeeEducation.Qualifications,
            FieldOfStudy = employeeEducation.FieldOfStudy,
            StartDate = employeeEducation.StartDate,
            EndDate = employeeEducation.EndDate
        };

        return BaseResponseModel<EmployeeEducationDto>.SuccessResponse(response, "Employee education updated successfully");
    }
}