namespace EmployeeManagement.Models
{
    public class FileValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public FileCategory? Category { get; set; }
    }
}