public class FileResponseDto
{
    public Guid Id { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public FileCategory FileCategory { get; set; }
    public DateTime CreatedAt { get; set; }
}