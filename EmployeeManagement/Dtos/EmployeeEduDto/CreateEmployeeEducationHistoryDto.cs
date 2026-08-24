using EmployeeManagement.Enums;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Dtos.EmployeeEduDto
{
    public class CreateEmployeeEducationHistoryDto
    {
        public Guid EmployeeId { get; set; }
        public List<EducationEntryDto> EducationHistory { get; set; } = new();
    }

    public class EducationEntryDto
    {
        [Required, MaxLength(200)]
        public string Institution { get; set; }

        [Required, MinLength(1, ErrorMessage = "At least one qualification is required")]
        public List<Qualification> Qualifications { get; set; } = new();

        [Required, MaxLength(150)]
        public string FieldOfStudy { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}