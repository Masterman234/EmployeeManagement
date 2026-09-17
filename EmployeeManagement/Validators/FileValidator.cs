using Microsoft.AspNetCore.Http;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;

namespace EmployeeManagement.Services
{
    public class FileValidator : IFileValidator
    {
        private static readonly Dictionary<string, (string MimeType, FileCategory Category)> AllowedFileTypes =
            new()
            {
                { ".jpg", ("image/jpeg", FileCategory.Image) },
                { ".jpeg", ("image/jpeg", FileCategory.Image) },
                { ".png", ("image/png", FileCategory.Image) },
                { ".webp", ("image/webp", FileCategory.Image) },

                { ".mp4", ("video/mp4", FileCategory.Video) },
                { ".webm", ("video/webm", FileCategory.Video) },
                { ".mov", ("video/quicktime", FileCategory.Video) },

                { ".pdf", ("application/pdf", FileCategory.Document) },
                { ".doc", ("application/msword", FileCategory.Document) },
                { ".docx", ("application/vnd.openxmlformats-officedocument.wordprocessingml.document", FileCategory.Document) },
                { ".xls", ("application/vnd.ms-excel", FileCategory.Document) },
                { ".xlsx", ("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileCategory.Document) },
                { ".txt", ("text/plain", FileCategory.Document) }
            };

        public FileValidationResult Validate(IFormFile file)
        {
            // 1. File must be provided
            if (file == null)
            {
                return new FileValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "File is required."
                };
            }

            // 2. File must not be empty
            if (file.Length == 0)
            {
                return new FileValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "File cannot be empty."
                };
            }

            // 3. Get the file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            // 4. Check whether the extension is supported
            if (!AllowedFileTypes.TryGetValue(extension, out var fileType))
            {
                return new FileValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "File type is not supported."
                };
            }

            // 5. Check MIME type
            if (!string.Equals(file.ContentType, fileType.MimeType, StringComparison.OrdinalIgnoreCase))
            {
                return new FileValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "File type does not match the file content type."
                };
            }

            // 6. Check maximum file size
            long maxSize = fileType.Category switch
            {
                FileCategory.Image => 5 * 1024 * 1024,
                FileCategory.Video => 50 * 1024 * 1024,
                FileCategory.Document => 10 * 1024 * 1024,
                _ => 0
            };

            if (file.Length > maxSize)
            {
                var maxSizeInMb = maxSize / (1024 * 1024);

                return new FileValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"File size exceeds the maximum allowed size of {maxSizeInMb} MB."
                };
            }

            // 7. File passed all validation
            return new FileValidationResult
            {
                IsValid = true,
                Category = fileType.Category
            };
        }
    }
}