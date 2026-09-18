using EmployeeManagement.Dtos;
using EmployeeManagement.Enums;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EmployeeManagement.Services
{
    public class FileService(
        IFileValidator fileValidator,
        IFileStorageService fileStorageService,
        IFileRepository fileRepository,
        IHttpContextAccessor httpContextAccessor,
        ILogger<FileService> logger) : IFileService
    {
        public async Task<BaseResponseModel<FileResponseDto>> UploadFileAsync(UploadFileDto request)
        {
            try
            {
                if (request == null || request.File == null)
                {
                    logger.LogWarning("UploadFileAsync called with a null file");
                    return BaseResponseModel<FileResponseDto>.FailureResponse("File is required.");
                }

                var validationResult = fileValidator.Validate(request.File);

                if (!validationResult.IsValid)
                {
                    logger.LogWarning("File validation failed for {FileName}: {Error}",
                        request.File.FileName, validationResult.ErrorMessage);

                    return BaseResponseModel<FileResponseDto>.FailureResponse(validationResult.ErrorMessage!);
                }

                var userIdClaim = httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(userIdClaim, out var userId))
                {
                    logger.LogWarning("UploadFileAsync attempted without a valid authenticated user");
                    return BaseResponseModel<FileResponseDto>.FailureResponse("User authentication is required.");
                }

                var extension = Path.GetExtension(request.File.FileName);
                var storedFileName = $"{Guid.NewGuid():N}{extension}";

                var fileUrl = await fileStorageService.SaveFileAsync(
                    request.File,
                    validationResult.Category!.Value,
                    storedFileName);

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

                await fileRepository.AddAsync(file);

                logger.LogInformation(
                    "User {UserId} uploaded file {FileId} ({OriginalFileName}, {FileSize} bytes, {Category})",
                    userId, file.Id, file.OriginalFileName, file.FileSize, file.FileCategory);

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

                return BaseResponseModel<FileResponseDto>.SuccessResponse(response, "File uploaded successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while uploading file {FileName}", request?.File?.FileName);

                return BaseResponseModel<FileResponseDto>.FailureResponse(
                    "An error occurred while uploading the file", ErrorType.InternalServerError);
            }
        }

        public async Task<BaseResponseModel<List<FileResponseDto>>> UploadMultipleFilesAsync(UploadMultipleFilesDto request)
        {
            try
            {
                if (request == null || request.Files == null || !request.Files.Any())
                {
                    logger.LogWarning("UploadMultipleFilesAsync called with no files");
                    return BaseResponseModel<List<FileResponseDto>>.FailureResponse("At least one file is required.");
                }

                var userIdClaim = httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(userIdClaim, out var userId))
                {
                    logger.LogWarning("UploadMultipleFilesAsync attempted without a valid authenticated user");
                    return BaseResponseModel<List<FileResponseDto>>.FailureResponse("User authentication is required.");
                }

                logger.LogInformation("User {UserId} attempting to upload {FileCount} files", userId, request.Files.Count);

                var responses = new List<FileResponseDto>();

                foreach (var formFile in request.Files)
                {
                    var validationResult = fileValidator.Validate(formFile);

                    if (!validationResult.IsValid)
                    {
                        logger.LogWarning(
                            "Batch upload rejected for user {UserId}: {FileName} failed validation — {Error}",
                            userId, formFile.FileName, validationResult.ErrorMessage);

                        return BaseResponseModel<List<FileResponseDto>>.FailureResponse(
                            $"{formFile.FileName}: {validationResult.ErrorMessage}");
                    }

                    var extension = Path.GetExtension(formFile.FileName);
                    var storedFileName = $"{Guid.NewGuid():N}{extension}";

                    var fileUrl = await fileStorageService.SaveFileAsync(
                        formFile,
                        validationResult.Category!.Value,
                        storedFileName);

                    var file = new UploadedFile
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        OriginalFileName = formFile.FileName,
                        StoredFileName = storedFileName,
                        FilePath = fileUrl,
                        ContentType = formFile.ContentType,
                        FileSize = formFile.Length,
                        FileCategory = validationResult.Category.Value
                    };

                    await fileRepository.AddAsync(file);

                    responses.Add(new FileResponseDto
                    {
                        Id = file.Id,
                        OriginalFileName = file.OriginalFileName,
                        ContentType = file.ContentType,
                        FileSize = file.FileSize,
                        FileUrl = file.FilePath,
                        FileCategory = file.FileCategory,
                        CreatedAt = file.CreatedAt
                    });
                }

                logger.LogInformation("User {UserId} successfully uploaded {FileCount} files", userId, responses.Count);

                return BaseResponseModel<List<FileResponseDto>>.SuccessResponse(
                    responses, $"{responses.Count} file(s) uploaded successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while uploading multiple files");

                return BaseResponseModel<List<FileResponseDto>>.FailureResponse(
                    "An error occurred while uploading the files", ErrorType.InternalServerError);
            }
        }

        public async Task<BaseResponseModel<FileResponseDto>> GetFileByIdAsync(Guid id)
        {
            try
            {
                var file = await fileRepository.GetByIdAsync(id);

                if (file == null)
                {
                    logger.LogWarning("GetFileByIdAsync: file {FileId} not found", id);
                    return BaseResponseModel<FileResponseDto>.FailureResponse("File not found.", ErrorType.NotFound);
                }

                var userIdClaim = httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(userIdClaim, out var userId) || file.UserId != userId)
                {
                    logger.LogWarning(
                        "User {AttemptedUserId} attempted to access file {FileId} owned by user {OwnerUserId}",
                        userIdClaim, id, file.UserId);

                    return BaseResponseModel<FileResponseDto>.FailureResponse(
                        "You do not have access to this file.", ErrorType.Forbidden);
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
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving file {FileId}", id);

                return BaseResponseModel<FileResponseDto>.FailureResponse(
                    "An error occurred while retrieving the file", ErrorType.InternalServerError);
            }
        }

        public async Task<BaseResponseModel<bool>> DeleteFileAsync(Guid id)
        {
            try
            {
                var file = await fileRepository.GetByIdAsync(id);

                if (file == null)
                {
                    logger.LogWarning("DeleteFileAsync: file {FileId} not found", id);
                    return BaseResponseModel<bool>.FailureResponse("File not found.", ErrorType.NotFound);
                }

                var userIdClaim = httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(userIdClaim, out var userId) || file.UserId != userId)
                {
                    logger.LogWarning(
                        "User {AttemptedUserId} attempted to delete file {FileId} owned by user {OwnerUserId}",
                        userIdClaim, id, file.UserId);

                    return BaseResponseModel<bool>.FailureResponse(
                        "You do not have access to delete this file.", ErrorType.Forbidden);
                }

                await fileStorageService.DeleteFileAsync(file.FilePath);
                await fileRepository.DeleteAsync(file);

                logger.LogInformation("User {UserId} deleted file {FileId} ({OriginalFileName})",
                    userId, file.Id, file.OriginalFileName);

                return BaseResponseModel<bool>.SuccessResponse(true, "File deleted successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while deleting file {FileId}", id);

                return BaseResponseModel<bool>.FailureResponse(
                    "An error occurred while deleting the file", ErrorType.InternalServerError);
            }
        }

        public async Task<BaseResponseModel<PagedResponse<FileResponseDto>>> GetAllFilesAsync(int pageNumber, int pageSize)
        {
            try
            {
                var userIdClaim = httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(userIdClaim, out var userId))
                {
                    logger.LogWarning("GetAllFilesAsync attempted without a valid authenticated user");
                    return BaseResponseModel<PagedResponse<FileResponseDto>>.FailureResponse("User authentication is required.");
                }

                var (files, totalCount) = await fileRepository.GetPagedFilesByUserAsync(userId, pageNumber, pageSize);

                var items = files.Select(file => new FileResponseDto
                {
                    Id = file.Id,
                    OriginalFileName = file.OriginalFileName,
                    ContentType = file.ContentType,
                    FileSize = file.FileSize,
                    FileUrl = file.FilePath,
                    FileCategory = file.FileCategory,
                    CreatedAt = file.CreatedAt
                }).ToList();

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var pagedResponse = new PagedResponse<FileResponseDto>
                {
                    Items = items,
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages
                };

                return BaseResponseModel<PagedResponse<FileResponseDto>>.SuccessResponse(
                    pagedResponse, "Files retrieved successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving files (page {PageNumber}, size {PageSize})",
                    pageNumber, pageSize);

                return BaseResponseModel<PagedResponse<FileResponseDto>>.FailureResponse(
                    "An error occurred while retrieving files", ErrorType.InternalServerError);
            }
        }
    }
}