using EmployeeManagement.Dtos;
using EmployeeManagement.Dtos.EmployeeEduDto;
using EmployeeManagement.Enums;
using EmployeeManagement.Models;
using EmployeeManagement.Repository;


namespace EmployeeManagement.Service;

public class EmployeeEducationService(IEmployeeEducationRepository employeeEducationRepository, ILogger<EmployeeEducationService> logger) : IEmployeeEducationService
{
    public async Task<BaseResponseModel<CreateEmployeeEducationDto>> CreateEmployeeEducationAsync(CreateEmployeeEducationDto request)
    {
        
        if (request == null)
        {
            logger.LogWarning("CreateEmployeeEducationAsync called with a null request");

            return BaseResponseModel<CreateEmployeeEducationDto>.FailureResponse("Request cannot be null");
        }

        logger.LogInformation("Creating education record for employee {EmployeeId} at {Institution}",
            request.EmployeeId, request.Institution);

        var education = new EmployeeEducation
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            Institution = request.Institution,
            FieldOfStudy = request.FieldOfStudy,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var qualification in request.Qualifications)
        {
            education.Qualifications.Add(
                new EmployeeEducationQualification
                {
                    Id = Guid.NewGuid(),
                    EmployeeEducationId = education.Id,
                    Qualification = qualification,
                    CreatedAt = DateTime.UtcNow
                });
        }

        await employeeEducationRepository.CreateEmployeeEducationAsync(education);

        logger.LogInformation("Successfully created education record {EducationId} for employee {EmployeeId}",
            education.Id, education.EmployeeId);

        var response = new CreateEmployeeEducationDto
        {
            EmployeeId = education.EmployeeId,
            Institution = education.Institution,
            Qualifications = education.Qualifications
                .Select(x => x.Qualification)
                .ToHashSet(),
            FieldOfStudy = education.FieldOfStudy,
            StartDate = education.StartDate,
            EndDate = education.EndDate
        };

