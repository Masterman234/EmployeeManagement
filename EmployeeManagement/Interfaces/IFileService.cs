namespace EmployeeManagement.Interfaces;

public interface IFileService
{
    Task<BaseResponseModel<FileResponseDto>> UploadFileAsync(UploadFileDto request);
    Task<BaseResponseModel<FileResponseDto>> GetFileByIdAsync(Guid id);
    Task<BaseResponseModel<bool>> DeleteFileAsync(Guid id);
}
