namespace EmployeeManagement.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile File, FileCategory category, string storedFileName);
    Task DeleteFileAsync(string filePath);
}
