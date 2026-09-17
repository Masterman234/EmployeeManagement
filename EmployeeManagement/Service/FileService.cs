using EmployeeManagement.Enums;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EmployeeManagement.Services;

public class FileService(
    IFileValidator fileValidator,
    IFileStorageService fileStorageService,
    IFileRepository fileRepository,
    IHttpContextAccessor httpContextAccessor) : IFileService
{
    public async Task<BaseResponseModel<FileResponseDto>> UploadFileAsync(
        UploadFileDto request)
    {
        if (request == null || request.File == null)
        {
            return BaseResponseModel<FileResponseDto>.FailureResponse("File is required.");
        }

        var validationResult = fileValidator.Validate(request.File);

        if (!validationResult.IsValid)
        {
            return BaseResponseModel<FileResponseDto>.FailureResponse(validationResult.ErrorMessage!);
        }

        // Get the logged-in user's ID from the JWT
        var userIdClaim = httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return BaseResponseModel<FileResponseDto>.FailureResponse("User authentication is required.");
        }

        var extension = Path.GetExtension(request.File.FileName);

        var storedFileName = $"{Guid.NewGuid():N}{extension}";

        // Save the physical file
        var fileUrl = await fileStorageService.SaveFileAsync(
            request.File,
            validationResult.Category!.Value,
            storedFileName);

        // Create the database entity
        var file = new UploadedFile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OriginalFileName = request.File.FileName,
            StoredFileName = storedFileName,
            FilePath = fileUrl,
            ContentType = request.File.ContentType,
            FileSize = request.File.Length,
            FileCategory = validationResult.Category.Value
        };

        Console.WriteLine($"CreatedAt before saving: {file.CreatedAt}");
        Console.WriteLine($"File Id before saving: {file.Id}");

        await fileRepository.AddAsync(file);

       

        var response = new FileResponseDto
        {
            Id = file.Id,
            OriginalFileName = file.OriginalFileName,
            ContentType = file.ContentType,
            FileSize = file.FileSize,
            FileUrl = file.FilePath,
            FileCategory = file.FileCategory,
            CreatedAt = file.CreatedAt
        };

        return BaseResponseModel<FileResponseDto>.SuccessResponse(
            response,
            "File uploaded successfully.");
    }

    public async Task<BaseResponseModel<FileResponseDto>> GetFileByIdAsync(Guid id)
    {
        var file = await fileRepository.GetByIdAsync(id);

        if (file == null)
        {
            return BaseResponseModel<FileResponseDto>.FailureResponse("File not found.", ErrorType.NotFound);
        }

        // Ownership check: only the uploader can view it
        var userIdClaim = httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId) || file.UserId != userId)
        {
            return BaseResponseModel<FileResponseDto>.FailureResponse("You do not have access to this file.", ErrorType.Forbidden);
        }

        var response = new FileResponseDto
        {
            Id = file.Id,
            OriginalFileName = file.OriginalFileName,
            ContentType = file.ContentType,
            FileSize = file.FileSize,
            FileUrl = file.FilePath,
            FileCategory = file.FileCategory,
            CreatedAt = file.CreatedAt
        };

        return BaseResponseModel<FileResponseDto>.SuccessResponse(response, "File retrieved successfully.");
    }

    public async Task<BaseResponseModel<bool>> DeleteFileAsync(Guid id)
    {
        var file = await fileRepository.GetByIdAsync(id);

        if (file == null)
        {
            return BaseResponseModel<bool>.FailureResponse("File not found.", ErrorType.NotFound);
        }

        var userIdClaim = httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId) || file.UserId != userId)
        {
            return BaseResponseModel<bool>.FailureResponse("You do not have access to delete this file.", ErrorType.Forbidden);
        }

        // Delete the physical file from disk first
        await fileStorageService.DeleteFileAsync(file.FilePath);

        // Then remove the database record
        await fileRepository.DeleteAsync(file);

        return BaseResponseModel<bool>.SuccessResponse(true, "File deleted successfully.");
    }
}

