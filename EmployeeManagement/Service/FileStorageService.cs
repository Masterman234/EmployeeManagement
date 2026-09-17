using Microsoft.AspNetCore.Http;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;

namespace EmployeeManagement.Services
{
    public class FileStorageService(
        IWebHostEnvironment environment,
        ILogger<FileStorageService> logger) : IFileStorageService
    {
        public async Task<string> SaveFileAsync(
            IFormFile file,
            FileCategory category,
            string storedFileName)
        {
            string folderName = category switch
            {
                FileCategory.Image => "images",
                FileCategory.Video => "videos",
                FileCategory.Document => "documents",
                _ => throw new ArgumentException("Invalid file category.")
            };

            string uploadFolder = Path.Combine(
                environment.WebRootPath,
                "uploads",
                folderName);

            Directory.CreateDirectory(uploadFolder);

            string filePath = Path.Combine(
                uploadFolder,
                storedFileName);

            try
            {
                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                logger.LogInformation(
                    "Saved file {StoredFileName} to {FilePath} ({FileSize} bytes)",
                    storedFileName, filePath, file.Length);

                return $"/uploads/{folderName}/{storedFileName}";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save file {StoredFileName} to {FilePath}", storedFileName, filePath);
                throw;
            }
        }

        public Task DeleteFileAsync(string filePath)
        {
            string physicalPath = Path.Combine(
                environment.WebRootPath,
                filePath.TrimStart('/'));

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
                logger.LogInformation("Deleted physical file at {PhysicalPath}", physicalPath);
            }
            else
            {
                logger.LogWarning("Attempted to delete file at {PhysicalPath}, but it did not exist on disk", physicalPath);
            }

            return Task.CompletedTask;
        }
    }
}