using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

    [Required]
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

    public bool IsActive { get; set; } = true;
    [Required]
    public ICollection<UploadedFile> Files { get; set; } = new HashSet<UploadedFile>();
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    public DateTime? LoginAt { get; set; }

    
}