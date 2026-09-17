using Microsoft.AspNetCore.Http;
using EmployeeManagement.Models;

namespace EmployeeManagement.Interfaces
{
    public interface IFileValidator
    {
        FileValidationResult Validate(IFormFile file);
    }
}