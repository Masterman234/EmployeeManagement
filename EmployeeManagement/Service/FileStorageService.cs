using Microsoft.AspNetCore.Http;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;

namespace EmployeeManagement.Services
{
    public class FileStorageService (IWebHostEnvironment environment) : IFileStorageService
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

            await using var stream = new FileStream(
                filePath,
                FileMode.Create);

            await file.CopyToAsync(stream);

            return $"/uploads/{folderName}/{storedFileName}";
        }

        public Task DeleteFileAsync(string filePath)
        {
            string physicalPath = Path.Combine(
                environment.WebRootPath,
                filePath.TrimStart('/'));

            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }

            return Task.CompletedTask;
        }
    }
}