        return BaseResponseModel<CreateEmployeeEducationDto>.SuccessResponse(
            response,
            "Employee education created successfully");
    }

    public async Task<BaseResponseModel<IEnumerable<EmployeeEducationDto>>> CreateEmployeeEducationHistoryAsync(CreateEmployeeEducationHistoryDto request)
    {
        
        
            if (request == null || request.EducationHistory == null || !request.EducationHistory.Any())
            {
                logger.LogWarning("CreateEmployeeEducationHistoryAsync called with an empty or null education history");

                return BaseResponseModel<IEnumerable<EmployeeEducationDto>>.FailureResponse(
                    "At least one education record is required");
            }

            logger.LogInformation("Creating {EntryCount} education records for employee {EmployeeId}",
                request.EducationHistory.Count, request.EmployeeId);

            if (request.EducationHistory.Count > 1)
            {
                var firstQualificationSet = request.EducationHistory.First().Qualifications
                    .OrderBy(qualification => qualification)
                    .ToList();

                var allSame = request.EducationHistory.All(entry =>
                    entry.Qualifications
                        .OrderBy(qualification => qualification)
                        .SequenceEqual(firstQualificationSet));

                if (allSame)
                {
                    logger.LogWarning(
                        "Rejected bulk education creation for employee {EmployeeId} — all {EntryCount} entries had identical qualifications",
                        request.EmployeeId, request.EducationHistory.Count);

                    return BaseResponseModel<IEnumerable<EmployeeEducationDto>>
                        .FailureResponse("Each education record should have a distinct qualification — all entries currently share the same qualification(s), which looks like a mistake.");
                }
            }

            var employeeEducations = new List<EmployeeEducation>();

            foreach (var entry in request.EducationHistory)
            {
                var education = new EmployeeEducation
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = request.EmployeeId,
                    Institution = entry.Institution,
                    FieldOfStudy = entry.FieldOfStudy,
                    StartDate = entry.StartDate,
                    EndDate = entry.EndDate,
                    CreatedAt = DateTime.UtcNow
                };

                foreach (var qualification in entry.Qualifications)
                {
                    education.Qualifications.Add(
                        new EmployeeEducationQualification
                        {
                            Id = Guid.NewGuid(),
                            EmployeeEducationId = education.Id,
                            Qualification = qualification,
                            CreatedAt = DateTime.UtcNow
                        });
                }

                employeeEducations.Add(education);
            }

            await employeeEducationRepository.CreateEmployeeEducationHistoryAsync(employeeEducations);

            logger.LogInformation("Successfully created {EntryCount} education records for employee {EmployeeId}",
                employeeEducations.Count, request.EmployeeId);

            var response = employeeEducations.Select(employeeEducation => new EmployeeEducationDto
            {
                Id = employeeEducation.Id,
                EmployeeId = employeeEducation.EmployeeId,
                Institution = employeeEducation.Institution,
                Qualifications = employeeEducation.Qualifications
                    .Select(x => x.Qualification)
                    .ToList(),
                FieldOfStudy = employeeEducation.FieldOfStudy,
                StartDate = employeeEducation.StartDate,
                EndDate = employeeEducation.EndDate
            });

            return BaseResponseModel<IEnumerable<EmployeeEducationDto>>.SuccessResponse(
               response, "Employee education history created successfully");

        
        
        

        
    }

    public async Task<BaseResponseModel<bool>> DeleteEmployeeEducationAsync(Guid id)
    {
        try
        {
            var employeeEducation = await employeeEducationRepository.GetEmployeeEducationByIdAsync(id);

            if (employeeEducation == null)
            {
                return BaseResponseModel<bool>.FailureResponse("No employee education record found", ErrorType.NotFound);
            }

            await employeeEducationRepository.DeleteEmployeeEducationAsync(id);

            return BaseResponseModel<bool>.SuccessResponse(true, "Employee education deleted successfully");
        }
        catch (Exception ex)
        {

            logger.LogError(
             ex,
             "An error occurred while deleting employee education with ID {EmployeeEducationId}",
             id);

            return BaseResponseModel<bool>.FailureResponse("An error occurred while deleting employee education",ErrorType.InternalServerError);

        }
       
    }

    public async Task<BaseResponseModel<PagedResponse<EmployeeEducationDto>>> GetAllEmployeeEducationsAsync(int pageNumber, int pageSize)
    {
        var (employeeEducations, totalCount) = await employeeEducationRepository.GetPagedEmployeeEducationsAsync(pageNumber, pageSize);

        if (employeeEducations == null || !employeeEducations.Any())
        {
            return BaseResponseModel<PagedResponse<EmployeeEducationDto>>.FailureResponse("No employee education record found");
        }

        var items = employeeEducations.Select(employeeEducation => new EmployeeEducationDto
        {
            Id = employeeEducation.Id,
            EmployeeId = employeeEducation.EmployeeId,
            Institution = employeeEducation.Institution,
            Qualifications = employeeEducation.Qualifications
                .Select(x => x.Qualification)
                .ToList(),
            FieldOfStudy = employeeEducation.FieldOfStudy,
            StartDate = employeeEducation.StartDate,
            EndDate = employeeEducation.EndDate
        }).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var pagedResponse = new PagedResponse<EmployeeEducationDto>
        {
            Items = items,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };

        return BaseResponseModel<PagedResponse<EmployeeEducationDto>>.SuccessResponse(pagedResponse, "Employee education records retrieved successfully");
    }

    public async Task<BaseResponseModel<EmployeeEducationDto>> GetEmployeeEducationByIdAsync(Guid id)
    {
        var employeeEducation = await employeeEducationRepository.GetEmployeeEducationByIdAsync(id);

        if (employeeEducation == null)
        {
            return BaseResponseModel<EmployeeEducationDto>.FailureResponse("No employee education record found", ErrorType.NotFound);
        }

        var response = new EmployeeEducationDto
        {
            Id = employeeEducation.Id,
            EmployeeId = employeeEducation.EmployeeId,
            Institution = employeeEducation.Institution,
            Qualifications = employeeEducation.Qualifications
                .Select(x => x.Qualification)
                .ToList(),
            FieldOfStudy = employeeEducation.FieldOfStudy,
            StartDate = employeeEducation.StartDate,
            EndDate = employeeEducation.EndDate
        };

        return BaseResponseModel<EmployeeEducationDto>.SuccessResponse(response, "Employee education retrieved successfully");
    }

    public async Task<BaseResponseModel<EmployeeEducationDto>> UpdateEmployeeEducationAsync(
      UpdateEmployeeEducationDto request)
    {
        try
        {
            if (request == null)
            {
                return BaseResponseModel<EmployeeEducationDto>
                    .FailureResponse("Request cannot be null");
            }

            var employeeEducation =
                await employeeEducationRepository.GetEmployeeEducationByIdAsync(request.Id);

            if (employeeEducation == null)
            {
                return BaseResponseModel<EmployeeEducationDto>
                    .FailureResponse(
                        "No employee education record found",
                        ErrorType.NotFound);
            }

            employeeEducation.EmployeeId = request.EmployeeId;
            employeeEducation.Institution = request.Institution;
            employeeEducation.FieldOfStudy = request.FieldOfStudy;
            employeeEducation.StartDate = request.StartDate;
            employeeEducation.EndDate = request.EndDate;
            employeeEducation.ModifiedAt = DateTime.UtcNow;

            employeeEducation.Qualifications.Clear();

            foreach (var qualification in request.Qualifications)
            {
                employeeEducation.Qualifications.Add(
                    new EmployeeEducationQualification
                    {
                        Id = Guid.NewGuid(),
                        EmployeeEducationId = employeeEducation.Id,
                        Qualification = qualification,
                        CreatedAt = DateTime.UtcNow
                    });
            }

            await employeeEducationRepository.UpdateEmployeeEducationAsync(
                employeeEducation);

            var response = new EmployeeEducationDto
            {
                Id = employeeEducation.Id,
                EmployeeId = employeeEducation.EmployeeId,
                Institution = employeeEducation.Institution,
                Qualifications = employeeEducation.Qualifications
                    .Select(x => x.Qualification)
                    .ToList(),
                FieldOfStudy = employeeEducation.FieldOfStudy,
                StartDate = employeeEducation.StartDate,
                EndDate = employeeEducation.EndDate
            };

            return BaseResponseModel<EmployeeEducationDto>.SuccessResponse(
                response,
                "Employee education updated successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while updating employee education with ID {EmployeeEducationId}",
                request?.Id);

            return BaseResponseModel<EmployeeEducationDto>.FailureResponse("An error occurred while updating employee education",ErrorType.InternalServerError);
        }
    }
}