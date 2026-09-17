using EmployeeManagement.Models;

namespace EmployeeManagement.Interfaces;

public interface IFileRepository
{
    Task<UploadedFile> AddAsync(UploadedFile file);
    Task<UploadedFile?> GetByIdAsync(Guid id);
    Task DeleteAsync(UploadedFile file);
    Task<(IEnumerable<UploadedFile> Files, int TotalCount)> GetPagedFilesByUserAsync(int userId, int pageNumber, int pageSize);
}