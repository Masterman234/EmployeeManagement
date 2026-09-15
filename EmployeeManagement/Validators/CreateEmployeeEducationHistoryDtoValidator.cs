using EmployeeManagement.Dtos.EmployeeEduDto;
using FluentValidation;

namespace EmployeeManagement.Validators;

public class CreateEmployeeEducationHistoryDtoValidator : AbstractValidator<CreateEmployeeEducationHistoryDto>
{
    public CreateEmployeeEducationHistoryDtoValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Employee ID is required.");

        RuleFor(x => x.EducationHistory)
            .NotEmpty().WithMessage("At least one education entry is required.");

        RuleForEach(x => x.EducationHistory)
            .SetValidator(new EducationEntryDtoValidator());
    }
}

public class EducationEntryDtoValidator : AbstractValidator<EducationEntryDto>
{
    public EducationEntryDtoValidator()
    {
        RuleFor(x => x.Institution)
            .NotEmpty().WithMessage("Institution is required.")
            .MaximumLength(200);

        RuleFor(x => x.FieldOfStudy)
            .NotEmpty().WithMessage("Field of study is required.")
            .MaximumLength(150);

        RuleFor(x => x.Qualifications)
            .NotEmpty().WithMessage("At least one qualification is required.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Start date cannot be in the future.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("End date must be after the start date.");
    }
}




