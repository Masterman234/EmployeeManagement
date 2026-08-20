using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Dtos.Company
{
    public class CreateCompanyDto
    {
        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(50)]
        public string Name { get; set; }

    }
}
