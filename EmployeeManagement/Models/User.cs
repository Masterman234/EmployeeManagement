using System.ComponentModel.DataAnnotations;
namespace EmployeeManagement.Models;

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
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public DateTime? LoginAt { get; set; }



}
