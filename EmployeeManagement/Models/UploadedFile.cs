using EmployeeManagement.Models;

public class UploadedFile : BaseEntity
{
    public int UserId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public FileCategory FileCategory { get; set; }
}
